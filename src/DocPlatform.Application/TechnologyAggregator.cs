using DocPlatform.Core.Models;

namespace DocPlatform.Application;

// Deterministically rolls up the application-wide technology stack from the
// scanned projects. (Facts derived by code, not the LLM.)
public static class TechnologyAggregator
{
    public static List<string> From(ApplicationModel application)
    {
        var tech = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (ProjectModel p in application.Repositories.SelectMany(r => r.Projects))
        {
            if (p.Kind == ProjectKind.DotNetApi) tech.Add("ASP.NET Core");
            if (p.Kind == ProjectKind.Angular) tech.Add("Angular");
            if (p.TargetFramework is not null) tech.Add($".NET ({p.TargetFramework})");
            if (p.HasAuthentication) tech.Add("JWT Authentication");

            if (Has(p, "Dapper")) tech.Add("Dapper");
            if (Has(p, "EntityFrameworkCore")) tech.Add("EF Core");
            if (Has(p, "SqlClient")) tech.Add("SQL Server");
            if (Has(p, "SemanticKernel")) tech.Add("Semantic Kernel (AI)");
            if (Has(p, "Swashbuckle") || Has(p, "OpenApi")) tech.Add("OpenAPI / Swagger");
        }

        return tech.OrderBy(t => t).ToList();
    }

    private static bool Has(ProjectModel p, string packageFragment) =>
        p.PackageReferences.Any(x => x.Contains(packageFragment, StringComparison.OrdinalIgnoreCase));
}
