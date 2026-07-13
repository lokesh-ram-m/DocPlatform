using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DocPlatform.Infrastructure.Extraction.Analyzers;

// Detects domain entities from THREE signals (folder-independent where possible):
//   1. EF Core DbSet<T>  — T is definitively an entity
//   2. Base class        — inherits a known entity base (semantic: walks the chain)
//   3. Folder convention  — classes/records under Models/Entities/Domain (fallback)
// The DTO/response filter (IsLikelyEntity) is applied throughout.
public sealed class EntityAnalyzer : IProjectAnalyzer
{
    private static readonly HashSet<string> EntityBaseNames = new(StringComparer.Ordinal)
    {
        "Entity", "BaseEntity", "EntityBase", "AggregateRoot", "AuditableEntity",
        "AuditableEntityBase", "IdentityUser", "AggregateRootBase"
    };

    public void Analyze(ProjectAnalysisContext context)
    {
        foreach (AnalyzedFile file in context.Files)
        {
            SemanticModel? model = context.GetSemanticModel(file.Tree);

            // 1. DbSet<T> — the strongest EF signal, works regardless of folder.
            foreach (GenericNameSyntax g in file.Root.DescendantNodes().OfType<GenericNameSyntax>())
            {
                if (g.Identifier.Text != "DbSet" || g.TypeArgumentList.Arguments.Count != 1) continue;
                string entity = RoslynSyntax.LastSegment(g.TypeArgumentList.Arguments[0].ToString());
                Add(context, entity);
            }

            // 2 & 3. Class/record: base-class chain (semantic) or Models folder convention.
            foreach (TypeDeclarationSyntax t in file.Root.DescendantNodes().OfType<TypeDeclarationSyntax>())
            {
                if (t is not (ClassDeclarationSyntax or RecordDeclarationSyntax)) continue;
                string name = t.Identifier.Text;

                if (DerivesFromEntityBase(t, model) || file.InModelsFolder)
                    Add(context, name);
            }
        }
    }

    private static bool DerivesFromEntityBase(TypeDeclarationSyntax type, SemanticModel? model)
    {
        // Semantic: walk the full base-type chain (resolves multi-level, cross-file).
        if (model?.GetDeclaredSymbol(type) is INamedTypeSymbol symbol)
        {
            for (INamedTypeSymbol? b = symbol.BaseType; b is not null; b = b.BaseType)
                if (EntityBaseNames.Contains(b.Name))
                    return true;
        }

        // Syntactic fallback: check the immediate declared base type names.
        return RoslynSyntax.BaseTypeNames(type.BaseList).Any(EntityBaseNames.Contains);
    }

    private static void Add(ProjectAnalysisContext context, string name)
    {
        if (!ExtractionHelpers.IsLikelyEntity(name)) return;

        // Don't list the base classes themselves as entities.
        bool looksLikeBase = EntityBaseNames.Contains(name)
            || name.EndsWith("Base", StringComparison.Ordinal)
            || (name.StartsWith("Base", StringComparison.Ordinal) && name.EndsWith("Entity", StringComparison.Ordinal));
        if (looksLikeBase) return;

        context.Project.Entities.Add(name);
    }
}
