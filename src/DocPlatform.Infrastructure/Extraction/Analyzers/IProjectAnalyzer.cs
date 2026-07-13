namespace DocPlatform.Infrastructure.Extraction.Analyzers;

// One focused analysis pass over a project. Each analyzer reads the parsed files
// in the context and contributes facts to the ProjectModel. Add a new analyzer to
// the pipeline to extend what the tool understands — no changes to the others.
public interface IProjectAnalyzer
{
    void Analyze(ProjectAnalysisContext context);
}
