namespace DocPlatform.Infrastructure.Extraction.Angular;

// One focused analysis pass over an Angular project's TypeScript files.
// Mirrors IProjectAnalyzer on the .NET side — add a new analyzer to extend coverage.
public interface IAngularAnalyzer
{
    void Analyze(AngularAnalysisContext context);
}
