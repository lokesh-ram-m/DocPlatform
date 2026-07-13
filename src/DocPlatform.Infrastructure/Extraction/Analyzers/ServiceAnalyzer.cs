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
            foreach (InterfaceDeclarationSyntax iface in file.Root.DescendantNodes().OfType<InterfaceDeclarationSyntax>())
            {
                string n = iface.Identifier.Text;
                if (n.StartsWith("I")) context.Project.Interfaces.Add(n);
                if (n.EndsWith("Service")) context.Project.Services.Add(n);
            }

            foreach (TypeDeclarationSyntax t in file.Root.DescendantNodes().OfType<TypeDeclarationSyntax>())
            {
                if (t is InterfaceDeclarationSyntax) continue;
                if (t.Identifier.Text.EndsWith("Service")) context.Project.Services.Add(t.Identifier.Text);
            }
        }
    }
}
