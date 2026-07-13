using DocPlatform.AI;
using DocPlatform.Application;
using DocPlatform.Core.Abstractions;
using DocPlatform.Core.Models;
using DocPlatform.Infrastructure.Extraction;
using DocPlatform.Infrastructure.Output;
using DocPlatform.Infrastructure.Scanning;
using DocPlatform.Infrastructure.Sourcing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

// ==========================================================================
//  DocPlatform Console
//   - interactive:  dotnet run                    (prompt for app + repos)
//   - headless/CI:  dotnet run -- --config apps.yml
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
string engine = config["Extraction:Engine"] ?? "Roslyn";

var services = new ServiceCollection();
services.AddSingleton<IRepositoryScanner, RepositoryScanner>();
services.AddSingleton<IMetadataExtractor>(_ =>
{
    // .NET projects -> chosen .NET extractor; Angular projects -> Angular extractor.
    IMetadataExtractor dotnet = engine.Equals("Heuristic", StringComparison.OrdinalIgnoreCase)
        ? new HeuristicMetadataExtractor()
        : new RoslynMetadataExtractor();
    return new CompositeMetadataExtractor(dotnet, new AngularMetadataExtractor());
});
services.AddSingleton<IMarkdownWriter, MarkdownWriter>();
services.AddSingleton<IAIProvider>(_ => new GitHubModelsProvider(endpoint, model, token));
services.AddSingleton<DocumentationOrchestrator>();
ServiceProvider provider = services.BuildServiceProvider();

var orchestrator = provider.GetRequiredService<DocumentationOrchestrator>();

string repoRoot = FindRepoRoot() ?? Directory.GetCurrentDirectory();
string outputDir = Path.Combine(repoRoot, config["Output:DocsFolder"] ?? "docs-site/docs");

// --- Headless / CI mode (--config apps.yml) ---
string? configPath = GetArg(args, "--config");
if (configPath is not null)
{
    await RunHeadlessAsync(configPath, orchestrator, outputDir, repoRoot, engine);
    return;
}

// --- Interactive mode ---
Console.WriteLine("========================================");
Console.WriteLine("  DocPlatform — AI Documentation (POC)");
Console.WriteLine("========================================\n");

Console.Write("Application name: ");
string appName = ReadNonEmpty("TaskFlow");

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

