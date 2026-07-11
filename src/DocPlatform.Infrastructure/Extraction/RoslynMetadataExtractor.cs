using DocPlatform.Core.Abstractions;
using DocPlatform.Core.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DocPlatform.Infrastructure.Extraction;

// ACCURATE extractor: parses real C# syntax trees with Roslyn (the C# compiler
// as a library). Understands base types, attributes and methods precisely —
// no regex guesswork. Same IMetadataExtractor, so it drops in for the heuristic one.
public class RoslynMetadataExtractor : IMetadataExtractor
{
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

            // ---- interfaces ----
            foreach (InterfaceDeclarationSyntax iface in root.DescendantNodes().OfType<InterfaceDeclarationSyntax>())
            {
                string name = iface.Identifier.Text;
                if (name.StartsWith("I")) project.Interfaces.Add(name);
                if (name.EndsWith("Service")) project.Services.Add(name);
            }

            // ---- classes ----
            foreach (ClassDeclarationSyntax cls in root.DescendantNodes().OfType<ClassDeclarationSyntax>())
            {
                string name = cls.Identifier.Text;
                List<string> baseTypes = cls.BaseList?.Types.Select(t => LastSegment(t.Type.ToString())).ToList() ?? new();

                if (HasAttribute(cls.AttributeLists, "Authorize")) project.HasAuthentication = true;

                // Controllers: name ends Controller OR inherits ControllerBase/Controller
                bool isController = name.EndsWith("Controller")
                    || baseTypes.Any(b => b is "ControllerBase" or "Controller");
                if (isController)
                {
                    var controller = new ControllerModel { Name = name };
                    controller.Route = FirstStringArg(cls.AttributeLists, "Route");

                    foreach (MethodDeclarationSyntax method in cls.Members.OfType<MethodDeclarationSyntax>())
                    {
                        if (!method.Modifiers.Any(SyntaxKind.PublicKeyword)) continue;
                        string? verb = HttpVerb(method.AttributeLists);
                        if (verb is not null)
                            controller.Actions.Add($"{verb} {method.Identifier.Text}");
                    }
                    project.Controllers.Add(controller);
                }

                // DbContexts: inherits DbContext
                if (baseTypes.Any(b => b == "DbContext")) project.DbContexts.Add(name);

                // Services by naming convention
                if (name.EndsWith("Service")) project.Services.Add(name);

                // Entities: classes under Models/Entities/Domain
                if (inModelsFolder) project.Entities.Add(name);
            }

            if (code.Contains("AddAuthentication")) project.HasAuthentication = true;
        }

        ExtractionHelpers.Dedupe(project.Services);
        ExtractionHelpers.Dedupe(project.Interfaces);
        ExtractionHelpers.Dedupe(project.Entities);
        ExtractionHelpers.Dedupe(project.DbContexts);
    }

    // -- attribute helpers ----------------------------------------------------
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
            if (n.StartsWith("Http") && n.Length > 4)
                return n[4..].ToUpper();   // HttpGet -> GET
        }
        return null;
    }

    private static string LastSegment(string typeName) => typeName.Split('.').Last().Trim();
}
