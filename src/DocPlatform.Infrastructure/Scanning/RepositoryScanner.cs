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
            repo.Projects.Add(ParseDotNetProject(csproj, repositoryPath));

        // ---- Angular projects (package.json that depends on @angular/core) ----
        // Non-Angular package.json files are recorded as unsupported (React/Vue/Node/...).
        foreach (string pkg in EnumerateFiles(repositoryPath)
                     .Where(f => Path.GetFileName(f).Equals("package.json", StringComparison.OrdinalIgnoreCase)))
        {
            ProjectModel? angular = TryParseAngularProject(pkg);
            if (angular is not null) repo.Projects.Add(angular);
            else repo.SkippedProjects.Add(new SkippedProject { Name = ProjectFolderName(pkg), Type = ClassifyJavaScript(pkg), Path = pkg });
        }

        // ---- Other unsupported ecosystems (Python / Java / Go / Ruby / PHP) ----
        foreach (string file in EnumerateFiles(repositoryPath))
        {
            string? type = Path.GetFileName(file).ToLowerInvariant() switch
            {
                "requirements.txt" or "pyproject.toml" or "setup.py" or "pipfile" => "Python",
                "pom.xml" => "Java (Maven)",
                "build.gradle" or "build.gradle.kts" => "Java/Kotlin (Gradle)",
                "go.mod" => "Go",
                "gemfile" => "Ruby",
                "composer.json" => "PHP",
                _ => null
            };
            if (type is not null)
                repo.SkippedProjects.Add(new SkippedProject { Name = ProjectFolderName(file), Type = type, Path = file });
        }

        // de-duplicate skipped entries (a project may have several marker files)
        repo.SkippedProjects = repo.SkippedProjects
            .GroupBy(s => $"{s.Name}|{s.Type}", StringComparer.OrdinalIgnoreCase)
            .Select(g => g.First())
            .ToList();

        return repo;
    }

    private static string ProjectFolderName(string filePath) =>
        Path.GetFileName(Path.GetDirectoryName(filePath)!) is { Length: > 0 } n ? n : Path.GetFileName(filePath);

    // Classify a non-Angular package.json by its dependencies.
    private static string ClassifyJavaScript(string packageJsonPath)
    {
        try
        {
            using JsonDocument doc = JsonDocument.Parse(File.ReadAllText(packageJsonPath));
            JsonElement root = doc.RootElement;
            bool Dep(string name) => HasDependency(root, "dependencies", name) || HasDependency(root, "devDependencies", name);

            if (Dep("next")) return "Next.js (React)";
            if (Dep("react") || Dep("react-dom")) return "React";
            if (Dep("vue")) return "Vue";
            if (Dep("svelte")) return "Svelte";
            if (Dep("@nestjs/core")) return "NestJS (Node)";
            if (Dep("express") || Dep("fastify") || Dep("koa")) return "Node.js (backend)";
        }
        catch { /* fall through */ }
        return "Node.js / JavaScript";
    }

    // -- .NET -----------------------------------------------------------------
    private static ProjectModel ParseDotNetProject(string csprojPath, string repositoryRoot)
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

            project.TargetFramework = doc.Descendants("TargetFramework").FirstOrDefault()?.Value
                ?? doc.Descendants("TargetFrameworks").FirstOrDefault()?.Value
                // Fall back to a central Directory.Build.props (common in real repos).
                ?? FindTfmInBuildProps(Path.GetDirectoryName(csprojPath)!, repositoryRoot);

            project.PackageReferences = doc.Descendants("PackageReference")
                .Select(e => e.Attribute("Include")?.Value)
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .Select(v => v!)
                .ToList();

            project.ProjectReferences = doc.Descendants("ProjectReference")
                // Normalize Windows backslashes so this works cross-platform (macOS/Linux).
                .Select(e => Path.GetFileNameWithoutExtension((e.Attribute("Include")?.Value ?? string.Empty).Replace('\\', '/')))
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
    // Walks up from the project folder (bounded by the repo root) looking for a
    // Directory.Build.props that centrally defines the TargetFramework.
    private static string? FindTfmInBuildProps(string startDir, string repositoryRoot)
    {
        string root = Path.GetFullPath(repositoryRoot);
        var dir = new DirectoryInfo(Path.GetFullPath(startDir));
        while (dir is not null && dir.FullName.StartsWith(root, StringComparison.OrdinalIgnoreCase))
        {
            string props = Path.Combine(dir.FullName, "Directory.Build.props");
            if (File.Exists(props))
            {
                try
                {
                    XDocument d = XDocument.Load(props);
                    string? tfm = d.Descendants("TargetFramework").FirstOrDefault()?.Value
                               ?? d.Descendants("TargetFrameworks").FirstOrDefault()?.Value;
                    if (!string.IsNullOrWhiteSpace(tfm)) return tfm;
                }
                catch { /* ignore malformed props */ }
            }
            dir = dir.Parent;
        }
        return null;
    }

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
