using DocPlatform.Core.Models;

namespace DocPlatform.Infrastructure.Extraction.Angular;

// One TypeScript source file (read as text — there's no Roslyn for TS, so the Angular
// analyzers work heuristically on the content, like the Heuristic .NET extractor).
public sealed class AngularFile
{
    public required string Path { get; init; }
    public required string Name { get; init; }     // file name, e.g. "task-list.component.ts"
    public required string Content { get; init; }
}

// Shared context for the Angular analyzers: the project and its .ts source files.
public sealed class AngularAnalysisContext
{
    public required ProjectModel Project { get; init; }
    public required IReadOnlyList<AngularFile> Files { get; init; }

    public AngularInfo Angular => Project.Angular ??= new AngularInfo();
}
