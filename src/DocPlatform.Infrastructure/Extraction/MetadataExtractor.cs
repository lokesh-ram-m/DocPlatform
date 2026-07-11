using System.Text.RegularExpressions;
using DocPlatform.Core.Abstractions;
using DocPlatform.Core.Models;

namespace DocPlatform.Infrastructure.Extraction;

// Heuristic, deterministic code parsing (regex + file conventions — not full Roslyn,
// which is enough for the POC). Fills each ProjectModel with real facts.
public class MetadataExtractor : IMetadataExtractor
{
    private static readonly string[] IgnoredDirs = { "bin", "obj", "node_modules", ".git", "dist", ".angular" };

    public void Extract(RepositoryModel repository)
    {
        foreach (ProjectModel project in repository.Projects)
        {
            switch (project.Kind)
            {
                case ProjectKind.DotNetApi:
                case ProjectKind.DotNetLibrary:
                case ProjectKind.DotNetOther:
                    ExtractDotNet(project);
                    break;
                case ProjectKind.Angular:
                    ExtractAngular(project);
                    break;
            }
        }
    }

    // -- .NET -----------------------------------------------------------------
    private static void ExtractDotNet(ProjectModel project)
    {
        string projectDir = Path.GetDirectoryName(project.Path)!;

        // Auth hint from packages up front.
        if (project.PackageReferences.Any(p => p.Contains("Authentication", StringComparison.OrdinalIgnoreCase)))
            project.HasAuthentication = true;

        foreach (string file in EnumerateFiles(projectDir).Where(f => f.EndsWith(".cs")))
        {
            string code;
            try { code = File.ReadAllText(file); } catch { continue; }

            string fileName = Path.GetFileNameWithoutExtension(file);
            string relDir = Path.GetDirectoryName(file)!.Replace(projectDir, "");

            // Authorization usage anywhere -> authentication in play
            if (code.Contains("[Authorize]") || code.Contains("AddAuthentication"))
                project.HasAuthentication = true;

            // Controllers
            foreach (Match m in Regex.Matches(code, @"class\s+(\w+Controller)\b"))
            {
                var controller = new ControllerModel { Name = m.Groups[1].Value };

                Match route = Regex.Match(code, @"\[Route\(""([^""]+)""\)\]");
                if (route.Success) controller.Route = route.Groups[1].Value;

                foreach (Match action in Regex.Matches(code,
                    @"\[Http(Get|Post|Put|Delete|Patch)[^\]]*\][\s\S]{0,160}?public\s+[\w<>\[\]?\s]+?\s+(\w+)\s*\("))
                {
                    controller.Actions.Add($"{action.Groups[1].Value.ToUpper()} {action.Groups[2].Value}");
                }
                project.Controllers.Add(controller);
            }

            // DbContexts
            foreach (Match m in Regex.Matches(code, @"class\s+(\w+)\s*:\s*[\w<>,\s]*\bDbContext\b"))
                project.DbContexts.Add(m.Groups[1].Value);

            // Interfaces (I-prefixed)
            foreach (Match m in Regex.Matches(code, @"\binterface\s+(I\w+)\b"))
                project.Interfaces.Add(m.Groups[1].Value);

            // Services (classes/interfaces ending in Service)
            foreach (Match m in Regex.Matches(code, @"(?:class|interface)\s+(\w*Service)\b"))
                project.Services.Add(m.Groups[1].Value);

            // Entities (classes living under Models/Entities/Domain folders)
            bool inModelsFolder = Regex.IsMatch(relDir, @"[/\\](Models|Entities|Domain)([/\\]|$)", RegexOptions.IgnoreCase);
            if (inModelsFolder)
                foreach (Match m in Regex.Matches(code, @"class\s+(\w+)\b"))
                    project.Entities.Add(m.Groups[1].Value);
        }

        Dedupe(project.Services);
        Dedupe(project.Interfaces);
        Dedupe(project.Entities);
        Dedupe(project.DbContexts);
    }

    // -- Angular --------------------------------------------------------------
    private static void ExtractAngular(ProjectModel project)
    {
        project.Angular ??= new AngularInfo();
        string projectDir = Path.GetDirectoryName(project.Path)!;

        foreach (string file in EnumerateFiles(projectDir))
        {
            string name = Path.GetFileName(file);

            // Components: *.component.ts
            if (name.EndsWith(".component.ts", StringComparison.OrdinalIgnoreCase))
                project.Angular.Components.Add(name.Replace(".component.ts", "", StringComparison.OrdinalIgnoreCase));

            // Routes: any *routes*.ts — pull the path: '...' values
            if (name.Contains("routes", StringComparison.OrdinalIgnoreCase) && name.EndsWith(".ts"))
            {
                string code;
                try { code = File.ReadAllText(file); } catch { continue; }
                foreach (Match m in Regex.Matches(code, @"path:\s*['""]([^'""]*)['""]"))
                {
                    string p = string.IsNullOrEmpty(m.Groups[1].Value) ? "(root)" : m.Groups[1].Value;
                    project.Angular.Routes.Add(p);
                }
            }
        }

        Dedupe(project.Angular.Components);
        Dedupe(project.Angular.Routes);
    }

    // -- helpers --------------------------------------------------------------
    private static void Dedupe(List<string> list)
    {
        var seen = new HashSet<string>(list, StringComparer.OrdinalIgnoreCase);
        list.Clear();
        list.AddRange(seen);
        list.Sort(StringComparer.OrdinalIgnoreCase);
    }

    private static IEnumerable<string> EnumerateFiles(string root)
    {
        var stack = new Stack<string>();
        stack.Push(root);
        while (stack.Count > 0)
        {
            string dir = stack.Pop();
            string[] subDirs;
            try { subDirs = Directory.GetDirectories(dir); } catch { continue; }
            foreach (string sub in subDirs)
                if (!IgnoredDirs.Contains(Path.GetFileName(sub), StringComparer.OrdinalIgnoreCase))
                    stack.Push(sub);

            string[] files;
            try { files = Directory.GetFiles(dir); } catch { continue; }
            foreach (string f in files) yield return f;
        }
    }
}
