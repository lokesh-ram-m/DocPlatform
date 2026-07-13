namespace DocPlatform.Infrastructure.Extraction.Angular;

// Detects route guards (*.guard.ts) and HTTP interceptors (*.interceptor.ts).
public sealed class AngularGuardAnalyzer : IAngularAnalyzer
{
    public void Analyze(AngularAnalysisContext context)
    {
        foreach (AngularFile f in context.Files)
        {
            if (f.Name.EndsWith(".guard.ts", StringComparison.OrdinalIgnoreCase))
                context.Angular.Guards.Add($"{f.Name[..^".guard.ts".Length]} (guard)");
            else if (f.Name.EndsWith(".interceptor.ts", StringComparison.OrdinalIgnoreCase))
                context.Angular.Guards.Add($"{f.Name[..^".interceptor.ts".Length]} (interceptor)");
        }
    }
}
