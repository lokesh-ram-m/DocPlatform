using System.Text.RegularExpressions;

namespace DocPlatform.Infrastructure.Extraction.Angular;

// Detects Angular services — classes decorated with @Injectable.
public sealed class AngularServiceAnalyzer : IAngularAnalyzer
{
    public void Analyze(AngularAnalysisContext context)
    {
        foreach (AngularFile f in context.Files)
        {
            if (!f.Content.Contains("@Injectable")) continue;
            foreach (Match m in Regex.Matches(f.Content, @"export\s+class\s+(\w+)"))
                context.Angular.Services.Add(m.Groups[1].Value);
        }
    }
}
