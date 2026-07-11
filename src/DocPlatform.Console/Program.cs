using DocPlatform.AI;
using DocPlatform.Core.Abstractions;
using DocPlatform.Core.Models;
using DocPlatform.Infrastructure.Extraction;
using DocPlatform.Infrastructure.Scanning;

// TEMPORARY test harness for Step 4 — scan + extract + AI documentation.
// (The real "create app / select repos / Analyze" console comes in Step 6.)

string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
string[] repoPaths =
{
    Path.Combine(home, "lokesh-ram/projects/task-management-app/TaskManagementBackend"),
    Path.Combine(home, "lokesh-ram/projects/task-management-app/TaskManagementFrontend")
};

IRepositoryScanner scanner = new RepositoryScanner();
IMetadataExtractor extractor = new MetadataExtractor();

// Build the ApplicationModel (this aggregation moves into the Orchestrator in Step 6)
var app = new ApplicationModel { Name = "TaskFlow" };
foreach (string path in repoPaths)
{
    RepositoryModel repo = scanner.Scan(path);
    extractor.Extract(repo);
    app.Repositories.Add(repo);
}
app.Technologies = AggregateTechnologies(app);
Console.WriteLine($"ApplicationModel built: {app.Repositories.Count} repos, tech: {string.Join(", ", app.Technologies)}\n");

// --- AI step ---
string? token = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
if (string.IsNullOrWhiteSpace(token))
{
    Console.WriteLine("❌ Set GITHUB_TOKEN to run the AI step.");
    return;
}

IAIProvider ai = new GitHubModelsProvider("https://models.github.ai/inference", "openai/gpt-4o-mini", token);
Console.WriteLine("🤖 Generating documentation...\n");
DocumentationResult docs = await ai.GenerateDocumentationAsync(app);

foreach ((string file, string md) in docs.Files)
{
    Console.WriteLine($"================= {file} =================");
    Console.WriteLine(md.Length > 900 ? md[..900] + "\n… (truncated for preview)" : md);
    Console.WriteLine();
}

static List<string> AggregateTechnologies(ApplicationModel app)
{
    var tech = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    foreach (ProjectModel p in app.Repositories.SelectMany(r => r.Projects))
    {
        if (p.Kind == ProjectKind.DotNetApi) tech.Add("ASP.NET Core");
        if (p.Kind == ProjectKind.Angular) tech.Add("Angular");
        if (p.TargetFramework is not null) tech.Add($".NET ({p.TargetFramework})");
        if (p.HasAuthentication) tech.Add("JWT Authentication");
        if (p.PackageReferences.Any(x => x.Contains("Dapper"))) tech.Add("Dapper");
        if (p.PackageReferences.Any(x => x.Contains("EntityFrameworkCore"))) tech.Add("EF Core");
        if (p.PackageReferences.Any(x => x.Contains("SqlClient"))) tech.Add("SQL Server");
        if (p.PackageReferences.Any(x => x.Contains("SemanticKernel"))) tech.Add("Semantic Kernel (AI)");
    }
    return tech.OrderBy(t => t).ToList();
}
