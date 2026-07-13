using DocPlatform.Core.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DocPlatform.Infrastructure.Extraction.Analyzers;

// Detects HTTP endpoints: attribute-routed controllers and minimal APIs.
// Emits final resolved endpoint paths (via RouteResolver).
public sealed class ControllerAnalyzer : IProjectAnalyzer
{
    private static readonly string[] HttpMaps = { "MapGet", "MapPost", "MapPut", "MapDelete", "MapPatch" };

    public void Analyze(ProjectAnalysisContext context)
    {
        foreach (AnalyzedFile file in context.Files)
        {
            foreach (ClassDeclarationSyntax cls in file.Root.DescendantNodes().OfType<ClassDeclarationSyntax>())
            {
                string name = cls.Identifier.Text;
                List<string> baseTypes = RoslynSyntax.BaseTypeNames(cls.BaseList);

                bool isController = name.EndsWith("Controller")
                    || baseTypes.Any(b => b is "ControllerBase" or "Controller");
                if (!isController) continue;

                string? controllerTemplate = RoslynSyntax.FirstStringArg(cls.AttributeLists, "Route");
                var controller = new ControllerModel
                {
                    Name = name,
                    Route = RouteResolver.ForController(name, controllerTemplate, null, string.Empty),
                    Authorization = ReadAuthorization(cls.AttributeLists)
                };

                foreach (MethodDeclarationSyntax method in cls.Members.OfType<MethodDeclarationSyntax>())
                {
                    if (!method.Modifiers.Any(SyntaxKind.PublicKeyword)) continue;
                    (string? verb, string? actionTemplate) = RoslynSyntax.HttpVerbAndTemplate(method.AttributeLists);
                    if (verb is null) continue;
                    actionTemplate ??= RoslynSyntax.FirstStringArg(method.AttributeLists, "Route");
                    string path = RouteResolver.ForController(name, controllerTemplate, actionTemplate, method.Identifier.Text);
                    controller.Actions.Add($"{verb} {path}");
                }

                context.Project.Controllers.Add(controller);
            }

            AnalyzeMinimalApis(file, context.Project);
        }
    }

    // Controller-level authorization: "AllowAnonymous", "Authorize", or "Authorize (Roles: …)".
    private static string? ReadAuthorization(SyntaxList<AttributeListSyntax> lists)
    {
        List<AttributeSyntax> attrs = lists.SelectMany(l => l.Attributes).ToList();

        if (attrs.Any(a => RoslynSyntax.LastSegment(a.Name.ToString()) == "AllowAnonymous"))
            return "AllowAnonymous";

        AttributeSyntax? auth = attrs.FirstOrDefault(a => RoslynSyntax.LastSegment(a.Name.ToString()) == "Authorize");
        if (auth is null) return null;

        var details = new List<string>();
        foreach (AttributeArgumentSyntax arg in auth.ArgumentList?.Arguments ?? default)
        {
            if (arg.Expression is not LiteralExpressionSyntax lit || lit.Token.Value is not string val) continue;
            string? nm = arg.NameEquals?.Name.Identifier.Text;
            if (nm == "Roles") details.Add($"Roles: {val}");
            else if (nm == "Policy" || nm is null) details.Add($"Policy: {val}");
        }
        return details.Count > 0 ? $"Authorize ({string.Join(", ", details)})" : "Authorize";
    }

    private static void AnalyzeMinimalApis(AnalyzedFile file, ProjectModel project)
    {
        // Distinct MapGroup prefixes joined as a base path.
        // TODO: full nested-group resolution needs data-flow analysis of the group variables.
        var groupPrefixes = new List<string>();
        foreach (InvocationExpressionSyntax inv in file.Root.DescendantNodes().OfType<InvocationExpressionSyntax>())
            if (inv.Expression is MemberAccessExpressionSyntax { Name.Identifier.Text: "MapGroup" }
                && RoslynSyntax.FirstStringLiteral(inv.ArgumentList) is string g)
                groupPrefixes.Add(g);

        string prefix = RouteResolver.Combine(groupPrefixes.Distinct(StringComparer.OrdinalIgnoreCase).ToArray());

        var endpoints = new List<string>();
        foreach (InvocationExpressionSyntax inv in file.Root.DescendantNodes().OfType<InvocationExpressionSyntax>())
        {
            if (inv.Expression is not MemberAccessExpressionSyntax ma) continue;
            string m = ma.Name.Identifier.Text;
            if (!HttpMaps.Contains(m)) continue;

            string? path = RoslynSyntax.FirstStringLiteral(inv.ArgumentList);
            string full = RouteResolver.Combine(prefix, path ?? string.Empty);
            endpoints.Add($"{m[3..].ToUpper()} {full}");
        }

        if (endpoints.Count > 0)
        {
            var controller = new ControllerModel
            {
                Name = Path.GetFileNameWithoutExtension(file.Path) + " (Minimal API)",
                Route = string.IsNullOrEmpty(prefix) ? null : prefix
            };
            controller.Actions.AddRange(endpoints.Distinct());
            project.Controllers.Add(controller);
        }
    }
}
