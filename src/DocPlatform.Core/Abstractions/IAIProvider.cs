using DocPlatform.Core.Models;

namespace DocPlatform.Core.Abstractions;

// The ONLY thing the Documentation Engine knows about the AI.
// Swap GitHubModelsProvider -> AzureAIFoundryProvider -> OpenAIProvider later
// without the engine ever changing. (Rule #2 and #3.)
public interface IAIProvider
{
    Task<DocumentationResult> GenerateDocumentationAsync(
        ApplicationModel application,
        CancellationToken cancellationToken = default);
}
