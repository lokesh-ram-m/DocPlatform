using DocPlatform.Core.Models;

namespace DocPlatform.Application;

// Builds the application's knowledge graph deterministically:
//   - Project ──depends on──▶ Project   (from ProjectReferences)
//   - Angular ──calls──▶ Web API        (frontend consuming the backend)
//   - Project ──persists to──▶ Database (from DbContexts / data packages)
public static class GraphBuilder
{
    private static readonly string[] DataPackages = { "Dapper", "EntityFrameworkCore", "SqlClient", "Npgsql", "MongoDB" };

    public static List<Relationship> Build(ApplicationModel app)
    {
        var relationships = new List<Relationship>();
        // Exclude test projects — they're noise on an architecture diagram.
        List<ProjectModel> projects = app.Repositories.SelectMany(r => r.Projects)
            .Where(p => !IsTestProject(p.Name))
            .ToList();
        var projectNames = new HashSet<string>(projects.Select(p => p.Name), StringComparer.OrdinalIgnoreCase);

        // depends on (project references that resolve to a known project)
        foreach (ProjectModel p in projects)
            foreach (string reference in p.ProjectReferences)
                if (projectNames.Contains(reference))
                    relationships.Add(new Relationship { From = p.Name, To = reference, Type = "depends on" });

        // frontend calls API
        List<ProjectModel> apis = projects.Where(p => p.Kind == ProjectKind.DotNetApi).ToList();
        foreach (ProjectModel ng in projects.Where(p => p.Kind == ProjectKind.Angular))
            foreach (ProjectModel api in apis)
                relationships.Add(new Relationship { From = ng.Name, To = api.Name, Type = "calls" });

        // persists to database
        List<string> databases = app.Capabilities
            .Where(c => c.Category == "Database")
            .Select(c => c.Name).Distinct().ToList();
        if (databases.Count == 0 && projects.Any(p => p.DbContexts.Count > 0))
            databases.Add("Database");

        // Prefer the project that owns a DbContext. Only fall back to data-package
        // detection when the app has no DbContext anywhere (e.g. Dapper apps) — this
        // keeps EF apps from wiring every layer to the database.
        bool appHasDbContext = projects.Any(p => p.DbContexts.Count > 0);
        foreach (ProjectModel p in projects)
        {
            bool dataProject = p.DbContexts.Count > 0
                || (!appHasDbContext && p.PackageReferences.Any(x => DataPackages.Any(d => x.Contains(d, StringComparison.OrdinalIgnoreCase))));
            if (dataProject)
                foreach (string db in databases)
                    relationships.Add(new Relationship { From = p.Name, To = db, Type = "persists to" });
        }

        // de-duplicate
        return relationships
            .GroupBy(r => $"{r.From}|{r.To}|{r.Type}", StringComparer.OrdinalIgnoreCase)
            .Select(g => g.First())
            .ToList();
    }

    private static bool IsTestProject(string name) =>
        name.EndsWith("Tests", StringComparison.OrdinalIgnoreCase) ||
        name.EndsWith("Test", StringComparison.OrdinalIgnoreCase);
}
