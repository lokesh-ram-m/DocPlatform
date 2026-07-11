using DocPlatform.Core.Abstractions;
using DocPlatform.Core.Models;
using DocPlatform.Infrastructure.Scanning;

// TEMPORARY test harness for Step 2 — verify the RepositoryScanner.
// (The real "create app / select repos / Analyze" console comes in Step 6.)

string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
string[] repoPaths =
{
    Path.Combine(home, "lokesh-ram/projects/task-management-app/TaskManagementBackend"),
    Path.Combine(home, "lokesh-ram/projects/task-management-app/TaskManagementFrontend")
};

IRepositoryScanner scanner = new RepositoryScanner();

Console.WriteLine("=== Scanning repositories ===\n");
foreach (string path in repoPaths)
{
    RepositoryModel repo = scanner.Scan(path);
    Console.WriteLine($"📦 Repository: {repo.Name}   (README: {(repo.HasReadme ? "yes" : "no")})");
    foreach (ProjectModel p in repo.Projects)
    {
        Console.WriteLine($"   └─ {p.Kind,-14} {p.Name}   [{p.TargetFramework ?? "-"}]");
        if (p.ProjectReferences.Count > 0)
            Console.WriteLine($"        refs: {string.Join(", ", p.ProjectReferences)}");
        if (p.PackageReferences.Count > 0)
            Console.WriteLine($"        packages: {string.Join(", ", p.PackageReferences.Take(4))}{(p.PackageReferences.Count > 4 ? " …" : "")}");
        if (p.Angular is not null)
            Console.WriteLine($"        angular deps: {string.Join(", ", p.Angular.Dependencies.Where(d => d.StartsWith("@angular")).Take(3))} …");
    }
    Console.WriteLine();
}
