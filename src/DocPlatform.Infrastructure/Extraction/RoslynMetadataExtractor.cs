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

    private readonly IReadOnlyList<IProjectAnalyzer> _analyzers = new IProjectAnalyzer[]
    {
        new ControllerAnalyzer(),
        new DbContextAnalyzer(),
        new EntityAnalyzer(),
        new ServiceAnalyzer(),
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

            SyntaxNode root = CSharpSyntaxTree.ParseText(code).GetRoot();
            string relDir = Path.GetDirectoryName(file)!.Replace(projectDir, "");

            files.Add(new AnalyzedFile
            {
                Path = file,
                RelativeDir = relDir,
                Root = root,
                InModelsFolder = ModelsFolder.IsMatch(relDir)
            });
        }

        return new ProjectAnalysisContext { Project = project, Files = files };
    }
}
