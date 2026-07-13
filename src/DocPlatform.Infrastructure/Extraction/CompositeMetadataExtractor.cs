using DocPlatform.Core.Abstractions;
using DocPlatform.Core.Models;

namespace DocPlatform.Infrastructure.Extraction;

// Routes each project to the right language extractor: .NET projects to the chosen
// .NET extractor (Roslyn/Heuristic), Angular projects to the Angular extractor.
// Each underlying extractor only touches the project kinds it supports.
public class CompositeMetadataExtractor : IMetadataExtractor
{
    private readonly IMetadataExtractor _dotnet;
    private readonly IMetadataExtractor _angular;

    public CompositeMetadataExtractor(IMetadataExtractor dotnet, IMetadataExtractor angular)
    {
        _dotnet = dotnet;
        _angular = angular;
    }

    public void Extract(RepositoryModel repository)
    {
        _dotnet.Extract(repository);
        _angular.Extract(repository);
    }
}
