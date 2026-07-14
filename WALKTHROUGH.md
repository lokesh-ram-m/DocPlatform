# DocPlatform — Walkthrough & Explainer

Read this end-to-end and you'll be able to explain the project cold: *what it does,
why it exists, why the AI is there, how the code analysis works, why each piece is
built the way it is, and where it extends.*

---

## 1. The pitch

**30-second version:**
> "DocPlatform reads an application's repositories, understands its architecture from
> the actual code, and uses AI to generate browsable Product and Technical documentation
> — organized per application, not per repo. The goal is documentation that stays current
> automatically instead of rotting."

**2-minute version (the problem → the solution):**
> "Every company has the same problem: documentation is always out of date, because
> humans have to maintain it. Meanwhile the knowledge of how a system works lives in
> people's heads. DocPlatform makes documentation a **byproduct of the code itself** —
> point it at the repositories, and it produces accurate specs in seconds. The key design
> choice is that **deterministic code extracts the facts** (controllers, endpoints,
> entities, dependencies) and **the AI only explains them** — so it can't hallucinate.
> It's a POC today, but it's architected to grow into a continuous, always-current
> Engineering Knowledge Platform."

---

## 2. Why AI — and, just as important, why NOT *only* AI

This is the single most important thing to explain well.

**Why we need AI:** Turning a pile of structured facts (5 controllers, 12 endpoints,
these entities, these packages) into readable, well-organized prose — an architecture
narrative, a product overview a business person understands — is exactly what LLMs are
good at. Writing that by hand for every app, kept current, is impossible.

**Why NOT just throw the repo at the AI:** If you paste raw code into an LLM and say
"document this," it will **hallucinate** — invent endpoints, guess technologies, state
architecture as fact. You can't trust it. And you'd blow past context limits on any real
repo.

**Our answer — the split that makes it trustworthy:**

| Job | Who does it | Why |
|-----|-------------|-----|
| **Discover the facts** (what controllers/entities/deps exist) | **Deterministic code** | Precise, verifiable, never invents |
| **Explain the facts** (write the prose) | **The AI** | Good at language, organization, tone |

> The one-liner to remember: **"Code finds the facts; the AI explains them. The AI never
> sees raw code — only a structured model we already verified."**

That's why, in every generated doc, if there's no message queue, it says *"none detected"*
instead of inventing RabbitMQ. That honesty is the whole product.

---

## 3. What we deliberately did NOT build (and why)

Being able to explain scope is a sign of maturity. We intentionally left out:

- **CI/CD / GitHub Actions / Azure DevOps triggers** (as the *core* — we did add a basic
  publish pipeline)
- **Knowledge-graph database (Neo4j), queues/Service Bus, event-driven ingestion**
- **Impact analysis, PR generation, governance, vulnerability/test analysis**
- **Incremental/change detection**

**Why not:** a POC's job is to **validate the workflow and prove the architecture is ready**
— not to build the whole platform. Building those now would be scope creep and risk shipping
nothing solid. **But** every one of them is *enabled* by the design (see §10 Extension points),
which is the point: *simple now, no shortcuts that block the future.*

---

## 4. The big picture (the pipeline)

```
You: "Application = X, repos = [...]"
      │
 1. RepositoryScanner    → find projects (.NET API / Angular / library)
 2. MetadataExtractor    → extract FACTS (controllers, endpoints, entities, DbContexts,
      │                     CQRS, Angular routes) — deterministically
 3. ApplicationModel     → all facts + technologies + capabilities + knowledge graph
      │                     (THE single source of truth)
 4. GitHubModelsProvider → AI turns the model into Markdown (never sees raw code)
 5. MarkdownWriter       → writes grouped docs per application
      │
 6. Docusaurus           → renders the documentation website
```

**Documentation belongs to an *application*, not a repo.** An app can span many repos
(backend + frontend + services); the docs describe the whole thing. Repos are just sources.

