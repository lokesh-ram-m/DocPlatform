using DocPlatform.Core.Models;
using Microsoft.CodeAnalysis;

namespace DocPlatform.Infrastructure.Extraction.Analyzers;

// One parsed .cs file plus a little context about where it lives.
public sealed class AnalyzedFile
{
    public required string Path { get; init; }
    public required string RelativeDir { get; init; }
    public required SyntaxTree Tree { get; init; }
    public bool InModelsFolder { get; init; }

    public SyntaxNode Root => Tree.GetRoot();
}

// Shared context passed to every analyzer. Holds the project, its parsed syntax trees,
// and a semantic Compilation so analyzers can resolve symbols (base types across files,
// interface implementations, generic arguments) — not just raw syntax.
public sealed class ProjectAnalysisContext
{
    public required ProjectModel Project { get; init; }
    public required IReadOnlyList<AnalyzedFile> Files { get; init; }

    // The project's Roslyn compilation (built from its syntax trees). Resolves the
    // project's OWN symbols even if external framework references aren't present.
    public Compilation? Compilation { get; init; }

    public SemanticModel? GetSemanticModel(SyntaxTree tree) => Compilation?.GetSemanticModel(tree);
}
