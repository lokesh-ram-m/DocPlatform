using System.Text.Json;
using System.Xml.Linq;
using DocPlatform.Core.Abstractions;
using DocPlatform.Core.Models;

namespace DocPlatform.Infrastructure.Scanning;

// Deterministic repository discovery — reads the local working copy only.
// Finds .NET projects (.csproj) and Angular projects (package.json with @angular/core).
public class RepositoryScanner : IRepositoryScanner
{
    private static readonly string[] IgnoredDirs = { "bin", "obj", "node_modules", ".git", "dist", ".angular" };

    public RepositoryModel Scan(string repositoryPath)
    {
        if (!Directory.Exists(repositoryPath))
            throw new DirectoryNotFoundException($"Repository path not found: {repositoryPath}");

        var repo = new RepositoryModel
        {
            Name = new DirectoryInfo(repositoryPath.TrimEnd('/', '\\')).Name,
            Path = repositoryPath,
            HasReadme = EnumerateFiles(repositoryPath).Any(f =>
                Path.GetFileName(f).StartsWith("README", StringComparison.OrdinalIgnoreCase))
        };

        // ---- .NET projects (.csproj) ----
        foreach (string csproj in EnumerateFiles(repositoryPath).Where(f => f.EndsWith(".csproj")))
            repo.Projects.Add(ParseDotNetProject(csproj));

        // ---- Angular projects (package.json that depends on @angular/core) ----
        foreach (string pkg in EnumerateFiles(repositoryPath)
                     .Where(f => Path.GetFileName(f).Equals("package.json", StringComparison.OrdinalIgnoreCase)))
        {
            ProjectModel? angular = TryParseAngularProject(pkg);
            if (angular is not null) repo.Projects.Add(angular);
        }

        return repo;
    }

    // -- .NET -----------------------------------------------------------------
    private static ProjectModel ParseDotNetProject(string csprojPath)
    {
        var project = new ProjectModel
        {
            Name = Path.GetFileNameWithoutExtension(csprojPath),
            Path = csprojPath
        };

        try
        {
            XDocument doc = XDocument.Load(csprojPath);
            string sdk = doc.Root?.Attribute("Sdk")?.Value ?? string.Empty;
            string? outputType = doc.Descendants("OutputType").FirstOrDefault()?.Value;

            project.TargetFramework = doc.Descendants("TargetFramework").FirstOrDefault()?.Value;

            project.PackageReferences = doc.Descendants("PackageReference")
                .Select(e => e.Attribute("Include")?.Value)
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .Select(v => v!)
                .ToList();

            project.ProjectReferences = doc.Descendants("ProjectReference")
                .Select(e => Path.GetFileNameWithoutExtension(e.Attribute("Include")?.Value ?? string.Empty))
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .ToList();

            project.Kind = sdk.Contains("Web", StringComparison.OrdinalIgnoreCase)
                ? ProjectKind.DotNetApi
                : string.Equals(outputType, "Exe", StringComparison.OrdinalIgnoreCase)
                    ? ProjectKind.DotNetOther
                    : ProjectKind.DotNetLibrary;
        }
        catch
        {
            project.Kind = ProjectKind.Unknown;
        }

        return project;
    }

    // -- Angular --------------------------------------------------------------
    private static ProjectModel? TryParseAngularProject(string packageJsonPath)
    {
        try
        {
            using JsonDocument doc = JsonDocument.Parse(File.ReadAllText(packageJsonPath));
            JsonElement root = doc.RootElement;

            bool isAngular = HasDependency(root, "dependencies", "@angular/core")
                          || HasDependency(root, "devDependencies", "@angular/core");
            if (!isAngular) return null;

            string name = root.TryGetProperty("name", out JsonElement n) && n.ValueKind == JsonValueKind.String
                ? n.GetString()!
                : Path.GetFileName(Path.GetDirectoryName(packageJsonPath)!);

            var project = new ProjectModel
            {
                Name = name,
                Path = packageJsonPath,
                Kind = ProjectKind.Angular,
                Angular = new AngularInfo
                {
                    Dependencies = ReadDependencyNames(root, "dependencies")
                }
            };
            return project;
        }
        catch
        {
            return null;
        }
    }

    private static bool HasDependency(JsonElement root, string section, string name) =>
        root.TryGetProperty(section, out JsonElement deps)
        && deps.ValueKind == JsonValueKind.Object
        && deps.TryGetProperty(name, out _);

    private static List<string> ReadDependencyNames(JsonElement root, string section) =>
        root.TryGetProperty(section, out JsonElement deps) && deps.ValueKind == JsonValueKind.Object
            ? deps.EnumerateObject().Select(p => p.Name).ToList()
            : new List<string>();

    // -- File walking (skips bin/obj/node_modules/.git) -----------------------
    private static IEnumerable<string> EnumerateFiles(string root)
    {
        var stack = new Stack<string>();
        stack.Push(root);
        while (stack.Count > 0)
        {
            string dir = stack.Pop();
            string[] subDirs;
            try { subDirs = Directory.GetDirectories(dir); }
            catch { continue; }

            foreach (string sub in subDirs)
            {
                string name = Path.GetFileName(sub);
                if (!IgnoredDirs.Contains(name, StringComparer.OrdinalIgnoreCase))
                    stack.Push(sub);
            }

            string[] files;
            try { files = Directory.GetFiles(dir); }
            catch { continue; }

            foreach (string f in files) yield return f;
        }
    }
}