---

## 5. Architecture — Clean Architecture and the dependency rule

```
DocPlatform.sln
└── src/
    ├── DocPlatform.Core            models + interfaces          → depends on NOTHING
    ├── DocPlatform.Application     orchestrator, graph, classifiers → Core
    ├── DocPlatform.Infrastructure  scanner, extractors, writer, git → Core
    ├── DocPlatform.AI              GitHubModelsProvider             → Core
    └── DocPlatform.Console         composition root / entry point   → all of the above
```

**The rule: dependencies point *inward* to Core.** Core defines *interfaces*
(`IAIProvider`, `IRepositoryScanner`, `IMetadataExtractor`, `IMarkdownWriter`);
the outer projects *implement* them.

**Why this matters (say this):** because the engine depends only on interfaces, we can
swap implementations — a different AI, a different extractor — **without touching the core
logic.** That's what makes the whole thing future-proof. It's the same reason we could
add Roslyn alongside regex, and the same reason Azure AI Foundry is a one-class change.

---

## 6. File by file — `DocPlatform.Core` (the heart)

Core has **no dependencies** and no logic — just the shared model and the contracts.

- **`Models/ApplicationModel.cs`** — THE source of truth. Contains:
  - `ApplicationModel` — Name, Repositories, Technologies, Capabilities, Relationships
  - `RepositoryModel` — a repo and its Projects
  - `ProjectModel` — kind, target framework, package/project references, Controllers,
    Services, Interfaces, Entities, DbContexts, CqrsRequests, HasAuthentication, Angular info
  - `ProjectKind` enum — DotNetApi / DotNetLibrary / DotNetOther / Angular / Unknown
  - **Why one big model:** every module produces or consumes it. It's the contract that lets
    the pieces stay decoupled and lets us grow into a knowledge graph later.
- **`Models/DetectedCapability.cs`** — `{ Category, Name, Source }` — a grounded capability
  (e.g. Messaging / RabbitMQ / RabbitMQ.Client).
- **`Models/Relationship.cs`** — `{ From, To, Type }` — one edge of the knowledge graph.
- **`Models/DocumentationResult.cs`** — `GeneratedDocument { Group, FileName, Markdown, Order }`
  + `DocumentationResult` (list + ApplicationName). The output of the AI step.
- **`Abstractions/` (the interfaces)** — `IAIProvider`, `IRepositoryScanner`,
  `IMetadataExtractor`, `IMarkdownWriter`. **Why interfaces in Core:** so the engine depends
  on *abstractions*, not concrete classes. This is the "seam" that makes everything swappable.

---

## 7. File by file — `DocPlatform.Infrastructure` (how code analysis works)

This is where the "understand the code" magic happens. Explain these carefully.

### `Scanning/RepositoryScanner.cs` — discovery
- Walks the repo's files (skipping `bin/obj/node_modules/.git/dist`).
- **.NET projects:** finds `.csproj`, parses them as **XML** (`System.Xml.Linq`). Reads:
  - the SDK attribute → `Microsoft.NET.Sdk.Web` ⇒ **DotNetApi**; `Exe` output ⇒ console; else **library**
  - `TargetFramework` (and if missing, walks up to a central **`Directory.Build.props`** — real repos centralize it)
  - `PackageReference` and `ProjectReference` entries
