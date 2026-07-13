using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DocPlatform.Infrastructure.Extraction.Analyzers;

// Detects authentication & authorization detail:
//   - schemes   (AddJwtBearer / AddCookie / AddOpenIdConnect / AddIdentity / ...)
//   - policies  (AddPolicy("name") and [Authorize(Policy=...)])
//   - roles     ([Authorize(Roles="Admin,...")])
//   - HasAuthentication (package / [Authorize] / AddAuthentication)
public sealed class AuthAnalyzer : IProjectAnalyzer
{
    private static readonly Dictionary<string, string> SchemeMethods = new(StringComparer.Ordinal)
    {
        ["AddJwtBearer"] = "JWT Bearer",
        ["AddCookie"] = "Cookie",
        ["AddOpenIdConnect"] = "OpenID Connect",
        ["AddOAuth"] = "OAuth",
        ["AddNegotiate"] = "Negotiate (Windows)",
        ["AddIdentity"] = "ASP.NET Core Identity",
        ["AddIdentityCore"] = "ASP.NET Core Identity",
        ["AddDefaultIdentity"] = "ASP.NET Core Identity",
        ["AddIdentityApiEndpoints"] = "ASP.NET Core Identity",
        ["AddMicrosoftIdentityWebApp"] = "Microsoft Identity (Entra ID)",
        ["AddMicrosoftIdentityWebApi"] = "Microsoft Identity (Entra ID)",
    };

    public void Analyze(ProjectAnalysisContext context)
    {
        var project = context.Project;

        if (project.PackageReferences.Any(p =>
                p.Contains("Authentication", StringComparison.OrdinalIgnoreCase) ||
                p.Contains("Identity", StringComparison.OrdinalIgnoreCase)))
            project.HasAuthentication = true;

        foreach (AnalyzedFile file in context.Files)
        {
            foreach (InvocationExpressionSyntax inv in file.Root.DescendantNodes().OfType<InvocationExpressionSyntax>())
            {
                if (inv.Expression is not MemberAccessExpressionSyntax ma) continue;
                string method = ma.Name.Identifier.Text;

                if (SchemeMethods.TryGetValue(method, out string? scheme))
                {
                    project.AuthSchemes.Add(scheme);
                    project.HasAuthentication = true;
                }
                else if (method == "AddAuthentication")
                {
                    project.HasAuthentication = true;
                }
                else if (method == "AddPolicy" && RoslynSyntax.FirstStringLiteral(inv.ArgumentList) is string policy)
                {
                    project.AuthPolicies.Add(policy);
                }
            }

            foreach (AttributeSyntax a in file.Root.DescendantNodes().OfType<AttributeSyntax>())
            {
                if (RoslynSyntax.LastSegment(a.Name.ToString()) != "Authorize") continue;
                project.HasAuthentication = true;
                ReadAuthorizeArgs(a, project);
            }
        }

        ExtractionHelpers.Dedupe(project.AuthSchemes);
        ExtractionHelpers.Dedupe(project.AuthPolicies);
        ExtractionHelpers.Dedupe(project.AuthRoles);
    }

    private static void ReadAuthorizeArgs(AttributeSyntax attr, Core.Models.ProjectModel project)
    {
        if (attr.ArgumentList is null) return;
        foreach (AttributeArgumentSyntax arg in attr.ArgumentList.Arguments)
        {
            if (arg.Expression is not LiteralExpressionSyntax lit || lit.Token.Value is not string val) continue;
            string? name = arg.NameEquals?.Name.Identifier.Text;

            if (name == "Roles")
                foreach (string r in val.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                    project.AuthRoles.Add(r);
            else if (name == "Policy" || name is null)   // Policy=… or positional policy name
                project.AuthPolicies.Add(val);
        }
    }
}