// ==========================================================================
//  Helpers
// ==========================================================================
static async Task RunHeadlessAsync(string configPath, DocumentationOrchestrator orchestrator,
    string outputDir, string repoRoot, string engine)
{
    if (!File.Exists(configPath))
    {
        Console.WriteLine($"❌ Config not found: {configPath}");
        return;
    }

    IDeserializer deserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .IgnoreUnmatchedProperties()
        .Build();
    AppsConfig cfg = deserializer.Deserialize<AppsConfig>(File.ReadAllText(configPath)) ?? new AppsConfig();

    string workDir = Path.Combine(repoRoot, ".sources");
    Console.WriteLine($"Headless mode — {cfg.Applications.Count} application(s), {engine} extractor\n");

    foreach (AppEntry appEntry in cfg.Applications)
    {
        Console.WriteLine($"▶ {appEntry.Name}");
        var paths = new List<string>();
        foreach (string repo in appEntry.Repos)
            paths.Add(GitSourceResolver.Resolve(repo, workDir, msg => Console.WriteLine("  " + msg)));

        await orchestrator.GenerateAsync(appEntry.Name, paths, outputDir, msg => Console.WriteLine("  " + msg));
        Console.WriteLine($"  ✅ {appEntry.Name}\n");
    }

    Console.WriteLine($"All documentation written to {outputDir}");
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
            {
                Console.WriteLine($"        controllers: {p.Controllers.Count} ({p.Controllers.Sum(c => c.Actions.Count)} endpoints)");
                foreach (ControllerModel c in p.Controllers.Take(4))
                    Console.WriteLine($"          • {c.Name}: {string.Join(", ", c.Actions.Take(6))}{(c.Actions.Count > 6 ? " …" : "")}");
            }
            if (p.Services.Count > 0)   Console.WriteLine($"        services:    {string.Join(", ", p.Services.Take(14))}{(p.Services.Count > 14 ? " …" : "")}");
            if (p.Entities.Count > 0)   Console.WriteLine($"        entities:    {string.Join(", ", p.Entities.Take(12))}{(p.Entities.Count > 12 ? " …" : "")}");
            if (p.DbContexts.Count > 0) Console.WriteLine($"        dbcontexts:  {string.Join(", ", p.DbContexts)}");
            if (p.CqrsRequests.Count > 0) Console.WriteLine($"        cqrs:        {p.CqrsRequests.Count} commands/queries");
            if (p.Angular is not null)
            {
                Console.WriteLine($"        angular:     {p.Angular.Components.Count} components, {p.Angular.Services.Count} services, {p.Angular.Routes.Count} routes, {p.Angular.Guards.Count} guards, {p.Angular.ApiCalls.Count} api calls");
                if (p.Angular.ApiCalls.Count > 0)
                    Console.WriteLine($"          api calls: {string.Join(", ", p.Angular.ApiCalls.Take(8))}");
                if (p.Angular.Routes.Count > 0)
                    Console.WriteLine($"          routes: {string.Join(", ", p.Angular.Routes.Take(8))}");
            }
        }
    }
    List<SkippedProject> skipped = app.Repositories.SelectMany(r => r.SkippedProjects).ToList();
    if (skipped.Count > 0)
    {
        Console.WriteLine("Skipped (unsupported — logged, not analyzed):");
        foreach (SkippedProject s in skipped)
            Console.WriteLine($"   {s.Name} [{s.Type}]");
    }

    Console.WriteLine($"\nTechnologies: {string.Join(", ", app.Technologies)}");
    Console.WriteLine("Capabilities:");
    foreach (IGrouping<string, DetectedCapability> g in app.Capabilities.GroupBy(c => c.Category))
        Console.WriteLine($"   {g.Key}: {string.Join(", ", g.Select(c => c.Name).Distinct())}");

    List<ProjectModel> all = app.Repositories.SelectMany(r => r.Projects).ToList();
    List<string> schemes = all.SelectMany(p => p.AuthSchemes).Distinct().ToList();
    List<string> policies = all.SelectMany(p => p.AuthPolicies).Distinct().ToList();
    List<string> roles = all.SelectMany(p => p.AuthRoles).Distinct().ToList();
    if (schemes.Count > 0 || policies.Count > 0 || roles.Count > 0)
    {
        Console.WriteLine("Auth:");
        if (schemes.Count > 0)  Console.WriteLine($"   schemes:  {string.Join(", ", schemes)}");
        if (policies.Count > 0) Console.WriteLine($"   policies: {string.Join(", ", policies)}");
        if (roles.Count > 0)    Console.WriteLine($"   roles:    {string.Join(", ", roles)}");
    }

    if (app.ArchitecturePatterns.Count > 0)
    {
        Console.WriteLine("Architecture patterns:");
        foreach (DetectedPattern p in app.ArchitecturePatterns)
            Console.WriteLine($"   • {p.Name} — {p.Evidence}");
    }

    if (app.CallGraph.Count > 0)
    {
        Console.WriteLine($"Call graph ({app.CallGraph.Count} edges):");
        foreach (Relationship e in app.CallGraph.Take(15))
            Console.WriteLine($"   {e.From} → {e.To}");
    }
}

static string? GetArg(string[] args, string name)
{
    int i = Array.IndexOf(args, name);
    return i >= 0 && i + 1 < args.Length ? args[i + 1] : null;
}

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
        if (dir.GetFiles("*.sln").Length > 0 || dir.GetFiles("*.slnx").Length > 0 || dir.GetFiles("apps.yml").Length > 0)
            return dir.FullName;
        dir = dir.Parent;
    }
    return null;
}

// Config model for apps.yml
class AppsConfig
{
    public List<AppEntry> Applications { get; set; } = new();
}

class AppEntry
{
    public string Name { get; set; } = string.Empty;
    public List<string> Repos { get; set; } = new();
}
