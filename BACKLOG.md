# DocPlatform — Backlog / Roadmap

Future work, grouped by area. These are **deliberately not built yet**; the architecture
(analyzer pipelines behind `IProjectAnalyzer`/`IAngularAnalyzer`, extractors behind
`IMetadataExtractor`, AI behind `IAIProvider`) is designed so each drops in cleanly.
Inline `// TODO` comments mark the exact spots in code.

## .NET extraction (semantic depth)
- **Generic type resolution** — resolve `Task<Result<T>>`, `Repository<T>` via the `SemanticModel`.
- **Cross-project symbol resolution** — resolve types across project boundaries (one Compilation
  per solution, not per project).
- **Full interface→implementation mapping** — semantically map every interface to its
  implementation(s), not only DI-registered ones.
- **Full route resolution across inheritance** — routes from inherited/base controllers and
  `[ApiController]` conventions. *(TODO in `ControllerAnalyzer`.)*
- **Nested `MapGroup` resolution** — data-flow analysis of group variables for minimal APIs.
  *(TODO in `ControllerAnalyzer`.)*
- **Non-generic DI registrations** — `AddScoped(typeof(IFoo), typeof(Foo))`. *(TODO in `DIAnalyzer`.)*
- **Method-call analysis** — dependencies used beyond constructor injection. *(TODO in `CallGraphAnalyzer`.)*
- **Concurrency detection** — `lock`, `SemaphoreSlim`, `ReaderWriterLockSlim`, EF Core concurrency tokens.
- **Auth deep-dive** — per-endpoint `[Authorize(Roles/Policy)]`, fallback policies, scopes.

## Angular extraction
- Resolve base-URL variables/concatenation fully for API calls.
- Component detail: `@Input`/`@Output`, templates.
- NgModules vs standalone, providers, DI graph on the frontend.
- **Match Angular `apiCalls` to backend controller endpoints** → a real frontend→backend edge in
  the call graph and diagram.

## More languages
- New `IMetadataExtractor`s for **React, Python, Java** — claim the `SkippedProject`s the scanner
  already records instead of skipping them.

## Platform / continuous docs (event-driven)
- **`DocPlatform.Api`** — a service endpoint (`POST /api/changes`) a repo's CI pipeline calls post-build.
- **Repo → application mapping** — repos self-declare their app (`.docplatform.yml` manifest and/or
  the trigger payload); DocPlatform keeps a persisted registry. New repos self-onboard.
- **Incremental updates** — persist the `ApplicationModel`, diff old vs new, regenerate only the
  affected documents (or skip if nothing structural changed).
- **Queue** (`IJobQueue`) — debounce rapid builds, coalesce per application, control throughput.
- **Notifications** (`INotifier`, currently a stub) — fire only on **architectural** change (from the model diff).

## Future capabilities
- **Knowledge-graph database** — cross-application relationships, org-wide dependency map.
- **Impact analysis** — "what breaks if I change this endpoint?"
- **Engineering assistant** — an agent grounded in the estate.
- **Azure AI Foundry provider** — a new `IAIProvider` (one class); the engine doesn't change.
