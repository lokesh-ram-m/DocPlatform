using DocPlatform.Core.Abstractions;
using DocPlatform.Core.Models;

namespace DocPlatform.Application;

// The heart of the pipeline. Coordinates the whole flow but knows NOTHING about
// concrete implementations (which scanner, which LLM) — only the Core abstractions.
//    scan -> extract -> build ApplicationModel -> AI -> write Markdown
public class DocumentationOrchestrator
{
    private readonly IRepositoryScanner _scanner;
    private readonly IMetadataExtractor _extractor;
    private readonly IAIProvider _aiProvider;
    private readonly IMarkdownWriter _writer;

    public DocumentationOrchestrator(
        IRepositoryScanner scanner,
        IMetadataExtractor extractor,
        IAIProvider aiProvider,
        IMarkdownWriter writer)
    {
        _scanner = scanner;
        _extractor = extractor;
        _aiProvider = aiProvider;
        _writer = writer;
    }

    // Deterministic only: scan + extract + classify into the ApplicationModel (no AI).
    public ApplicationModel BuildModel(
        string applicationName,
        IEnumerable<string> repositoryPaths,
        Action<string>? log = null)
    {
        log ??= _ => { };

        var application = new ApplicationModel { Name = applicationName };
        foreach (string path in repositoryPaths)
        {
            log($"Scanning {path} ...");
            RepositoryModel repo = _scanner.Scan(path);
            _extractor.Extract(repo);
            application.Repositories.Add(repo);
            log($"  found {repo.Projects.Count} project(s)");
        }
        application.Technologies = TechnologyAggregator.From(application);
        application.Capabilities = CapabilityClassifier.Classify(application);
        application.Relationships = GraphBuilder.Build(application);   // knowledge graph
        application.CallGraph = application.Repositories               // component call graph
            .SelectMany(r => r.Projects)
            .SelectMany(p => p.ComponentDependencies)
            .GroupBy(r => $"{r.From}|{r.To}", StringComparer.Ordinal)
            .Select(g => g.First())
            .ToList();
        application.ArchitecturePatterns = ArchitectureDetector.Detect(application);
        return application;
    }

    public async Task<ApplicationModel> GenerateAsync(
        string applicationName,
        IEnumerable<string> repositoryPaths,
        string outputDirectory,
        Action<string>? log = null,
        CancellationToken cancellationToken = default)
    {
        log ??= _ => { };

        // 1-3. Deterministic: scan + extract + classify.
        ApplicationModel application = BuildModel(applicationName, repositoryPaths, log);

        // 4. AI: explain the metadata (incl. the knowledge graph) as Markdown.
        log("Generating documentation with the AI provider ...");
        DocumentationResult docs = await _aiProvider.GenerateDocumentationAsync(application, cancellationToken);

        // 4b. Inject the deterministic knowledge-graph diagram into architecture.md.
        InjectDiagram(docs, application);

        // 5. Write Markdown into the Docusaurus docs folder.
        log($"Writing {docs.Documents.Count} document(s) to {outputDirectory}");
        _writer.Write(docs, outputDirectory);

        return application;
    }

    private static void InjectDiagram(DocumentationResult docs, ApplicationModel application)
    {
        string systemDiagram = DiagramGenerator.ToMermaid(application);
        string componentDiagram = DiagramGenerator.ToComponentMermaid(application);
        if (string.IsNullOrEmpty(systemDiagram) && string.IsNullOrEmpty(componentDiagram)) return;

        GeneratedDocument? arch = docs.Documents.FirstOrDefault(d =>
            d.FileName.Equals("architecture.md", StringComparison.OrdinalIgnoreCase));
        if (arch is null) return;

        var section = new System.Text.StringBuilder();
        if (!string.IsNullOrEmpty(systemDiagram))
            section.Append("## System Diagram\n\n")
                   .Append("_Generated from the application's knowledge graph (project references, calls, persistence)._\n\n")
                   .Append(systemDiagram).Append('\n');
        if (!string.IsNullOrEmpty(componentDiagram))
            section.Append("\n## Component Call Graph\n\n")
                   .Append("_How components wire up (controller → service → repository → data), from constructor injection._\n\n")
                   .Append(componentDiagram).Append('\n');

        // Insert the diagram right after the first heading line.
        string[] lines = arch.Markdown.Split('\n');
        int headingIndex = Array.FindIndex(lines, l => l.TrimStart().StartsWith("#"));
        if (headingIndex >= 0)
            arch.Markdown = string.Join('\n', lines[..(headingIndex + 1)]) + "\n\n" + section + "\n" +
                            string.Join('\n', lines[(headingIndex + 1)..]);
        else
            arch.Markdown = section + "\n" + arch.Markdown;
    }
}
