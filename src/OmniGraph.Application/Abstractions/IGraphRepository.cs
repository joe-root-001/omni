using OmniGraph.Application.Models;
using OmniGraph.Domain.Models;

namespace OmniGraph.Application.Abstractions;

public interface IGraphRepository
{
    Task<KnowledgeEntity?> GetEntityAsync(string entityId, CancellationToken cancellationToken);

    Task<GraphQueryResult> SearchAsync(GraphQueryRequest request, CancellationToken cancellationToken);

    Task<GraphQueryResult> GetEntityNeighborhoodAsync(string entityId, int depth, CancellationToken cancellationToken);

    Task<ImpactAnalysisResult> AnalyzeImpactAsync(ImpactAnalysisRequest request, CancellationToken cancellationToken);

    Task UpsertExtractionAsync(ExtractionResult extractionResult, CancellationToken cancellationToken);
}
