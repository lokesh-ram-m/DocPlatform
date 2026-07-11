using DocPlatform.Core.Abstractions;
using DocPlatform.Core.Models;
using DocPlatform.Infrastructure.Extraction;
using DocPlatform.Infrastructure.Scanning;

// TEMPORARY test harness for Steps 2–3 — scanner + metadata extractor.
// (The real "create app / select repos / Analyze" console comes in Step 6.)

string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
string[] repoPaths =
{
    Path.Combine(home, "lokesh-ram/projects/task-management-app/TaskManagementBackend"),
    Path.Combine(home, "lokesh-ram/projects/task-management-app/TaskManagementFrontend")
};

IRepositoryScanner scanner = new RepositoryScanner();
IMetadataExtractor extractor = new MetadataExtractor();

foreach (string path in repoPaths)
{
    RepositoryModel repo = scanner.Scan(path);
    extractor.Extract(repo);

    Console.WriteLine($"📦 {repo.Name}");
    foreach (ProjectModel p in repo.Projects)
    {
        Console.WriteLine($"   └─ {p.Kind} : {p.Name} [{p.TargetFramework ?? "-"}]  auth={p.HasAuthentication}");
        if (p.Controllers.Count > 0)
        {
            Console.WriteLine("      Controllers:");
            foreach (ControllerModel c in p.Controllers)
                Console.WriteLine($"        • {c.Name}  route={c.Route}  actions=[{string.Join(", ", c.Actions)}]");
        }
        if (p.Services.Count > 0)   Console.WriteLine($"      Services:   {string.Join(", ", p.Services)}");
        if (p.Interfaces.Count > 0) Console.WriteLine($"      Interfaces: {string.Join(", ", p.Interfaces)}");
        if (p.Entities.Count > 0)   Console.WriteLine($"      Entities:   {string.Join(", ", p.Entities)}");
        if (p.DbContexts.Count > 0) Console.WriteLine($"      DbContexts: {string.Join(", ", p.DbContexts)}");
        if (p.Angular is not null)
        {
            Console.WriteLine($"      Components ({p.Angular.Components.Count}): {string.Join(", ", p.Angular.Components)}");
            Console.WriteLine($"      Routes:     {string.Join(", ", p.Angular.Routes)}");
        }
    }
    Console.WriteLine();
}
