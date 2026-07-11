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
                    var controller = new ControllerModel { Name = name, Route = FirstStringArg(cls.AttributeLists, "Route") };
                    foreach (MethodDeclarationSyntax method in cls.Members.OfType<MethodDeclarationSyntax>())
                    {
                        if (!method.Modifiers.Any(SyntaxKind.PublicKeyword)) continue;
                        string? verb = HttpVerb(method.AttributeLists);
                        if (verb is not null) controller.Actions.Add($"{verb} {method.Identifier.Text}");
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
        var endpoints = new List<string>();
        string? groupRoute = null;

        foreach (InvocationExpressionSyntax inv in root.DescendantNodes().OfType<InvocationExpressionSyntax>())
        {
            if (inv.Expression is not MemberAccessExpressionSyntax ma) continue;
            string m = ma.Name.Identifier.Text;

            if (HttpMaps.Contains(m))
            {
                string label = FirstStringLiteral(inv.ArgumentList) ?? FirstArgText(inv.ArgumentList) ?? "";
                endpoints.Add($"{m[3..].ToUpper()} {label}".Trim());
            }
            else if (m == "MapGroup")
            {
                groupRoute ??= FirstStringLiteral(inv.ArgumentList);
            }
        }

        if (endpoints.Count > 0)
        {
            var controller = new ControllerModel
            {
                Name = Path.GetFileNameWithoutExtension(file) + " (Minimal API)",
                Route = groupRoute
            };
            controller.Actions.AddRange(endpoints);
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

    private static string? HttpVerb(SyntaxList<AttributeListSyntax> lists)
    {
        foreach (AttributeSyntax a in lists.SelectMany(l => l.Attributes))
        {
            string n = LastSegment(a.Name.ToString());
            if (n.StartsWith("Http") && n.Length > 4) return n[4..].ToUpper();
        }
        return null;
    }

    private static string? FirstStringLiteral(ArgumentListSyntax? args)
    {
        foreach (ArgumentSyntax a in args?.Arguments ?? default)
            if (a.Expression is LiteralExpressionSyntax lit && lit.Token.Value is string s)
                return s;
        return null;
    }

    private static string? FirstArgText(ArgumentListSyntax? args) =>
        args?.Arguments.FirstOrDefault()?.Expression.ToString();

    // "IdentityDbContext<AppUser, Role, string>" -> "IdentityDbContext";  "A.B.DbContext" -> "DbContext"
    private static string NormalizeBase(string typeName)
    {
        int lt = typeName.IndexOf('<');
        if (lt >= 0) typeName = typeName[..lt];
        return LastSegment(typeName);
    }

    private static string LastSegment(string typeName) => typeName.Split('.').Last().Trim();
}
