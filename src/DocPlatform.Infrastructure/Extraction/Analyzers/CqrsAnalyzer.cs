using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DocPlatform.Infrastructure.Extraction.Analyzers;

// Detects MediatR-style CQRS requests: types ending in "Command" or "Query"
// (classes and records).
public sealed class CqrsAnalyzer : IProjectAnalyzer
{
    public void Analyze(ProjectAnalysisContext context)
    {
        foreach (AnalyzedFile file in context.Files)
        foreach (TypeDeclarationSyntax t in file.Root.DescendantNodes().OfType<TypeDeclarationSyntax>())
        {
            if (t is InterfaceDeclarationSyntax) continue;
            string n = t.Identifier.Text;
            if (n.EndsWith("Command") || n.EndsWith("Query"))
                context.Project.CqrsRequests.Add(n);
        }
    }
}
