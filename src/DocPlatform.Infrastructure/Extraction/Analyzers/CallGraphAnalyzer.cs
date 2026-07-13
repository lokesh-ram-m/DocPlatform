using DocPlatform.Core.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DocPlatform.Infrastructure.Extraction.Analyzers;

// Builds the component call graph from CONSTRUCTOR INJECTION: a class's constructor
// parameters are its dependencies (controller -> service -> repository -> data).
// Injected interfaces are resolved to implementations via the DI map, else by naming.
// TODO: method-call analysis for non-injected usage; cross-project symbol resolution.
public sealed class CallGraphAnalyzer : IProjectAnalyzer
{
    private static readonly string[] ComponentSuffixes =
        { "Service", "Repository", "Context", "Store", "Handler", "Provider", "Manager", "Client", "Gateway", "UnitOfWork" };

    // Framework/infrastructure injections that aren't domain components — exclude from the graph.
    private static readonly HashSet<string> Framework = new(StringComparer.Ordinal)
    {
        "IConfiguration", "ILogger", "IMapper", "IMediator", "ISender", "IHttpContextAccessor",
        "IServiceProvider", "IServiceScopeFactory", "IWebHostEnvironment", "IHostEnvironment",
        "IMemoryCache", "IDistributedCache", "IHttpClientFactory", "HttpClient", "DbContext"
    };

    public void Analyze(ProjectAnalysisContext context)
    {
        foreach (AnalyzedFile file in context.Files)
        foreach (ClassDeclarationSyntax cls in file.Root.DescendantNodes().OfType<ClassDeclarationSyntax>())
        {
            string from = cls.Identifier.Text;

            // Traditional constructors + primary constructor parameters.
            IEnumerable<ParameterSyntax> parameters = cls.Members
                .OfType<ConstructorDeclarationSyntax>()
                .SelectMany(c => c.ParameterList.Parameters)
                .Concat(cls.ParameterList?.Parameters ?? Enumerable.Empty<ParameterSyntax>());

            foreach (ParameterSyntax param in parameters)
            {
                if (param.Type is null) continue;
                string typeName = RoslynSyntax.NormalizeBase(param.Type.ToString());
                if (!IsComponent(typeName)) continue;

                string to = Resolve(typeName, context.Project);
                if (!string.Equals(to, from, StringComparison.Ordinal))
                    context.Project.ComponentDependencies.Add(new Relationship { From = from, To = to, Type = "uses" });
            }
        }

        // de-duplicate
        context.Project.ComponentDependencies = context.Project.ComponentDependencies
            .GroupBy(r => $"{r.From}|{r.To}", StringComparer.Ordinal)
            .Select(g => g.First())
            .ToList();
    }

    private static bool IsComponent(string typeName)
    {
        if (Framework.Contains(typeName)) return false;
        return IsInterface(typeName) || ComponentSuffixes.Any(s => typeName.EndsWith(s, StringComparison.Ordinal));
    }

    private static string Resolve(string typeName, ProjectModel project)
    {
        if (project.ServiceImplementations.TryGetValue(typeName, out string? impl)) return impl;  // DI map
        if (IsInterface(typeName)) return typeName[1..];                                           // IFoo -> Foo
        return typeName;
    }

    private static bool IsInterface(string name) =>
        name.Length >= 2 && name[0] == 'I' && char.IsUpper(name[1]);
}
