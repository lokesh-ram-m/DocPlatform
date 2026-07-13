using System.Text.RegularExpressions;

namespace DocPlatform.Infrastructure.Extraction.Angular;

// Detects Angular components (*.component.ts) with their selector when present.
public sealed class AngularComponentAnalyzer : IAngularAnalyzer
{
    public void Analyze(AngularAnalysisContext context)
    {
        foreach (AngularFile f in context.Files)
        {
            if (!f.Name.EndsWith(".component.ts", StringComparison.OrdinalIgnoreCase)) continue;

            string name = f.Name[..^".component.ts".Length];
            Match selector = Regex.Match(f.Content, @"selector:\s*['""]([^'""]+)['""]");
            context.Angular.Components.Add(selector.Success ? $"{name} ({selector.Groups[1].Value})" : name);
        }
    }
}
