using System.Text.Json;
using System.Text.Json.Serialization;
using Azure;
using Azure.AI.Inference;
using DocPlatform.AI.Prompts;
using DocPlatform.Core.Abstractions;
using DocPlatform.Core.Models;

namespace DocPlatform.AI;

// POC implementation of IAIProvider using GitHub Models (free tier) via the
// Azure AI Inference SDK. Swappable for AzureAIFoundryProvider/OpenAIProvider later —
// the Documentation Engine only ever sees IAIProvider.
public class GitHubModelsProvider : IAIProvider
{
    private readonly ChatCompletionsClient _client;
    private readonly string _model;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        Converters = { new JsonStringEnumConverter() }   // so the AI sees "DotNetApi", not 1
    };

    public GitHubModelsProvider(string endpoint, string model, string apiKey)
    {
        _client = new ChatCompletionsClient(
            new Uri(endpoint),
            new AzureKeyCredential(apiKey),
            new AzureAIInferenceClientOptions());
        _model = model;
    }

    public async Task<DocumentationResult> GenerateDocumentationAsync(
        ApplicationModel application,
        CancellationToken cancellationToken = default)
    {
        var result = new DocumentationResult();
        string metadataJson = JsonSerializer.Serialize(application, JsonOptions);

        foreach (DocSpec spec in DocumentationPrompts.Plan(application.Name, metadataJson))
        {
            string markdown = await CompleteAsync(DocumentationPrompts.System, spec.Instructions, cancellationToken);
            result.Add(new GeneratedDocument
            {
                Group = spec.Group,
                FileName = spec.FileName,
                Markdown = markdown,
                Order = spec.Order
            });
        }

        return result;
    }

    private async Task<string> CompleteAsync(string system, string user, CancellationToken ct)
    {
        var options = new ChatCompletionsOptions
        {
            Model = _model,
            Messages =
            {
                new ChatRequestSystemMessage(system),
                new ChatRequestUserMessage(user)
            }
        };

        Response<ChatCompletions> response = await _client.CompleteAsync(options, ct);
        return response.Value.Content;
    }
}
