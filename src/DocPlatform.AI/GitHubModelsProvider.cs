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

        foreach (DocSpec spec in DocumentationPrompts.Plan(application.Name))
        {
            // Per-document metadata — only the slice this doc needs, so large apps stay
            // within the model's token budget.
            string metadataJson = JsonSerializer.Serialize(MetadataFor(application, spec.FileName), JsonOptions);
            string user = spec.Instructions +
                "\n\nAPPLICATION METADATA (the only source of truth):\n```json\n" + metadataJson + "\n```";

            string markdown = await CompleteAsync(DocumentationPrompts.System, user, cancellationToken);
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

    // Builds ONLY the metadata a given document needs — keeps each prompt small.
    private static object MetadataFor(ApplicationModel app, string fileName)
    {
        var capabilities = app.Capabilities.Select(c => new { c.Category, c.Name });
        var projectsLite = AllProjects(app).Select(p => new { p.Name, kind = p.Kind.ToString(), p.TargetFramework });
        var skipped = app.Repositories.SelectMany(r => r.SkippedProjects).Select(s => new { s.Name, s.Type });
        string name = app.Name;

        return fileName switch
        {
            "overview.md" => new
            {
                name, technologies = app.Technologies, capabilities,
                repositories = app.Repositories.Select(r => new { r.Name, projects = r.Projects.Select(p => new { p.Name, kind = p.Kind.ToString() }) }),
                architecturePatterns = app.ArchitecturePatterns.Select(p => p.Name),
                unanalyzedComponents = skipped
            },
            "features.md" => new
            {
                name, capabilities,
                controllers = AllControllers(app).Select(c => c.Name).Distinct(),
                entities = AllProjects(app).SelectMany(p => p.Entities).Distinct().Take(50),
                angular = AngularSummary(app)
            },
            "use-cases.md" => new
            {
                name, capabilities,
                controllers = AllControllers(app).Select(c => c.Name).Distinct()
            },
            "architecture.md" => new
            {
                name, projects = projectsLite,
                relationships = app.Relationships.Select(r => new { r.From, r.To, r.Type }),
                callGraph = app.CallGraph.Take(50).Select(r => new { r.From, r.To }),
                architecturePatterns = app.ArchitecturePatterns.Select(p => new { p.Name, p.Evidence }),
                angular = AngularSummary(app)
            },
            "technology-stack.md" => new { name, technologies = app.Technologies, capabilities, projects = projectsLite },
            "infrastructure-and-cloud.md" => new { name, capabilities, projects = projectsLite, unanalyzedComponents = skipped },
            "data-storage-and-messaging.md" => new
            {
                name, capabilities,
                dbContexts = AllProjects(app).SelectMany(p => p.DbContexts).Distinct(),
                entities = AllProjects(app).SelectMany(p => p.Entities).Distinct().Take(60)
            },
            "security-and-authentication.md" => new
            {
                name, capabilities,
                authSchemes = AllProjects(app).SelectMany(p => p.AuthSchemes).Distinct(),
                authPolicies = AllProjects(app).SelectMany(p => p.AuthPolicies).Distinct(),
                authRoles = AllProjects(app).SelectMany(p => p.AuthRoles).Distinct(),
                protectedControllers = AllControllers(app).Where(c => c.Authorization is not null).Select(c => new { c.Name, c.Authorization }),
                angularGuards = AllProjects(app).Where(p => p.Angular is not null).SelectMany(p => p.Angular!.Guards).Distinct()
            },
            "api-reference.md" => new
            {
                name,
                controllers = AllControllers(app).Select(c => new { c.Name, c.Route, actions = Cap(c.Actions, 25) }),
                angularApiCalls = AllProjects(app).Where(p => p.Angular is not null).SelectMany(p => p.Angular!.ApiCalls).Distinct().Take(60)
            },
            _ => new { name, technologies = app.Technologies, capabilities }
        };
    }

    private static IEnumerable<ProjectModel> AllProjects(ApplicationModel app) => app.Repositories.SelectMany(r => r.Projects);
    private static IEnumerable<ControllerModel> AllControllers(ApplicationModel app) => AllProjects(app).SelectMany(p => p.Controllers);

    private static object AngularSummary(ApplicationModel app) =>
        AllProjects(app).Where(p => p.Angular is not null).Select(p => new
        {
            components = Cap(p.Angular!.Components, 20),
            services = Cap(p.Angular!.Services, 20),
            routes = Cap(p.Angular!.Routes, 20),
            apiCalls = Cap(p.Angular!.ApiCalls, 30)
        });

    // Cap a list, appending a "+N more" marker so the AI knows it's truncated.
    private static List<string> Cap(List<string> list, int max) =>
        list.Count <= max ? list : list.Take(max).Append($"…(+{list.Count - max} more)").ToList();
}
