using DocPlatform.Core.Models;

namespace DocPlatform.Core.Abstractions;

// Enriches each project in a repository with deep, DETERMINISTIC facts
// (controllers, services, entities, DbContexts, Angular routes/components).
// The LLM never does this — code does. (Rules #4 and #5.)
public interface IMetadataExtractor
{
    void Extract(RepositoryModel repository);
}
