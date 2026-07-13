using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DocPlatform.Infrastructure.Extraction.Analyzers;

// Detects EF Core DbContexts (including IdentityDbContext and any *DbContext base).
public sealed class DbContextAnalyzer : IProjectAnalyzer
{
    public void Analyze(ProjectAnalysisContext context)
    {
        foreach (AnalyzedFile file in context.Files)
        foreach (ClassDeclarationSyntax cls in file.Root.DescendantNodes().OfType<ClassDeclarationSyntax>())
        {
            string name = cls.Identifier.Text;
            List<string> baseTypes = RoslynSyntax.BaseTypeNames(cls.BaseList);
            if (baseTypes.Any(b => b.EndsWith("DbContext")) || name.EndsWith("DbContext"))
                context.Project.DbContexts.Add(name);
        }
    }
}
