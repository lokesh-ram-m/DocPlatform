using DocPlatform.Core.Models;
using Microsoft.CodeAnalysis;

namespace DocPlatform.Infrastructure.Extraction.Analyzers;

// One parsed .cs file plus a little context about where it lives.
public sealed class AnalyzedFile
{
    public required string Path { get; init; }
    public required string RelativeDir { get; init; }
    public required SyntaxNode Root { get; init; }
    public bool InModelsFolder { get; init; }
}

// Shared context passed to every analyzer. Holds the project being analyzed and its
// parsed syntax trees. (Stage 2 will add a Compilation + SemanticModel here.)
public sealed class ProjectAnalysisContext
{
    public required ProjectModel Project { get; init; }
    public required IReadOnlyList<AnalyzedFile> Files { get; init; }
}
