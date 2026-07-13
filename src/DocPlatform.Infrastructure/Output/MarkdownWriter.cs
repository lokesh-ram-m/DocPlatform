using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using DocPlatform.Core.Abstractions;
using DocPlatform.Core.Models;

namespace DocPlatform.Infrastructure.Output;

// Writes docs under a per-APPLICATION folder so multiple applications coexist:
//   docs/<app>/<group>/<file>.md  (+ Docusaurus _category_.json at each level)
// and regenerates a homepage (index.md) listing every documented application.
public class MarkdownWriter : IMarkdownWriter
{
    private static readonly Dictionary<string, int> GroupOrder = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Product Specification"] = 1,
        ["Technical Specification"] = 2
    };

    public void Write(DocumentationResult documentation, string outputDirectory)
    {
        Directory.CreateDirectory(outputDirectory);

        string appName = string.IsNullOrWhiteSpace(documentation.ApplicationName) ? "Application" : documentation.ApplicationName;
        string appDir = Path.Combine(outputDirectory, Slugify(appName));

        // Re-write just THIS application's folder (other apps are left untouched).
        if (Directory.Exists(appDir)) Directory.Delete(appDir, recursive: true);
        Directory.CreateDirectory(appDir);
        WriteCategory(appDir, appName, position: null, collapsed: true);

        foreach (IGrouping<string, GeneratedDocument> group in documentation.Documents.GroupBy(d => d.Group))
        {
            string groupDir = Path.Combine(appDir, Slugify(group.Key));
            Directory.CreateDirectory(groupDir);
            int groupPos = GroupOrder.TryGetValue(group.Key, out int gp) ? gp : 99;
            WriteCategory(groupDir, group.Key, groupPos, collapsed: false);

            foreach (GeneratedDocument doc in group)
            {
                string frontMatter = $"---\nsidebar_position: {doc.Order}\n---\n\n";
                File.WriteAllText(Path.Combine(groupDir, doc.FileName), frontMatter + doc.Markdown.Trim() + "\n");
            }
        }

        RegenerateHomepage(outputDirectory);
    }

    // Homepage (slug: /) listing every application folder currently present.
    private static void RegenerateHomepage(string outputDirectory)
    {
        var sb = new StringBuilder();
        sb.AppendLine("---");
        sb.AppendLine("slug: /");
        sb.AppendLine("title: Documented Applications");
        sb.AppendLine("sidebar_position: 0");
        sb.AppendLine("---");
        sb.AppendLine();
        sb.AppendLine("# Documented Applications");
        sb.AppendLine();
        sb.AppendLine("AI-generated Product & Technical specifications for the following applications:");
        sb.AppendLine();

        foreach (string dir in Directory.GetDirectories(outputDirectory).OrderBy(d => d))
        {
            string categoryFile = Path.Combine(dir, "_category_.json");
            if (!File.Exists(categoryFile)) continue;

            string slug = Path.GetFileName(dir);
            string label = ReadCategoryLabel(categoryFile) ?? slug;
            sb.AppendLine($"- **[{label}](/{slug}/product-specification/overview)** — Product & Technical specs");
        }

        File.WriteAllText(Path.Combine(outputDirectory, "index.md"), sb.ToString());
    }

    private static void WriteCategory(string dir, string label, int? position, bool collapsed)
    {
        var sb = new StringBuilder();
        sb.AppendLine("{");
        sb.AppendLine($"  \"label\": \"{label}\",");
        if (position is not null) sb.AppendLine($"  \"position\": {position},");
        sb.AppendLine($"  \"collapsed\": {collapsed.ToString().ToLowerInvariant()}");
        sb.AppendLine("}");
        File.WriteAllText(Path.Combine(dir, "_category_.json"), sb.ToString());
    }

    private static string? ReadCategoryLabel(string categoryFile)
    {
        try
        {
            using JsonDocument doc = JsonDocument.Parse(File.ReadAllText(categoryFile));
            return doc.RootElement.TryGetProperty("label", out JsonElement l) ? l.GetString() : null;
        }
        catch { return null; }
    }

    private static string Slugify(string value)
    {
        string s = value.ToLowerInvariant().Trim();
        s = Regex.Replace(s, @"[^a-z0-9]+", "-");
        return s.Trim('-');
    }
}