- **Angular projects:** finds `package.json` containing `@angular/core`.
- **Why XML parsing (not regex) for csproj:** a csproj *is* structured XML — parsing it is
  reliable and correct. (We use regex/Roslyn only for `.cs` source, which isn't structured data.)
- **Key gotcha we fixed:** csproj uses Windows `\` paths; on macOS `Path.GetFileNameWithoutExtension`
  didn't split them → we normalize `\`→`/`. (Great "we tested on real repos" story.)

### Extraction = analyzer pipelines
A `CompositeMetadataExtractor` (implements `IMetadataExtractor`) routes each project to the
right pipeline: **.NET → the .NET extractor**, **Angular → the Angular extractor**. Each only
touches the project kinds it supports.

**.NET — `RoslynMetadataExtractor` (semantic, the default)**
- Roslyn = the C# compiler as a library (`Microsoft.CodeAnalysis`). It parses each `.cs` file
  into a **syntax tree**, and builds a per-project **`Compilation` + `SemanticModel`** so it can
  *resolve symbols* — base-type chains across files, generic arguments, etc. (External framework
  types may not resolve; the project's own symbols do — no build/restore needed.)
- It runs a **pipeline of `IProjectAnalyzer`s**, each a small focused pass:
  `ControllerAnalyzer` (routes), `DbContextAnalyzer`, `EntityAnalyzer` (DbSet + base classes),
  `ServiceAnalyzer`, **`DIAnalyzer`** (registrations → interface/impl map), **`CallGraphAnalyzer`**
  (constructor injection → controller→service→repo edges), `CqrsAnalyzer`, `AuthAnalyzer`.
- **`HeuristicMetadataExtractor`** is the fast **regex** alternative (no compilation) — swap via
  `appsettings.json` → `"Extraction": { "Engine": "Heuristic" }`.
- **The line to say:** *"regex guesses from text; Roslyn understands the code. Both are
  deterministic — Roslyn is just higher-fidelity. New capabilities are just new analyzers in the
  pipeline — we never touch the others."*

**Angular — `AngularMetadataExtractor` (heuristic)**
- There's no Roslyn for TypeScript, so this is a **heuristic pipeline of `IAngularAnalyzer`s**
  over the `.ts` files: `AngularComponentAnalyzer` (+ selector), `AngularServiceAnalyzer`
  (`@Injectable`), `AngularRouteAnalyzer` (route → component / lazy), **`AngularHttpAnalyzer`**
  (the **backend API endpoints the app calls**, base-URL resolved), `AngularGuardAnalyzer`
  (guards + interceptors).
- **Why this matters:** the Angular app's detected API calls line up with the backend controllers'
  endpoints — the frontend and backend connect in the application view.

- **`Extraction/ExtractionHelpers.cs`** — shared bits: the file walker, the entity filter
  (drops `Dto`/`Response`/`Request`/`BaseEntity`), and list de-duplication.
- **`Application/CapabilityClassifier`, `GraphBuilder`, `DiagramGenerator`, `ArchitectureDetector`**
  turn the raw facts into capabilities, the knowledge graph, the diagrams, and the detected
  architecture patterns (Clean / Layered / CQRS / Repository / DI).

### `Output/MarkdownWriter.cs` — writing
- Writes `docs/<app>/<group>/<file>.md` — **per application**, so multiple apps coexist and a
  run only touches its own app's folder.
- Writes Docusaurus `_category_.json` at each level (turns folders into sidebar sections).
- Regenerates the homepage `index.md` listing every documented application.

### `Sourcing/GitSourceResolver.cs` — for CI
- A repo entry that's a **git URL** → shallow-cloned; a **local path** → used as-is. This is
  what lets headless/CI mode pull source automatically.

---

## 8. File by file — `DocPlatform.Application` (the orchestration + brains)

- **`DocumentationOrchestrator.cs`** — the conductor. Two methods:
  - `BuildModel(...)` — scan → extract → aggregate technologies → classify capabilities →
    build knowledge graph. **Deterministic only, no AI.** (Also used by `SCAN_ONLY` for a
    fast detection report.)
  - `GenerateAsync(...)` — `BuildModel` + call the AI + **inject the diagram** into
    `architecture.md` + write. It knows *nothing* about which AI or extractor is used — only
    the interfaces. That's the dependency rule in action.
- **`TechnologyAggregator.cs`** — rolls up the app-wide tech list from the projects.
- **`CapabilityClassifier.cs`** — **grounding engine.** Maps package fragments → categories
  (JwtBearer→Authentication, Dapper→Data Access, RabbitMQ.Client→Messaging, Azure.*→Cloud…).
  **Why:** gives the AI *grounded facts* so the Technical Spec is accurate and honest ("no
  queue detected") instead of guessed.
- **`GraphBuilder.cs`** — builds the **knowledge graph** edges: `depends on` (from
  ProjectReferences), `calls` (Angular→API), `persists to` (data project→database). Excludes
  test projects. **Why a graph:** so the AI *knows* how components relate instead of guessing,
  and so we can draw it.
- **`DiagramGenerator.cs`** — turns the graph into a **Mermaid** flowchart. **Deterministic**,
  so the diagram can never disagree with the code; the AI prose is built from the same graph,
  so picture and words always match.

---

## 9. File by file — `DocPlatform.AI` (how we use the LLM safely)

- **`GitHubModelsProvider.cs`** — implements `IAIProvider`. Uses the **Azure AI Inference SDK**
  to call **GitHub Models** (free tier). For each document it:
  1. serializes the `ApplicationModel` to JSON (enum names, not numbers),
  2. sends the system prompt + that JSON to the model,
  3. collects the Markdown.
  - **Why GitHub Models:** free, and it uses the *same Azure AI SDK* as Azure AI Foundry — so
    moving to Foundry later is a one-class change.
  - **Why behind `IAIProvider`:** the engine has no idea which LLM it's using.
- **`Prompts/DocumentationPrompts.cs`** — the "brains" of quality. Defines:
  - The **system prompt** — the rules: *use ONLY the metadata; never invent; if something isn't
    present, say so; hedge architecture ("Detected Patterns"); output clean Markdown.*
  - `Plan(...)` — yields a `DocSpec` for each document across the **two audiences**:
    - **Product Specification** (PM/BU/end users): overview, features, use-cases
    - **Technical Specification** (engineers): architecture (+ the graph diagram), technology-stack,
      infrastructure-and-cloud, data-storage-and-messaging, security-and-authentication, api-reference
  - **Why two specs:** different readers need different language. Same facts, two lenses.

---

## 10. `DocPlatform.Console` (the entry point)

- **`Program.cs`** — the **composition root**. It reads config, builds the DI container (wiring
  the concrete scanner/extractors/AI/writer to the interfaces), and runs from **`apps.yml`**
  (cloning git URLs via `GitSourceResolver`). Two ways to invoke it:
  - **Generate:** `dotnet run -- --config apps.yml` — documents every app in the file. This is what CI runs.
  - **Scan only:** `SCAN_ONLY=1 dotnet run -- --config apps.yml` — a detection report, **no AI**,
    no token needed (cheap testing; uses a `NoOpAIProvider`).
  - `--config` defaults to `apps.yml` at the repo root.
- **`apps.yml`** — declares the applications and their repos (git URLs or local paths).
- **`appsettings.json`** — non-secret config (AI endpoint/model, extractor engine, output folder).
- **`appsettings.Development.json`** — the **token** (gitignored). `.example` template is committed.

---

## 11. The docs site — `docs-site/` (Docusaurus)

- Docusaurus renders the generated Markdown into a website. Its **only** job is rendering.
- We enabled the **Mermaid theme** so the knowledge-graph diagram renders.
- We parse docs as CommonMark (`markdown.format: 'md'`) so `{id}` / `Task<T>` in API docs don't
  break MDX. `url`/`baseUrl` are env-configurable for GitHub Pages.

---

## 12. CI/CD — how to configure it and how it works

### What's built now (centralized publish pipeline)
- **`apps.yml`** — declares the applications + their repos (git URLs or local paths).
- **`.github/workflows/docs.yml`** — on push (or manual): checkout → run the tool headless
  (`--config apps.yml`, clones repos, generates docs) → build Docusaurus → **deploy to GitHub Pages**.

**How to configure it (3 steps):**
1. Repo **secret** `AI_TOKEN` = your GitHub Models token (used as env `Ai__GitHubToken`).
2. **Settings → Pages → Source: GitHub Actions.**
3. Push → the workflow runs → docs go live at `https://<user>.github.io/DocPlatform/`.

### The event-driven model you actually want (designed, not yet built)
> "Each source repo's own pipeline, post-build, pings DocPlatform: *'I changed.'* DocPlatform —
> a running service — figures out which application that repo belongs to, and updates **only
> that application's** docs, **incrementally**."

Pieces it needs (all designed):
- **`DocPlatform.Api`** — `POST /api/changes { repository, application }`.
- **Repo → application mapping** — each repo **declares its app** (a `.docplatform.yml`
  manifest and/or the trigger payload). DocPlatform keeps a **persisted registry** (app → repos).
- **Routing:** app not in registry ⇒ **new app, full generation**; app exists but repo is new ⇒
  **new service, regenerate that app**; existing repo changed ⇒ **incremental** (diff the stored
  `ApplicationModel` vs the new one → regenerate only the affected docs, or skip if nothing changed).
- **Queue + worker** (debounce + coalesce per app) so many simultaneous builds don't stampede.
- **`INotifier`** (logs for now) — fire only on **architectural** change (detected by the model diff).
- **Reality:** for real cross-repo triggers the service must be **hosted** (e.g. Azure App Service).
  Locally we demo it by simulating a repo's ping with a `curl`.

---

## 13. Extension points (where it grows — memorize these)

Every extension is "just implement an interface":

| Want to… | Do this | Because |
|----------|---------|---------|
| Use Azure AI Foundry / OpenAI | new `IAIProvider` | engine only knows the interface |
| Support Node/Python/React | new `IMetadataExtractor` (+ scanner rules) | extraction is swappable |
| Higher-accuracy .NET parsing | already done — Roslyn extractor | swap via config |
| Continuous docs | the event-driven service (§12) | headless mode + registry enable it |
| Cross-app knowledge graph | persist relationships to a graph DB | we already model them |
| Impact analysis / assistant | query the graph | the model is the foundation |

---

## 14. Anticipated questions (and your answers)

- **"Does it hallucinate?"** → No — the AI only receives verified facts, never raw code, and it
  says "none detected" rather than inventing. The diagram is deterministic.
- **"Why not just use ChatGPT on the repo?"** → Context limits + hallucination + no structure.
  Our `ApplicationModel` is what makes it reliable and extensible.
- **"Regex or Roslyn?"** → Both exist; Roslyn is the accurate default. Swappable by one config
  line thanks to the interface.
- **"Why GitHub Models?"** → Free for the POC, and same SDK as Azure AI Foundry, so migration
  is one class.
- **"How does it know which repos are one app?"** → The repo declares it (manifest / trigger
  payload); DocPlatform keeps a registry. New repos self-onboard.
- **"Is it production-ready?"** → It's a POC that *validates the workflow and proves the
  architecture*. The roadmap (CI/CD, incremental, knowledge graph) drops in without a rewrite.

---

## 15. Key terms (glossary)

- **ApplicationModel** — the structured facts about an application; the single source of truth.
- **Deterministic extraction** — facts found by code (not AI), so they're verifiable.
- **Grounding** — giving the AI only real, verified facts so it can't invent.
- **Roslyn** — the C# compiler exposed as a library; lets us parse code into a syntax tree.
- **Capability** — a categorized technology detected from packages (auth, messaging, cloud…).
- **Knowledge graph** — the modeled relationships between projects (depends-on / calls / persists-to).
- **IAIProvider / IMetadataExtractor** — the interfaces that make the AI and the parser swappable.

---

*You now have the whole thing. Read it once on the commute; tomorrow we do a quick lock-in on
whatever's fuzzy.*
