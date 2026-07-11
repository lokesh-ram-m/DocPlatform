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

    public async Task<ApplicationModel> GenerateAsync(
        string applicationName,
        IEnumerable<string> repositoryPaths,
        string outputDirectory,
        Action<string>? log = null,
        CancellationToken cancellationToken = default)
    {
        log ??= _ => { };

        // 1-3. Deterministic: scan + extract into the ApplicationModel.
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

        // 4. AI: explain the metadata as Markdown.
        log("Generating documentation with the AI provider ...");
        DocumentationResult docs = await _aiProvider.GenerateDocumentationAsync(application, cancellationToken);

        // 5. Write Markdown into the Docusaurus docs folder.
        log($"Writing {docs.Documents.Count} document(s) to {outputDirectory}");
        _writer.Write(docs, outputDirectory);

        return application;
    }
}
