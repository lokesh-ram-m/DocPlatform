using System.Text;
using DocPlatform.Core.Models;

namespace DocPlatform.Application;

// Renders the knowledge graph as a Mermaid flowchart (deterministic — drawn from
// the real relationships, so it can never disagree with the code).
public static class DiagramGenerator
{
    public static string ToMermaid(ApplicationModel app)
    {
        if (app.Relationships.Count == 0) return string.Empty;

        var databaseNodes = new HashSet<string>(
            app.Relationships.Where(r => r.Type == "persists to").Select(r => r.To),
            StringComparer.OrdinalIgnoreCase);

        var ids = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        int counter = 0;
        string IdFor(string name)
        {
            if (!ids.TryGetValue(name, out string? id)) { id = "n" + counter++; ids[name] = id; }
            return id;
        }

        var sb = new StringBuilder();
        sb.AppendLine("```mermaid");
        sb.AppendLine("graph LR");

        // node declarations (databases drawn as cylinders)
        foreach (string name in app.Relationships.SelectMany(r => new[] { r.From, r.To })
                     .Distinct(StringComparer.OrdinalIgnoreCase))
        {
            string id = IdFor(name);
            sb.AppendLine(databaseNodes.Contains(name) ? $"  {id}[(\"{name}\")]" : $"  {id}[\"{name}\"]");
        }

        // edges
        foreach (Relationship r in app.Relationships)
            sb.AppendLine($"  {IdFor(r.From)} -->|{r.Type}| {IdFor(r.To)}");

        sb.AppendLine("```");
        return sb.ToString();
    }
}
