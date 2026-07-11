using DocPlatform.Core.Models;

namespace DocPlatform.Core.Abstractions;

// Writes the generated Markdown documents into an output directory
// (the Docusaurus docs folder).
public interface IMarkdownWriter
{
    void Write(DocumentationResult documentation, string outputDirectory);
}
