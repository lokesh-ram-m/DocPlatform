namespace DocPlatform.Infrastructure.Extraction;

// Resolves ASP.NET Core route templates into final endpoint paths.
// Handles controller + action template combination, [controller]/[action] tokens,
// absolute (~/ or /) action routes, and path normalization.
internal static class RouteResolver
{
    // Combine a controller-level [Route] template with an action-level template.
    public static string ForController(string controllerName, string? controllerTemplate,
        string? actionTemplate, string methodName)
    {
        string shortName = controllerName.EndsWith("Controller", StringComparison.OrdinalIgnoreCase)
            ? controllerName[..^"Controller".Length]
            : controllerName;

        string ct = Substitute(controllerTemplate, shortName, methodName);
        string at = Substitute(actionTemplate, shortName, methodName);

        string combined;
        if (IsAbsolute(at)) combined = at.TrimStart('~');           // action route overrides
        else if (string.IsNullOrEmpty(at)) combined = ct;
        else if (string.IsNullOrEmpty(ct)) combined = at;
        else combined = ct.TrimEnd('/') + "/" + at.TrimStart('/');

        return Normalize(combined);
    }

    // Join path segments (used for Minimal API MapGroup prefixes + endpoint paths).
    public static string Combine(params string?[] segments)
    {
        IEnumerable<string> parts = segments
            .Where(s => !string.IsNullOrEmpty(s))
            .Select(s => s!.Trim('/'))
            .Where(s => s.Length > 0);
        return Normalize(string.Join("/", parts));
    }

    private static string Substitute(string? template, string controller, string action) =>
        (template ?? string.Empty)
            .Replace("[controller]", controller, StringComparison.OrdinalIgnoreCase)
            .Replace("[action]", action, StringComparison.OrdinalIgnoreCase);

    private static bool IsAbsolute(string t) => t.StartsWith('/') || t.StartsWith("~/");

    private static string Normalize(string path)
    {
        while (path.Contains("//")) path = path.Replace("//", "/");
        path = "/" + path.Trim('/');
        return path;
    }
}
