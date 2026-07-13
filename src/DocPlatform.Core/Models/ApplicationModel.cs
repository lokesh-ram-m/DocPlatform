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

    // Categorized capabilities (auth, storage, messaging, cloud, …) detected from packages.
    public List<DetectedCapability> Capabilities { get; set; } = new();

    // Knowledge graph: how the projects relate (dependencies, calls, persistence).
    public List<Relationship> Relationships { get; set; } = new();

    // Call graph: how components wire up (controller -> service -> repository -> data),
    // aggregated from every project's ComponentDependencies.
    public List<Relationship> CallGraph { get; set; } = new();

    // Heuristically-detected architecture patterns (Clean, Layered, CQRS, ...).
    public List<DetectedPattern> ArchitecturePatterns { get; set; } = new();
}

public class RepositoryModel
{
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public bool HasReadme { get; set; }

    public List<ProjectModel> Projects { get; set; } = new();

    // Projects detected but not yet supported (React, Python, Java, …). Recorded and
    // logged rather than treated as errors; a future language extractor would claim these.
    public List<SkippedProject> SkippedProjects { get; set; } = new();
}

public class SkippedProject
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;   // e.g. "React", "Python", "Java (Maven)"
    public string Path { get; set; } = string.Empty;
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
    public List<string> CqrsRequests { get; set; } = new();   // MediatR commands/queries
    public bool HasAuthentication { get; set; }

    // Authentication & authorization detail.
    public List<string> AuthSchemes { get; set; } = new();    // JWT Bearer, Cookie, OIDC, Identity, …
    public List<string> AuthPolicies { get; set; } = new();   // named authorization policies (AddPolicy)
    public List<string> AuthRoles { get; set; } = new();      // roles referenced in [Authorize(Roles=…)]

    // interface -> implementation, from DI registrations (AddScoped/Singleton/Transient).
    public Dictionary<string, string> ServiceImplementations { get; set; } = new();

    // Component dependencies from constructor injection (this class -> what it uses).
    public List<Relationship> ComponentDependencies { get; set; } = new();

    // ---- Angular specifics (null for non-Angular projects) ----
    public AngularInfo? Angular { get; set; }
}

public class ControllerModel
{
    public string Name { get; set; } = string.Empty;
    public string? Route { get; set; }
    public List<string> Actions { get; set; } = new();

    // Controller-level authorization, e.g. "Authorize", "Authorize (Roles: Admin)", "AllowAnonymous".
    public string? Authorization { get; set; }
}

public class AngularInfo
{
    public List<string> Components { get; set; } = new();
    public List<string> Routes { get; set; } = new();
    public List<string> Dependencies { get; set; } = new();
}
