namespace DocPlatform.Infrastructure.Extraction;

// Shared bits used across the extractors: file walking (skips bin/obj/node_modules),
// the entity-name filter, and list de-duplication.
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
