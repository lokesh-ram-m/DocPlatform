# DocPlatform — AI Engineering Documentation Platform

Point it at an application's repositories → it reads the code, understands the
structure, asks an AI to write the documentation, and publishes a browsable
documentation website. Documentation that stays current because it's generated
from the code itself.

> Built to grow into a production **Engineering Knowledge Platform** (continuous docs,
> knowledge graph, impact analysis) **without re-architecting** — every module speaks
> through a single `ApplicationModel` and clean, swappable interfaces.

---

## What it does

```
apps.yml: { application: TaskFlow, repos: [Backend, Frontend] }
        │
   1. RepositoryScanner    → discovers projects (.NET / Angular) + logs unsupported ones
   2. Metadata extractors  → deterministically extract facts via analyzer pipelines
        │                     (.NET = Roslyn/semantic, Angular = heuristic)
   3. ApplicationModel     → all facts + capabilities + knowledge graph + call graph
        │                     + architecture patterns (the single source of truth)
   4. GitHubModelsProvider → AI explains the model as Markdown (never sees raw code)
   5. MarkdownWriter       → writes grouped docs, per application
   6. Docusaurus           → renders the documentation website
```

Documentation belongs to an **application**, not a repository — an app can span
many repos, and the docs describe the whole thing.

### The generated documentation (per application)

- **📘 Product Specification** (PM / Business / End Users) — overview, features, use-cases
- **⚙️ Technical Specification** (engineers) — architecture (with a **knowledge-graph diagram**
  and a **component call-graph diagram**), technology stack, infrastructure & cloud,
  data/storage/messaging, security & auth, API reference

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
5. **Composable analyzers.** Extraction is a pipeline of small analyzers — add a capability by
   adding an analyzer, never by touching the others.

---

## Architecture (Clean Architecture)

```
DocPlatform.sln
└── src/
    ├── DocPlatform.Core            ApplicationModel + interfaces          → depends on nothing
    ├── DocPlatform.Application     Orchestrator, GraphBuilder, Diagram,   → Core
    │                               CapabilityClassifier, ArchitectureDetector
    ├── DocPlatform.Infrastructure  Scanner + analyzer pipelines + Writer  → Core
    ├── DocPlatform.AI              GitHubModelsProvider : IAIProvider     → Core
    └── DocPlatform.Console         Composition root / entry point         → all
```

Dependencies point **inward** to `Core`. The engine depends only on abstractions, so the
AI provider and the extractors are swappable.

### Extraction = analyzer pipelines

A `CompositeMetadataExtractor` routes each project to the right pipeline:

- **.NET** — `RoslynMetadataExtractor` builds a `Compilation` + `SemanticModel` and runs a
  pipeline of `IProjectAnalyzer`s: Controller, DbContext, Entity, Service, **DI**,
  **CallGraph**, CQRS, Auth. (A regex-based `HeuristicMetadataExtractor` is the fast alternative —
  switch via `appsettings.json` → `"Extraction": { "Engine": "Roslyn" }`.)
- **Angular** — `AngularMetadataExtractor` runs a pipeline of `IAngularAnalyzer`s (heuristic,
  since there's no Roslyn for TypeScript): Component, Service, Route, **Http (API calls)**, Guard.

---

## Running it

The tool is **config-driven** — declare your estate in `apps.yml`:

```yaml
applications:
  - name: eShopOnWeb
    repos:
      - https://github.com/dotnet-architecture/eShopOnWeb.git   # git URL (cloned) or local path
```

### Prerequisites
- **.NET SDK** (projects target `net9.0`; prefix commands with `DOTNET_ROLL_FORWARD=Major` if only a newer runtime is installed)
- **Node.js** (for the Docusaurus site)
- A **GitHub Models token** (free): fine-grained PAT with **Models: Read-only** — only needed to generate, not to scan

### 1. Configure the token
```bash
cp src/DocPlatform.Console/appsettings.Development.json.example src/DocPlatform.Console/appsettings.Development.json
# edit it and set Ai:GitHubToken
```

### 2. Run it (two modes)
```bash
# Generate documentation for every app in apps.yml
dotnet run --project src/DocPlatform.Console -- --config apps.yml

# Scan only — a detection report, NO AI calls, no token needed
SCAN_ONLY=1 dotnet run --project src/DocPlatform.Console -- --config apps.yml
```
`--config` defaults to `apps.yml` at the repo root if omitted.

### 3. Browse the site
```bash
cd docs-site
npm install      # first time only
npm run start    # → http://localhost:3000   (or: npm run build && npm run serve)
```

### CI/CD (continuous docs)
`.github/workflows/docs.yml` runs the tool headless on every push, builds the site, and deploys
to **GitHub Pages**. Set the repo secret **`AI_TOKEN`** and enable **Pages → Source: GitHub Actions**.

---

## What it detects

**.NET (semantic, via Roslyn):**
- Project types, target frameworks (incl. central `Directory.Build.props`), packages
- **Controllers + minimal-API endpoints** with **resolved final routes** (`GET /api/orders/{id}`,
  `[controller]` tokens, `MapGroup` prefixes)
- **Entities** via `DbSet<T>`, base-class chains, and folders (not folders alone)
- **Services** — including implementations registered through **DI** (`AddScoped<I,Impl>`) that
  don't end in "Service"
- `DbContext`/`IdentityDbContext`, **CQRS** commands/queries
- **Call graph** — controller → service → repository → data (constructor injection)
- **Architecture patterns** — Clean / Layered / CQRS / Repository / DI (with evidence)
- **Auth** — schemes (JWT/Cookie/OIDC/Identity), policies, roles, per-controller `[Authorize]`

**Angular (heuristic):**
- Components (+ selector), **services (`@Injectable`)**, routes (→ component / lazy),
  **guards & interceptors**, and the **backend API endpoints it calls** (base-URL resolved)

**Cross-cutting:**
- **Capabilities** from packages: auth, data, database, cloud storage, messaging, caching,
  cloud SDKs, AI/LLM, validation, mapping, logging, testing
- **Unsupported projects** (React/Vue/Node/Python/Java/Go/Ruby/PHP) are **logged and recorded**,
  not errored — a hook for future language extractors

Validated against real repos: `dotnet/eShop`, `dotnet-architecture/eShopOnWeb`,
`jasontaylordev/CleanArchitecture`.

---

## Roadmap

See [`BACKLOG.md`](BACKLOG.md). Highlights: event-driven / incremental docs (diff the model,
regenerate only what changed), a queue for high build volume, notifications on architectural
change, a knowledge-graph database + impact analysis, more language extractors, and an
Azure AI Foundry provider. The architecture already enables each of these.
