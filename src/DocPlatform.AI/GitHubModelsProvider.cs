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
        var result = new DocumentationResult { ApplicationName = application.Name };
        // Compact projection so large apps (many projects) fit the model's token budget.
        // Drops PackageReferences/ProjectReferences (covered by Capabilities/Relationships)
        // and caps long lists.
        string metadataJson = JsonSerializer.Serialize(ToCompact(application), JsonOptions);

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

    // A trimmed view of the model for the prompt — keeps what the AI needs, drops the rest.
    private static object ToCompact(ApplicationModel app) => new
    {
        name = app.Name,
        technologies = app.Technologies,
        capabilities = app.Capabilities.Select(c => new { c.Category, c.Name }),
        architecturePatterns = app.ArchitecturePatterns.Select(p => new { p.Name, p.Evidence }),
        relationships = app.Relationships.Select(r => new { r.From, r.To, r.Type }),
        callGraph = app.CallGraph.Take(60).Select(r => new { r.From, r.To }),
        repositories = app.Repositories.Select(repo => new
        {
            name = repo.Name,
            projects = repo.Projects.Select(p => new
            {
                name = p.Name,
                kind = p.Kind.ToString(),
                targetFramework = p.TargetFramework,
                hasAuthentication = p.HasAuthentication,
                controllers = p.Controllers.Select(c => new { c.Name, c.Route, actions = Cap(c.Actions, 20) }),
                services = Cap(p.Services, 15),
                interfaces = Cap(p.Interfaces, 15),
                entities = Cap(p.Entities, 20),
                dbContexts = p.DbContexts,
                cqrsRequests = Cap(p.CqrsRequests, 20),
                angular = p.Angular is null ? null : new
                {
                    components = Cap(p.Angular.Components, 25),
                    routes = Cap(p.Angular.Routes, 25)
                }
            })
        })
    };

    // Cap a list, appending a "+N more" marker so the AI knows it's truncated.
    private static List<string> Cap(List<string> list, int max) =>
        list.Count <= max ? list : list.Take(max).Append($"…(+{list.Count - max} more)").ToList();
}
