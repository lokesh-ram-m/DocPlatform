# DocPlatform — AI Engineering Documentation Platform (POC)

Point it at an application's repositories → it reads the code, understands the
structure, asks an AI to write the documentation, and publishes a browsable
documentation website.

> **This is a Proof of Concept.** It is built so it can grow into a production
> "Engineering Knowledge Platform" (continuous docs, knowledge graph, impact
> analysis) **without re-architecting** — every module speaks through a single
> `ApplicationModel` and clean interfaces.

---

## What it does

```
You: "Application = TaskFlow, repos = [Backend, Frontend]"
        │
   1. RepositoryScanner    → discovers projects (.NET API / Angular / library)
   2. MetadataExtractor    → deterministically extracts facts (controllers,
        │                     endpoints, entities, DbContexts, CQRS, Angular routes)
   3. ApplicationModel     → all facts + capabilities + knowledge graph (source of truth)
        │
   4. GitHubModelsProvider → AI explains the model as Markdown (never sees raw code)
   5. MarkdownWriter       → writes grouped docs into the Docusaurus folder
        │
   6. Docusaurus           → renders a documentation website
```

Documentation belongs to an **application**, not a repository — an app can span
many repos, and the docs describe the whole thing.

### The generated documentation (per application)

- **📘 Product Specification** (for PM / Business / End Users) — overview, features, use-cases
- **⚙️ Technical Specification** (for engineers) — architecture (with a **knowledge-graph diagram**),
  technology stack, infrastructure & cloud, data/storage/messaging, security & auth, API reference

---

## Key design principles

1. **Code extracts facts; the AI only explains them.** Deterministic parsing builds the
   `ApplicationModel`; the LLM receives that structured model — **never the raw repository** —
   so it can't invent endpoints or technologies.
2. **The AI is replaceable.** Everything talks to `IAIProvider`. Swap GitHub Models for
   Azure AI Foundry / OpenAI by changing **one class**.
3. **Everything flows through the `ApplicationModel`** — the single source of truth.
4. **Grounded, honest output.** If there's no message queue or cloud service, the docs say so
   rather than hallucinating one.
5. **The knowledge graph is used to understand, not just to draw** — relationships feed the AI
   for accurate architecture, and render as a diagram that always agrees with the prose.

---

## Architecture (Clean Architecture)

```
DocPlatform.sln
└── src/
    ├── DocPlatform.Core            Models (ApplicationModel, Relationship, ...) + interfaces   → depends on nothing
    ├── DocPlatform.Application     Orchestrator, GraphBuilder, DiagramGenerator, classifiers    → Core
    ├── DocPlatform.Infrastructure  RepositoryScanner, Heuristic/Roslyn extractors, MarkdownWriter → Core
    ├── DocPlatform.AI              GitHubModelsProvider : IAIProvider                            → Core
    └── DocPlatform.Console         Composition root / the "Analyze" trigger                      → all
```

Dependencies point **inward** to `Core`. The Documentation Engine depends only on abstractions,
so implementations (AI provider, extractor) are swappable.

### Two swappable metadata extractors

| Extractor | How | Use |
|-----------|-----|-----|
| `HeuristicMetadataExtractor` | regex + file conventions | fast |
| `RoslynMetadataExtractor` | real C# syntax trees (`Microsoft.CodeAnalysis`) | accurate (default) |

Switch in `appsettings.json`: `"Extraction": { "Engine": "Roslyn" }` (or `"Heuristic"`).

---

## Running it

### Prerequisites
- .NET SDK (targets `net9.0`; run with `DOTNET_ROLL_FORWARD=Major` if only a newer runtime is installed)
- Node.js (for the Docusaurus site)
- A **GitHub Models token** (free): fine-grained PAT with **Models: Read-only**

### 1. Configure the token
Copy the template and add your token (this file is gitignored):
```bash
cp src/DocPlatform.Console/appsettings.Development.json.example src/DocPlatform.Console/appsettings.Development.json
# edit it and set Ai:GitHubToken
```

### 2. Generate documentation
```bash
dotnet run --project src/DocPlatform.Console
# enter an application name, then repo paths (one per line, blank to finish)
```
Repeat for as many applications as you like — each is added without disturbing the others.

Tip: set `SCAN_ONLY=1` for a fast detection report **without** calling the AI.

### 3. Browse the site
```bash
cd docs-site
npm install      # first time only
npm run start    # or: npm run build && npm run serve   → http://localhost:3000
```

---

## What it detects

- **.NET**: project types, target frameworks (incl. central `Directory.Build.props`), package
  references, controllers **and minimal-API endpoints**, services, interfaces, entities,
  `DbContext`/`IdentityDbContext`, CQRS commands/queries, `[Authorize]` usage
- **Angular**: components, routes (module-based, standalone, `provideRouter`), dependencies
- **Capabilities** (from packages): auth, data access, database, cloud storage, messaging,
  caching, cloud SDKs, AI/LLM, validation, mapping, logging, testing
- **Knowledge graph**: project dependencies, frontend→API calls, persistence to databases

Validated against real repos: `dotnet-architecture/eShopOnWeb`, `jasontaylordev/CleanArchitecture`.

---

## Roadmap (deliberately NOT built in the POC — the architecture enables them)

| Phase | Capability | Enabled by |
|-------|-----------|-----------|
| 2 | **Continuous docs** via CI/CD (regenerate on merge) | headless/config-driven run + `IAIProvider` |
| 2 | **Change-aware / incremental** updates (only re-read what changed) | persisted `ApplicationModel` + model diff |
| 2 | **Queue** for high build volume (debounce, coalesce per app) | `IJobQueue` abstraction |
| 2 | **Notifications** on architectural change | `INotifier` (diff old vs new model) |
| 3 | **Knowledge Graph** database + cross-application analysis | already model relationships |
| 4 | **Impact analysis / engineering assistant** | the graph |
| — | More stacks (Node, Python, React), Azure AI Foundry provider | new `IMetadataExtractor` / `IAIProvider` |

---

## Not in scope for the POC
No CI/CD, Azure DevOps, GitHub Actions, queues/Service Bus, knowledge-graph database,
incremental updates, or repository change detection — these are future phases. The POC
validates the workflow and proves the architecture is ready for them.
