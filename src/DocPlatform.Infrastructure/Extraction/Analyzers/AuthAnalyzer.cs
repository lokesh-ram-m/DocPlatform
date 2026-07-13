using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DocPlatform.Infrastructure.Extraction.Analyzers;

// Detects whether the project uses authentication:
//   - an Authentication package, OR
//   - an [Authorize] attribute anywhere, OR
//   - an AddAuthentication(...) registration.
public sealed class AuthAnalyzer : IProjectAnalyzer
{
    public void Analyze(ProjectAnalysisContext context)
    {
        if (context.Project.PackageReferences.Any(p => p.Contains("Authentication", StringComparison.OrdinalIgnoreCase)))
        {
            context.Project.HasAuthentication = true;
            return;
        }

        foreach (AnalyzedFile file in context.Files)
        {
            bool hasAuthorize = file.Root.DescendantNodes().OfType<AttributeSyntax>()
                .Any(a => RoslynSyntax.LastSegment(a.Name.ToString()) == "Authorize");
            bool addsAuth = file.Root.DescendantNodes().OfType<InvocationExpressionSyntax>()
                .Any(inv => inv.Expression is MemberAccessExpressionSyntax { Name.Identifier.Text: "AddAuthentication" });

            if (hasAuthorize || addsAuth)
            {
                context.Project.HasAuthentication = true;
                return;
            }
        }
    }
}
