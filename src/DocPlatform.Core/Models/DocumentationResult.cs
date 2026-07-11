namespace DocPlatform.Core.Models;

// The output of the AI: a set of Markdown documents keyed by filename
// (e.g. "overview.md" -> "# Overview ...").
public class DocumentationResult
{
    public Dictionary<string, string> Files { get; } = new();

    public void Add(string fileName, string markdown) => Files[fileName] = markdown;
}
