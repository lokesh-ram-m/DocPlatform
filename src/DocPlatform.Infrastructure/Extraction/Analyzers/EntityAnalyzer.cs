using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DocPlatform.Infrastructure.Extraction.Analyzers;

// Detects domain entities. Currently folder-based (Models/Entities/Domain) + a filter
// that drops DTOs/responses/etc. Stage 2 will add DbSet<T> and base-class signals.
public sealed class EntityAnalyzer : IProjectAnalyzer
{
    public void Analyze(ProjectAnalysisContext context)
    {
        foreach (AnalyzedFile file in context.Files)
        {
            if (!file.InModelsFolder) continue;
            foreach (TypeDeclarationSyntax t in file.Root.DescendantNodes().OfType<TypeDeclarationSyntax>())
            {
                if (t is not (ClassDeclarationSyntax or RecordDeclarationSyntax)) continue;
                string name = t.Identifier.Text;
                if (ExtractionHelpers.IsLikelyEntity(name))
                    context.Project.Entities.Add(name);
            }
        }
    }
}
