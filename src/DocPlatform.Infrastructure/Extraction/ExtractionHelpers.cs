using System.Text.RegularExpressions;
using DocPlatform.Core.Models;

namespace DocPlatform.Infrastructure.Extraction;

// Shared bits used by BOTH metadata extractors (heuristic + Roslyn):
//   - file walking that skips bin/obj/node_modules
//   - Angular parsing (Roslyn is C#-only, so Angular is handled the same way for both)
internal static class ExtractionHelpers
{
    private static readonly string[] IgnoredDirs = { "bin", "obj", "node_modules", ".git", "dist", ".angular" };

    public static IEnumerable<string> EnumerateFiles(string root)
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

    public static void ExtractAngular(ProjectModel project)
    {
        project.Angular ??= new AngularInfo();
        string projectDir = Path.GetDirectoryName(project.Path)!;

        foreach (string file in EnumerateFiles(projectDir))
        {
            string name = Path.GetFileName(file);

            if (name.EndsWith(".component.ts", StringComparison.OrdinalIgnoreCase))
                project.Angular.Components.Add(name.Replace(".component.ts", "", StringComparison.OrdinalIgnoreCase));

            if (!name.EndsWith(".ts", StringComparison.OrdinalIgnoreCase) ||
                name.EndsWith(".spec.ts", StringComparison.OrdinalIgnoreCase))
                continue;

            string code;
            try { code = File.ReadAllText(file); } catch { continue; }

            // A route source = filename hints OR any Angular routing construct in the file.
            bool routeSource =
                name.Contains("routes", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("routing", StringComparison.OrdinalIgnoreCase) ||
                code.Contains("RouterModule") ||
                code.Contains("provideRouter") ||
                Regex.IsMatch(code, @":\s*Routes\b");
            if (!routeSource) continue;

            foreach (Match m in Regex.Matches(code, @"path:\s*['""]([^'""]*)['""]"))
                project.Angular.Routes.Add(string.IsNullOrEmpty(m.Groups[1].Value) ? "(root)" : m.Groups[1].Value);
        }

        Dedupe(project.Angular.Components);
        Dedupe(project.Angular.Routes);
    }

    // Entity precision: filter out DTOs, responses, base classes, etc. so only
    // real domain entities land in the model.
    private static readonly string[] NonEntitySuffixes =
        { "Dto", "Response", "Request", "Result", "ViewModel", "Vm", "Mapping",
          "Options", "Settings", "Configuration", "Command", "Query", "Handler", "Validator" };

    public static bool IsLikelyEntity(string name)
    {
        if (string.Equals(name, "BaseEntity", StringComparison.OrdinalIgnoreCase)) return false;
        return !NonEntitySuffixes.Any(s => name.EndsWith(s, StringComparison.Ordinal));
    }

    public static void Dedupe(List<string> list)
    {
        var seen = new HashSet<string>(list, StringComparer.OrdinalIgnoreCase);
        list.Clear();
        list.AddRange(seen);
        list.Sort(StringComparer.OrdinalIgnoreCase);
    }
}
