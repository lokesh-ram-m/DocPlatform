using DocPlatform.Core.Abstractions;
using DocPlatform.Core.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DocPlatform.Infrastructure.Extraction;

// ACCURATE extractor: parses real C# syntax trees with Roslyn. Handles controllers,
// minimal APIs, EF/Identity DbContexts, CQRS requests, services, interfaces, entities.
public class RoslynMetadataExtractor : IMetadataExtractor
{
    private static readonly string[] HttpMaps = { "MapGet", "MapPost", "MapPut", "MapDelete", "MapPatch" };

    public void Extract(RepositoryModel repository)
    {
        foreach (ProjectModel project in repository.Projects)
        {
            if (project.Kind == ProjectKind.Angular) ExtractionHelpers.ExtractAngular(project);
            else ExtractDotNet(project);
        }
    }

    private static void ExtractDotNet(ProjectModel project)
    {
        string projectDir = Path.GetDirectoryName(project.Path)!;

        if (project.PackageReferences.Any(p => p.Contains("Authentication", StringComparison.OrdinalIgnoreCase)))
            project.HasAuthentication = true;

        foreach (string file in ExtractionHelpers.EnumerateFiles(projectDir).Where(f => f.EndsWith(".cs")))
        {
            string code;
            try { code = File.ReadAllText(file); } catch { continue; }

            SyntaxNode root = CSharpSyntaxTree.ParseText(code).GetRoot();
            string relDir = Path.GetDirectoryName(file)!.Replace(projectDir, "");
            bool inModelsFolder = System.Text.RegularExpressions.Regex.IsMatch(
                relDir, @"[/\\](Models|Entities|Domain)([/\\]|$)",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            // interfaces
            foreach (InterfaceDeclarationSyntax iface in root.DescendantNodes().OfType<InterfaceDeclarationSyntax>())
            {
                string n = iface.Identifier.Text;
                if (n.StartsWith("I")) project.Interfaces.Add(n);
                if (n.EndsWith("Service")) project.Services.Add(n);
            }

            // naming conventions across classes AND records (CQRS uses records)
            foreach (TypeDeclarationSyntax t in root.DescendantNodes().OfType<TypeDeclarationSyntax>())
            {
                if (t is InterfaceDeclarationSyntax) continue;
                string n = t.Identifier.Text;
                if (n.EndsWith("Service")) project.Services.Add(n);
                if (n.EndsWith("Command") || n.EndsWith("Query")) project.CqrsRequests.Add(n);
                if (inModelsFolder && ExtractionHelpers.IsLikelyEntity(n) && t is ClassDeclarationSyntax or RecordDeclarationSyntax)
                    project.Entities.Add(n);
            }

            // classes: controllers, DbContexts, auth
            foreach (ClassDeclarationSyntax cls in root.DescendantNodes().OfType<ClassDeclarationSyntax>())
            {
                string name = cls.Identifier.Text;
                List<string> baseTypes = cls.BaseList?.Types.Select(x => NormalizeBase(x.Type.ToString())).ToList() ?? new();

                if (HasAttribute(cls.AttributeLists, "Authorize")) project.HasAuthentication = true;

                if (name.EndsWith("Controller") || baseTypes.Any(b => b is "ControllerBase" or "Controller"))
                {
                    string? controllerTemplate = FirstStringArg(cls.AttributeLists, "Route");
                    var controller = new ControllerModel
                    {
                        Name = name,
                        Route = RouteResolver.ForController(name, controllerTemplate, null, string.Empty)
                    };
                    foreach (MethodDeclarationSyntax method in cls.Members.OfType<MethodDeclarationSyntax>())
                    {
                        if (!method.Modifiers.Any(SyntaxKind.PublicKeyword)) continue;
                        (string? verb, string? actionTemplate) = HttpVerbAndTemplate(method.AttributeLists);
                        if (verb is null) continue;
                        actionTemplate ??= FirstStringArg(method.AttributeLists, "Route");   // [Route] on the action
                        string path = RouteResolver.ForController(name, controllerTemplate, actionTemplate, method.Identifier.Text);
                        controller.Actions.Add($"{verb} {path}");
                    }
                    project.Controllers.Add(controller);
                }

                // DbContext: base type OR class name ends with "DbContext" (covers IdentityDbContext)
                if (baseTypes.Any(b => b.EndsWith("DbContext")) || name.EndsWith("DbContext"))
                    project.DbContexts.Add(name);
            }

            // minimal APIs: MapGet/MapPost/... calls, grouped per file
            ExtractMinimalApis(root, file, project);
        }

        ExtractionHelpers.Dedupe(project.Services);
        ExtractionHelpers.Dedupe(project.Interfaces);
        ExtractionHelpers.Dedupe(project.Entities);
        ExtractionHelpers.Dedupe(project.DbContexts);
        ExtractionHelpers.Dedupe(project.CqrsRequests);
    }

    private static void ExtractMinimalApis(SyntaxNode root, string file, ProjectModel project)
    {
        // Collect the MapGroup prefixes in the file and join them as a base path.
        // TODO: full nested-group resolution needs data-flow analysis of the group variables;
        //       this best-effort join is correct for the common single-group-per-file pattern.
        var groupPrefixes = new List<string>();
        foreach (InvocationExpressionSyntax inv in root.DescendantNodes().OfType<InvocationExpressionSyntax>())
            if (inv.Expression is MemberAccessExpressionSyntax { Name.Identifier.Text: "MapGroup" }
                && FirstStringLiteral(inv.ArgumentList) is string g)
                groupPrefixes.Add(g);

        // Distinct: files often declare the same group prefix multiple times (e.g. per API
        // version) — joining duplicates would wrongly repeat the prefix.
        string prefix = RouteResolver.Combine(groupPrefixes.Distinct(StringComparer.OrdinalIgnoreCase).ToArray());

        var endpoints = new List<string>();
        foreach (InvocationExpressionSyntax inv in root.DescendantNodes().OfType<InvocationExpressionSyntax>())
        {
            if (inv.Expression is not MemberAccessExpressionSyntax ma) continue;
            string m = ma.Name.Identifier.Text;
            if (!HttpMaps.Contains(m)) continue;

            string? path = FirstStringLiteral(inv.ArgumentList);              // the endpoint's own route
            string full = RouteResolver.Combine(prefix, path ?? string.Empty);
            endpoints.Add($"{m[3..].ToUpper()} {full}");
        }

        if (endpoints.Count > 0)
        {
            var controller = new ControllerModel
            {
                Name = Path.GetFileNameWithoutExtension(file) + " (Minimal API)",
                Route = string.IsNullOrEmpty(prefix) ? null : prefix
            };
            controller.Actions.AddRange(endpoints.Distinct());   // same route can be mapped per API version
            project.Controllers.Add(controller);
        }
    }

    // -- helpers --------------------------------------------------------------
    private static bool HasAttribute(SyntaxList<AttributeListSyntax> lists, string name) =>
        lists.SelectMany(l => l.Attributes).Any(a => LastSegment(a.Name.ToString()) == name);

    private static string? FirstStringArg(SyntaxList<AttributeListSyntax> lists, string attrName)
    {
        AttributeSyntax? attr = lists.SelectMany(l => l.Attributes)
            .FirstOrDefault(a => LastSegment(a.Name.ToString()) == attrName);
        if (attr?.ArgumentList?.Arguments.FirstOrDefault()?.Expression is LiteralExpressionSyntax lit
            && lit.Token.Value is string s)
            return s;
        return null;
    }

    // Returns the HTTP verb and the optional route template from an [Http*("template")] attribute.
    private static (string? Verb, string? Template) HttpVerbAndTemplate(SyntaxList<AttributeListSyntax> lists)
    {
        foreach (AttributeSyntax a in lists.SelectMany(l => l.Attributes))
        {
            string n = LastSegment(a.Name.ToString());
            if (n.StartsWith("Http") && n.Length > 4)
            {
                string? template = null;
                if (a.ArgumentList?.Arguments.FirstOrDefault()?.Expression is LiteralExpressionSyntax lit
                    && lit.Token.Value is string s)
                    template = s;
                return (n[4..].ToUpper(), template);
            }
        }
        return (null, null);
    }

    private static string? FirstStringLiteral(ArgumentListSyntax? args)
    {
        foreach (ArgumentSyntax a in args?.Arguments ?? default)
            if (a.Expression is LiteralExpressionSyntax lit && lit.Token.Value is string s)
                return s;
        return null;
    }

    // "IdentityDbContext<AppUser, Role, string>" -> "IdentityDbContext";  "A.B.DbContext" -> "DbContext"
    private static string NormalizeBase(string typeName)
    {
        int lt = typeName.IndexOf('<');
        if (lt >= 0) typeName = typeName[..lt];
        return LastSegment(typeName);
    }

    private static string LastSegment(string typeName) => typeName.Split('.').Last().Trim();
}
