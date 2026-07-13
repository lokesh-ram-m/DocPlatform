namespace DocPlatform.AI.Prompts;

// One document to generate: which group it belongs to, its file, order, and the prompt.
public record DocSpec(string Group, string FileName, int Order, string Instructions);

// The documentation plan: a PRODUCT specification (for PM/BU/end users) and a deep
// TECHNICAL specification (for engineers). The AI explains the metadata — never invents.
public static class DocumentationPrompts
{
    public const string ProductGroup = "Product Specification";
    public const string TechnicalGroup = "Technical Specification";

    public const string System =
        "You generate engineering documentation for a software APPLICATION from STRUCTURED METADATA " +
        "that was extracted deterministically from its repositories.\n\n" +
        "You produce two kinds of documents:\n" +
        "- PRODUCT SPECIFICATION — for Product Managers, Business Units and End Users. Plain, non-technical " +
        "language focused on what the product does and the value it delivers.\n" +
        "- TECHNICAL SPECIFICATION — for engineers. Precise, thorough, in-depth.\n\n" +
        "STRICT RULES:\n" +
        "- Use ONLY facts present in the metadata (projects, controllers, entities, technologies, capabilities). " +
        "Never invent endpoints, technologies, cloud services, queues, or databases.\n" +
        "- If a concern is not present in the metadata, say so explicitly (e.g. 'No message queue was detected').\n" +
        "- The documentation describes the APPLICATION as a whole; repositories are just sources.\n" +
        "- For architecture, hedge — 'Detected Patterns', 'Likely'.\n" +
        "- Output clean GitHub-flavored Markdown. No preamble, start with the top-level '# ' heading.";

    public static IEnumerable<DocSpec> Plan(string appName, string metadataJson)
    {
        string meta = $"\n\nAPPLICATION METADATA (the only source of truth):\n```json\n{metadataJson}\n```";

        // ---------- PRODUCT SPECIFICATION ----------
        yield return new(ProductGroup, "overview.md", 1,
            $"# {appName} — Product Overview\n" +
            "Audience: Product Managers, Business Units, End Users (non-technical).\n" +
            "Sections: ## What is " + appName + "?  ## The Problem It Solves  ## Who It's For (the roles/personas that benefit)  ## Key Value.\n" +
            "Infer the product's purpose from its controllers, entities and features. Avoid technical jargon." + meta);

        yield return new(ProductGroup, "features.md", 2,
            "# Features\n" +
            "Audience: PM / Business / End Users. Describe the product's capabilities in BUSINESS terms, grouped by " +
            "functional area (infer areas from controllers/entities, e.g. Task Management, Projects, Users & Access). " +
            "For each capability: what it lets a user do and why it helps them. No code, no endpoints." + meta);

        yield return new(ProductGroup, "use-cases.md", 3,
            "# Use Cases\n" +
            "Audience: PM / Business / End Users. Write 3-5 concrete day-to-day scenarios as short narratives showing how " +
            "a Product Manager, a Business Unit, and an End User would use the product. Keep it practical and non-technical." + meta);

        // ---------- TECHNICAL SPECIFICATION ----------
        yield return new(TechnicalGroup, "architecture.md", 1,
            "# Architecture\n" +
            "Sections: ## Detected Patterns — list the `architecturePatterns` from the metadata with their evidence; " +
            "keep the wording tentative ('detected', 'likely'). " +
            "## Solution Structure (each repository and its projects, with responsibilities). " +
            "## Component Responsibilities. ## How the Pieces Fit Together — describe the flow using the " +
            "`relationships` array in the metadata (which project depends on which, frontend→API calls, persistence to the database). " +
            "Do NOT draw a diagram (one is added automatically); describe the relationships in prose." + meta);

        yield return new(TechnicalGroup, "technology-stack.md", 2,
            "# Technology Stack\n" +
            "Be thorough. Present a Markdown table grouped by concern: Concern | Technology | Details. " +
            "Cover runtime & frameworks (.NET target framework, ASP.NET Core, Angular), data access, database, authentication, " +
            "API documentation, AI/LLM, and any cross-cutting libraries. Derive strictly from technologies, capabilities and package references." + meta);

        yield return new(TechnicalGroup, "infrastructure-and-cloud.md", 3,
            "# Infrastructure & Cloud\n" +
            "Sections: ## Hosting & Runtime (how each part runs — ASP.NET Core service, Angular SPA, containerizable). " +
            "## Cloud Services (list detected cloud capabilities; if none are detected, state that clearly and note it is host-agnostic/containerizable). " +
            "## Configuration (config sources such as appsettings/environment variables, if evident). " +
            "## Deployment Considerations (grounded, hedged). Be detailed but never invent cloud services not in the metadata." + meta);

        yield return new(TechnicalGroup, "data-storage-and-messaging.md", 4,
            "# Data, Storage & Messaging\n" +
            "Sections: ## Databases (from Database capabilities, DbContexts and data packages). " +
            "## Data Access Approach (e.g. Dapper vs EF Core). ## Entities / Domain Model (from entities). " +
            "## Caching (if detected, else 'none detected'). " +
            "## Messaging & Queuing (list detected; if none, state 'No message queue or event bus was detected in the current metadata'). Ground every claim." + meta);

        yield return new(TechnicalGroup, "security-and-authentication.md", 5,
            "# Security & Authentication\n" +
            "Sections: ## Authentication — the scheme(s) from `authSchemes` (e.g. JWT Bearer, Cookie, OpenID Connect, ASP.NET Core Identity). " +
            "## Authorization — the named policies (`authPolicies`), roles (`authRoles`), and which controllers are protected " +
            "(from each controller's `authorization` field: Authorize / AllowAnonymous). " +
            "## Secret & Password Handling (e.g. BCrypt if detected in capabilities). " +
            "## Security-relevant Libraries. If a concern is absent, say so plainly." + meta);

        yield return new(TechnicalGroup, "api-reference.md", 6,
            "# API Reference\n" +
            "For each API project and each controller in the metadata, add a `### <ControllerName>` heading with its base route, " +
            "then a Markdown table: Method | Action | Endpoint. Only include controllers/actions present in the metadata." + meta);
    }
}
