using System.Text.RegularExpressions;

namespace DocPlatform.Infrastructure.Extraction.Angular;

// Detects routes and maps each to its component (or marks lazy-loaded routes).
public sealed class AngularRouteAnalyzer : IAngularAnalyzer
{
    public void Analyze(AngularAnalysisContext context)
    {
        foreach (AngularFile f in context.Files)
        {
            if (!IsRouteSource(f)) continue;

            foreach (Match m in Regex.Matches(f.Content, @"path:\s*['""]([^'""]*)['""]"))
            {
                string path = string.IsNullOrEmpty(m.Groups[1].Value) ? "(root)" : m.Groups[1].Value;

                // Look a little ahead for the component / lazy loader of this route.
                string window = f.Content.Substring(m.Index, Math.Min(160, f.Content.Length - m.Index));
                Match component = Regex.Match(window, @"component:\s*(\w+)");
                bool lazy = window.Contains("loadComponent") || window.Contains("loadChildren");

                string label = component.Success ? $"{path} → {component.Groups[1].Value}"
                             : lazy ? $"{path} → (lazy)"
                             : path;
                context.Angular.Routes.Add(label);
            }
        }
    }

    private static bool IsRouteSource(AngularFile f) =>
        f.Name.Contains("routes", StringComparison.OrdinalIgnoreCase) ||
        f.Name.Contains("routing", StringComparison.OrdinalIgnoreCase) ||
        f.Content.Contains("RouterModule") ||
        f.Content.Contains("provideRouter") ||
        Regex.IsMatch(f.Content, @":\s*Routes\b");
}
