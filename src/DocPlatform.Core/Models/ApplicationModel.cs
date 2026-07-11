namespace DocPlatform.Core.Models;

// ==========================================================================
//  ApplicationModel — the SINGLE SOURCE OF TRUTH.
//  Every module either produces or consumes this object.
//  Documentation belongs to the APPLICATION; repositories are just sources.
//  (Deterministic code fills this in — the LLM only ever reads it.)
// ==========================================================================
public class ApplicationModel
{
    public string Name { get; set; } = string.Empty;

    public List<RepositoryModel> Repositories { get; set; } = new();

    // Aggregated across all repos (deduplicated) — convenient for the AI prompt.
    public List<string> Technologies { get; set; } = new();
}

public class RepositoryModel
{
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public bool HasReadme { get; set; }

    public List<ProjectModel> Projects { get; set; } = new();
}

public enum ProjectKind
{
    Unknown,
    DotNetApi,        // ASP.NET Core web project
    DotNetLibrary,    // shared class library
    DotNetOther,      // console/worker/etc.
    Angular
}

public class ProjectModel
{
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public ProjectKind Kind { get; set; } = ProjectKind.Unknown;
    public string? TargetFramework { get; set; }

    public List<string> PackageReferences { get; set; } = new();
    public List<string> ProjectReferences { get; set; } = new();

    // ---- .NET specifics ----
    public List<ControllerModel> Controllers { get; set; } = new();
    public List<string> Services { get; set; } = new();
    public List<string> Interfaces { get; set; } = new();
    public List<string> Entities { get; set; } = new();
    public List<string> DbContexts { get; set; } = new();
    public bool HasAuthentication { get; set; }

    // ---- Angular specifics (null for non-Angular projects) ----
    public AngularInfo? Angular { get; set; }
}

public class ControllerModel
{
    public string Name { get; set; } = string.Empty;
    public string? Route { get; set; }
    public List<string> Actions { get; set; } = new();
}

public class AngularInfo
{
    public List<string> Components { get; set; } = new();
    public List<string> Routes { get; set; } = new();
    public List<string> Dependencies { get; set; } = new();
}
