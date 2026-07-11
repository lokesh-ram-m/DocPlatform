namespace DocPlatform.AI.Prompts;

// Prompt templates. The AI receives only the STRUCTURED METADATA (JSON) and is
// told to explain — never to invent facts. Architecture wording stays tentative.
public static class DocumentationPrompts
{
    public const string System =
        "You are a senior software architect and technical writer producing engineering " +
        "documentation for a software APPLICATION. You are given STRUCTURED METADATA that was " +
        "extracted deterministically from the application's repositories.\n\n" +
        "STRICT RULES:\n" +
        "- Use ONLY facts present in the metadata. Never invent controllers, endpoints, classes, or technologies.\n" +
        "- The documentation describes the APPLICATION as a whole; repositories are just sources.\n" +
        "- For architecture, never claim certainty — use wording like 'Detected Patterns' or 'Likely Architecture'.\n" +
        "- Output clean, well-structured GitHub-flavored Markdown. No preamble, no code fences around the whole doc.\n" +
        "- Be concise and professional.";

    // Returns (fileName, userPrompt) for each mandatory document.
    public static IEnumerable<(string FileName, string UserPrompt)> MandatoryDocs(string appName, string metadataJson)
    {
        yield return ("overview.md",
            $"Generate `overview.md` for the application \"{appName}\".\n" +
            "Sections:\n" +
            $"# {appName}\n" +
            "## Summary — 2-4 sentences on what the application is and does (infer from projects/controllers).\n" +
            "## Repositories — bullet list; for each repository a one-line purpose.\n" +
            "## Technology Stack — from the metadata's technologies and package references.\n\n" +
            $"METADATA:\n```json\n{metadataJson}\n```");

        yield return ("architecture.md",
            "Generate `architecture.md`.\n" +
            "Sections:\n" +
            "# Architecture\n" +
            "## Detected Patterns — heuristic; hedge the wording (e.g. Repository Pattern, Layered, DI, Clean Architecture) based on evidence in the metadata (interfaces, services, project references).\n" +
            "## Backend Components — the .NET projects and their apparent responsibilities.\n" +
            "## Frontend — the Angular app, its components and routes.\n" +
            "## Data & Persistence — infer from DbContexts and data packages (e.g. Dapper, EF Core).\n\n" +
            $"METADATA:\n```json\n{metadataJson}\n```");

        yield return ("api.md",
            "Generate `api.md` documenting the HTTP API.\n" +
            "For each controller present in the metadata, add a `### <ControllerName>` heading with its base route, " +
            "then a Markdown table with columns: Method | Action | Endpoint. " +
            "Only include controllers and actions that appear in the metadata.\n\n" +
            $"METADATA:\n```json\n{metadataJson}\n```");
    }
}
