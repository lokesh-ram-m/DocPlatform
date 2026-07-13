using DocPlatform.Core.Abstractions;
using DocPlatform.Core.Models;
using DocPlatform.Infrastructure.Extraction.Angular;

namespace DocPlatform.Infrastructure.Extraction;

// Runs a pipeline of Angular analyzers over each Angular project's TypeScript files.
// Mirrors RoslynMetadataExtractor on the .NET side (heuristic instead of semantic).
public class AngularMetadataExtractor : IMetadataExtractor
{
    private readonly IReadOnlyList<IAngularAnalyzer> _analyzers = new IAngularAnalyzer[]
    {
        new AngularComponentAnalyzer(),
        new AngularServiceAnalyzer(),
        new AngularRouteAnalyzer(),
        new AngularHttpAnalyzer(),
        new AngularGuardAnalyzer(),
    };

    public void Extract(RepositoryModel repository)
    {
        foreach (ProjectModel project in repository.Projects)
        {
            if (project.Kind != ProjectKind.Angular) continue;

            AngularAnalysisContext context = BuildContext(project);
            foreach (IAngularAnalyzer analyzer in _analyzers)
                analyzer.Analyze(context);

            AngularInfo ng = project.Angular!;
            ExtractionHelpers.Dedupe(ng.Components);
            ExtractionHelpers.Dedupe(ng.Routes);
            ExtractionHelpers.Dedupe(ng.Services);
            ExtractionHelpers.Dedupe(ng.Guards);
            ExtractionHelpers.Dedupe(ng.ApiCalls);
        }
    }

    private static AngularAnalysisContext BuildContext(ProjectModel project)
    {
        string dir = Path.GetDirectoryName(project.Path)!;   // package.json's folder = project root
        var files = new List<AngularFile>();

        foreach (string f in ExtractionHelpers.EnumerateFiles(dir)
                     .Where(f => f.EndsWith(".ts") && !f.EndsWith(".spec.ts")))
        {
            string content;
            try { content = File.ReadAllText(f); } catch { continue; }
            files.Add(new AngularFile { Path = f, Name = Path.GetFileName(f), Content = content });
        }

        return new AngularAnalysisContext { Project = project, Files = files };
    }
}
