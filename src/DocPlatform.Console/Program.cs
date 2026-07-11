using DocPlatform.AI;
using DocPlatform.Application;
using DocPlatform.Core.Abstractions;
using DocPlatform.Core.Models;
using DocPlatform.Infrastructure.Extraction;
using DocPlatform.Infrastructure.Output;
using DocPlatform.Infrastructure.Scanning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// ==========================================================================
//  DocPlatform Console — the POC's "Analyze" trigger.
//  User creates an application, selects repositories, clicks Analyze.
// ==========================================================================

// --- Configuration ---
IConfiguration config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile("appsettings.Development.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

string endpoint = config["Ai:Endpoint"]!;
string model = config["Ai:Model"]!;
string token = config["Ai:GitHubToken"] ?? Environment.GetEnvironmentVariable("GITHUB_TOKEN") ?? "";

if (string.IsNullOrWhiteSpace(token))
{
    Console.WriteLine("❌ No AI token. Add Ai:GitHubToken to appsettings.Development.json or set GITHUB_TOKEN.");
    return;
}

// --- Dependency Injection (composition root) ---
// Choose the extraction engine from config: "Roslyn" (accurate) or "Heuristic" (fast regex).
string engine = config["Extraction:Engine"] ?? "Roslyn";

var services = new ServiceCollection();
services.AddSingleton<IRepositoryScanner, RepositoryScanner>();
services.AddSingleton<IMetadataExtractor>(_ =>
    engine.Equals("Heuristic", StringComparison.OrdinalIgnoreCase)
        ? new HeuristicMetadataExtractor()
        : new RoslynMetadataExtractor());
services.AddSingleton<IMarkdownWriter, MarkdownWriter>();
services.AddSingleton<IAIProvider>(_ => new GitHubModelsProvider(endpoint, model, token));
services.AddSingleton<DocumentationOrchestrator>();
ServiceProvider provider = services.BuildServiceProvider();

var orchestrator = provider.GetRequiredService<DocumentationOrchestrator>();

// --- UI ---
Console.WriteLine("========================================");
Console.WriteLine("  DocPlatform — AI Documentation (POC)");
Console.WriteLine("========================================\n");

// 1. Create an application
Console.Write("Application name: ");
string appName = ReadNonEmpty("TaskFlow");

// 2. Select repositories
Console.WriteLine("\nAdd repository paths (one per line, blank line to finish):");
var repoPaths = new List<string>();
while (true)
{
    Console.Write("  repo> ");
    string? line = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(line)) break;
    string path = Expand(line.Trim());
    if (Directory.Exists(path)) repoPaths.Add(path);
    else Console.WriteLine($"    ⚠️ not found, skipped: {path}");
}

if (repoPaths.Count == 0)
{
    Console.WriteLine("No valid repositories. Exiting.");
    return;
}

// 3. Resolve output folder (Docusaurus docs)
string repoRoot = FindRepoRoot() ?? Directory.GetCurrentDirectory();
string outputDir = Path.Combine(repoRoot, config["Output:DocsFolder"] ?? "docs-site/docs");

// 4. Analyze  (SCAN_ONLY env var = detection report without calling the AI)
bool scanOnly = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SCAN_ONLY"));
Console.WriteLine($"\n▶ Analyzing '{appName}' ({repoPaths.Count} repositories) — {engine} extractor{(scanOnly ? ", SCAN-ONLY" : "")}...\n");

ApplicationModel app = scanOnly
    ? orchestrator.BuildModel(appName, repoPaths, log: msg => Console.WriteLine("  " + msg))
    : await orchestrator.GenerateAsync(appName, repoPaths, outputDir, log: msg => Console.WriteLine("  " + msg));

PrintReport(app);
if (!scanOnly)
{
    Console.WriteLine($"\n   Docs written : {outputDir}");
    Console.WriteLine("   Next: run the Docusaurus site to browse the documentation.");
}

static void PrintReport(ApplicationModel app)
{
    Console.WriteLine("\n=== Detection Report ===");
    foreach (RepositoryModel repo in app.Repositories)
    {
        Console.WriteLine($"📦 {repo.Name}");
        foreach (ProjectModel p in repo.Projects)
        {
            Console.WriteLine($"   └─ {p.Kind} : {p.Name} [{p.TargetFramework ?? "-"}]");
            if (p.Controllers.Count > 0)
                Console.WriteLine($"        controllers: {p.Controllers.Count} ({p.Controllers.Sum(c => c.Actions.Count)} endpoints)");
            if (p.Services.Count > 0)   Console.WriteLine($"        services:    {p.Services.Count}");
            if (p.Entities.Count > 0)   Console.WriteLine($"        entities:    {string.Join(", ", p.Entities.Take(12))}{(p.Entities.Count > 12 ? " …" : "")}");
            if (p.DbContexts.Count > 0) Console.WriteLine($"        dbcontexts:  {string.Join(", ", p.DbContexts)}");
            if (p.CqrsRequests.Count > 0) Console.WriteLine($"        cqrs:        {p.CqrsRequests.Count} commands/queries");
            if (p.Angular is not null)  Console.WriteLine($"        angular:     {p.Angular.Components.Count} components, {p.Angular.Routes.Count} routes");
        }
    }
    Console.WriteLine($"\nTechnologies: {string.Join(", ", app.Technologies)}");
    Console.WriteLine("Capabilities:");
    foreach (IGrouping<string, DetectedCapability> g in app.Capabilities.GroupBy(c => c.Category))
        Console.WriteLine($"   {g.Key}: {string.Join(", ", g.Select(c => c.Name).Distinct())}");
}

// --- helpers ---
static string ReadNonEmpty(string fallback)
{
    string? v = Console.ReadLine();
    return string.IsNullOrWhiteSpace(v) ? fallback : v.Trim();
}

static string Expand(string path) =>
    path.StartsWith("~")
        ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), path[1..].TrimStart('/', '\\'))
        : path;

static string? FindRepoRoot()
{
    var dir = new DirectoryInfo(AppContext.BaseDirectory);
    while (dir is not null)
    {
        if (dir.GetFiles("DocPlatform.sln").Length > 0) return dir.FullName;
        dir = dir.Parent;
    }
    return null;
}
