namespace DocPlatform.Core.Models;

// One generated Markdown document, tagged with the group it belongs to
// (e.g. "Product Specification") so the output can be organized into sections.
public class GeneratedDocument
{
    public string Group { get; set; } = string.Empty;     // "Product Specification"
    public string FileName { get; set; } = string.Empty;  // "overview.md"
    public string Markdown { get; set; } = string.Empty;
    public int Order { get; set; }                         // position within the group
}

// The full set of generated documents for one application.
public class DocumentationResult
{
    public string ApplicationName { get; set; } = string.Empty;

    public List<GeneratedDocument> Documents { get; } = new();

    public void Add(GeneratedDocument document) => Documents.Add(document);
}
