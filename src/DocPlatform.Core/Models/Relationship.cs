namespace DocPlatform.Core.Models;

// One edge in the application's knowledge graph.
// e.g. From="Web", To="Application", Type="depends on".
public class Relationship
{
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;   // "depends on" | "calls" | "persists to"
}
