using System.Text.RegularExpressions;
using DocPlatform.Core.Abstractions;
using DocPlatform.Core.Models;
using DocPlatform.Infrastructure.Extraction.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace DocPlatform.Infrastructure.Extraction;

// Coordinates a pipeline of focused analyzers over each .NET project.
// Parses the project's .cs files once into a shared context, then runs each analyzer.
// Add a new IProjectAnalyzer to extend what the tool understands.
public class RoslynMetadataExtractor : IMetadataExtractor
{
    private static readonly Regex ModelsFolder =
        new(@"[/\\](Models|Entities|Domain)([/\\]|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    // BCL references, loaded once — lets the SemanticModel resolve core types.
    private static readonly Lazy<MetadataReference[]> ReferenceAssemblies = new(() =>
    {
        var refs = new List<MetadataReference>();
        if (AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") is string tpa)
            foreach (string path in tpa.Split(Path.PathSeparator))
                try { refs.Add(MetadataReference.CreateFromFile(path)); } catch { /* skip unreadable */ }
        if (refs.Count == 0)
            refs.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
        return refs.ToArray();
    });

    private readonly IReadOnlyList<IProjectAnalyzer> _analyzers = new IProjectAnalyzer[]
    {
        new ControllerAnalyzer(),
        new DbContextAnalyzer(),
        new EntityAnalyzer(),
        new ServiceAnalyzer(),
        new DIAnalyzer(),
        new CqrsAnalyzer(),
        new AuthAnalyzer(),
    };

    public void Extract(RepositoryModel repository)
    {
        foreach (ProjectModel project in repository.Projects)
        {
            if (project.Kind == ProjectKind.Angular)
            {
                ExtractionHelpers.ExtractAngular(project);
                continue;
            }

            ProjectAnalysisContext context = BuildContext(project);
            foreach (IProjectAnalyzer analyzer in _analyzers)
                analyzer.Analyze(context);

            ExtractionHelpers.Dedupe(project.Services);
            ExtractionHelpers.Dedupe(project.Interfaces);
            ExtractionHelpers.Dedupe(project.Entities);
            ExtractionHelpers.Dedupe(project.DbContexts);
            ExtractionHelpers.Dedupe(project.CqrsRequests);
        }
    }

    private static ProjectAnalysisContext BuildContext(ProjectModel project)
    {
        string projectDir = Path.GetDirectoryName(project.Path)!;
        var files = new List<AnalyzedFile>();

        foreach (string file in ExtractionHelpers.EnumerateFiles(projectDir).Where(f => f.EndsWith(".cs")))
        {
            string code;
            try { code = File.ReadAllText(file); } catch { continue; }

            SyntaxTree tree = CSharpSyntaxTree.ParseText(code, path: file);
            string relDir = Path.GetDirectoryName(file)!.Replace(projectDir, "");

            files.Add(new AnalyzedFile
            {
                Path = file,
                RelativeDir = relDir,
                Tree = tree,
                InModelsFolder = ModelsFolder.IsMatch(relDir)
            });
        }

        // Build a semantic compilation from the project's own trees. External framework
        // types may not resolve, but the project's own symbols (cross-file) will.
        CSharpCompilation compilation = CSharpCompilation.Create(
            "DocPlatform.Analysis",
            files.Select(f => f.Tree),
            ReferenceAssemblies.Value,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        return new ProjectAnalysisContext { Project = project, Files = files, Compilation = compilation };
    }
}
