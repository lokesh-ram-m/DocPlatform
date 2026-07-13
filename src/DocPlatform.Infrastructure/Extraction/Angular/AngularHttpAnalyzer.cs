using System.Text.RegularExpressions;

namespace DocPlatform.Infrastructure.Extraction.Angular;

// Detects the backend endpoints the app calls (this.http.get(...)), resolving the
// base-URL field (apiUrl/baseUrl/...) and path parameters so the endpoint is meaningful.
// This is what connects the frontend to the backend in the application view.
public sealed class AngularHttpAnalyzer : IAngularAnalyzer
{
    private static readonly Regex HttpCall = new(
        @"\bhttp\w*\.(get|post|put|delete|patch)\s*(?:<[^>]*>)?\s*\(\s*([^,)]+)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex BaseUrlField = new(
        @"(?:apiUrl|apiBaseUrl|baseUrl|API_URL|apiRoot)\s*[:=]\s*[`'""]([^`'""]+)[`'""]",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public void Analyze(AngularAnalysisContext context)
    {
        foreach (AngularFile f in context.Files)
        {
            string basePath = ExtractBasePath(f.Content);
            foreach (Match m in HttpCall.Matches(f.Content))
            {
                string method = m.Groups[1].Value.ToUpperInvariant();
                string url = ResolveUrl(m.Groups[2].Value, basePath);
                if (url.Length > 1) context.Angular.ApiCalls.Add($"{method} {url}");
            }
        }
    }

    // e.g. "http://localhost:5114/api/tasks" -> "/api/tasks"
    private static string ExtractBasePath(string content)
    {
        Match m = BaseUrlField.Match(content);
        if (!m.Success) return string.Empty;
        return Regex.Replace(m.Groups[1].Value, @"^https?://[^/]+", string.Empty).TrimEnd('/');
    }

    private static string ResolveUrl(string expr, string basePath)
    {
        expr = expr.Trim().Trim('`', '\'', '"', ' ');
        expr = Regex.Replace(expr, @"\$\{[^}]*[Uu]rl[^}]*\}", basePath);   // ${this.apiUrl}
        expr = Regex.Replace(expr, @"\bthis\.\w*[Uu]rl\b", basePath);       // this.apiUrl (bare)
        expr = Regex.Replace(expr, @"\$\{[^}]*\}", "{id}");                 // remaining path params
        expr = expr.Trim().Trim('`', '\'', '"', ' ');
        while (expr.Contains("//")) expr = expr.Replace("//", "/");
        return expr.Contains('/') ? expr : string.Empty;
    }
}
