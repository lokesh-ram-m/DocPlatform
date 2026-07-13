using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DocPlatform.Infrastructure.Extraction.Analyzers;

// Detects interfaces (I-prefixed) and services (types ending in "Service").
// Stage 3's DIAnalyzer will add DI-registration-based service detection.
public sealed class ServiceAnalyzer : IProjectAnalyzer
{
    public void Analyze(ProjectAnalysisContext context)
    {
        foreach (AnalyzedFile file in context.Files)
        {
            // Interfaces go to Interfaces only (not Services).
            foreach (InterfaceDeclarationSyntax iface in file.Root.DescendantNodes().OfType<InterfaceDeclarationSyntax>())
                if (iface.Identifier.Text.StartsWith("I"))
                    context.Project.Interfaces.Add(iface.Identifier.Text);

            // Services = concrete classes ending in "Service" (DIAnalyzer adds registered impls too).
            foreach (TypeDeclarationSyntax t in file.Root.DescendantNodes().OfType<TypeDeclarationSyntax>())
            {
                if (t is InterfaceDeclarationSyntax) continue;
                if (t.Identifier.Text.EndsWith("Service")) context.Project.Services.Add(t.Identifier.Text);
            }
        }
    }
}
