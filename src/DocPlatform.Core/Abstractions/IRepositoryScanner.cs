using DocPlatform.Core.Models;

namespace DocPlatform.Core.Abstractions;

// Discovers what's in ONE repository's local working copy (no git operations).
// Produces a shallow RepositoryModel (projects, kinds, refs). Deep code facts
// are added later by IMetadataExtractor.
public interface IRepositoryScanner
{
    RepositoryModel Scan(string repositoryPath);
}
