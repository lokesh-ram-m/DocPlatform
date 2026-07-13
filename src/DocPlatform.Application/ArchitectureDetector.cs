using DocPlatform.Core.Models;

namespace DocPlatform.Application;

// Heuristically detects architecture patterns from the ApplicationModel signals
// (project layering, CQRS types, repository interfaces, the call graph, DI).
// Deterministic, evidence-based — the docs present these as "detected", never certain.
public static class ArchitectureDetector
{
    public static List<DetectedPattern> Detect(ApplicationModel app)
    {
        var patterns = new List<DetectedPattern>();
        List<ProjectModel> projects = app.Repositories.SelectMany(r => r.Projects).ToList();
        var names = projects.Select(p => p.Name.ToLowerInvariant()).ToList();

        bool HasLayer(string token) => names.Any(n => n.Contains(token));

        // Clean Architecture — distinct Domain / Application / Infrastructure layers.
        bool clean = HasLayer("domain") && HasLayer("application") && HasLayer("infrastructure");
        if (clean)
            patterns.Add(new DetectedPattern
            {
                Name = "Clean Architecture",
                Evidence = "Separate Domain, Application, and Infrastructure projects."
            });

        // CQRS — Command/Query types and/or MediatR.
        int cqrs = projects.Sum(p => p.CqrsRequests.Count);
        bool mediatr = projects.Any(p => p.PackageReferences.Any(x => x.Contains("MediatR", StringComparison.OrdinalIgnoreCase)));
        if (cqrs > 0 || mediatr)
            patterns.Add(new DetectedPattern
            {
                Name = "CQRS",
                Evidence = $"{cqrs} command/query type(s){(mediatr ? " with MediatR" : "")}."
            });

        // Repository Pattern — I*Repository interfaces.
        List<string> repoInterfaces = projects.SelectMany(p => p.Interfaces)
            .Where(i => i.EndsWith("Repository")).Distinct().ToList();
        if (repoInterfaces.Count > 0)
            patterns.Add(new DetectedPattern
            {
                Name = "Repository Pattern",
                Evidence = $"Repository interfaces: {string.Join(", ", repoInterfaces.Take(3))}{(repoInterfaces.Count > 3 ? " …" : "")}."
            });

        // Layered — controller -> service -> repository flow (unless it's Clean, which supersedes).
        bool controllerToRepo = app.CallGraph.Any(e => e.From.EndsWith("Controller"))
                             && app.CallGraph.Any(e => e.To.EndsWith("Repository"));
        if (controllerToRepo && !clean)
            patterns.Add(new DetectedPattern
            {
                Name = "Layered Architecture",
                Evidence = "Controller → service → repository separation in the call graph."
            });

        // Dependency Injection — registrations / constructor injection.
        if (projects.Any(p => p.ServiceImplementations.Count > 0) || app.CallGraph.Count > 0)
            patterns.Add(new DetectedPattern
            {
                Name = "Dependency Injection",
                Evidence = "Components resolved via constructor injection and DI registrations."
            });

        return patterns;
    }
}
