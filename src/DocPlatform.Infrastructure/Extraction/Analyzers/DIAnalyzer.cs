using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DocPlatform.Infrastructure.Extraction.Analyzers;

// Detects services registered through Dependency Injection:
//   AddScoped<IFoo, Foo>()  /  AddSingleton<Foo>()  /  AddTransient<...>()  /  AddHostedService<...>()
// Captures implementation classes even when they DON'T end in "Service", and records
// the interface -> implementation map (used later by the call-graph analyzer).
// TODO: also resolve non-generic registrations: AddScoped(typeof(IFoo), typeof(Foo)).
public sealed class DIAnalyzer : IProjectAnalyzer
{
    private static readonly HashSet<string> RegisterMethods = new(StringComparer.Ordinal)
    {
        "AddScoped", "AddSingleton", "AddTransient", "AddHostedService"
    };

    public void Analyze(ProjectAnalysisContext context)
    {
        foreach (AnalyzedFile file in context.Files)
        foreach (InvocationExpressionSyntax inv in file.Root.DescendantNodes().OfType<InvocationExpressionSyntax>())
        {
            // We only handle the generic form: services.AddScoped<...>()
            if (inv.Expression is not MemberAccessExpressionSyntax { Name: GenericNameSyntax gname }) continue;
            if (!RegisterMethods.Contains(gname.Identifier.Text)) continue;

            var typeArgs = gname.TypeArgumentList.Arguments;
            if (typeArgs.Count == 0) continue;

            string impl = RoslynSyntax.LastSegment(typeArgs[^1].ToString());          // last arg = implementation
            string? iface = typeArgs.Count >= 2 ? RoslynSyntax.LastSegment(typeArgs[0].ToString()) : null;

            // Register the concrete implementation as a service (skip interface-only regs).
            if (!impl.StartsWith("I") || !LooksLikeInterface(impl))
                context.Project.Services.Add(impl);

            if (iface is not null)
            {
                if (LooksLikeInterface(iface)) context.Project.Interfaces.Add(iface);
                context.Project.ServiceImplementations[iface] = impl;
            }
        }
    }

    // "IFoo" is an interface; "Invoice" is not (starts with I but second char isn't uppercase).
    private static bool LooksLikeInterface(string name) =>
        name.Length >= 2 && name[0] == 'I' && char.IsUpper(name[1]);
}
