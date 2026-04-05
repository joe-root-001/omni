using HotChocolate;
using OmniGraph.Api.Mappings;
using OmniGraph.Application.Abstractions;
using OmniGraph.Application.Models;
using OmniGraph.Application.Services;
using OmniGraph.Contracts.Analysis;
using OmniGraph.Contracts.Graph;
using OmniGraph.Contracts.Query;
using OmniGraph.Domain.Enums;

namespace OmniGraph.Api.GraphQL;

public sealed class GraphQuery
{
    public async Task<KnowledgeNodeResponse?> EntityAsync(
        string entityId,
        [Service] GraphExplorationService explorationService,
        CancellationToken cancellationToken)
    {
        var entity = await explorationService.GetEntityAsync(entityId, cancellationToken);
        return entity?.ToResponse();
    }

    public async Task<GraphQueryResponse> SearchAsync(
        [Service] IGraphRepository graphRepository,
        CancellationToken cancellationToken,
        KnowledgeEntityType? entityType = null,
        RiskLevel? minimumRisk = null,
        string? text = null,
        string? relatedToPolicy = null,
        int depth = 2,
        int limit = 25)
    {
        var result = await graphRepository.SearchAsync(
            new GraphQueryRequest(entityType, minimumRisk, text, relatedToPolicy, depth, limit),
            cancellationToken);

        return result.ToResponse();
    }

    public async Task<NaturalLanguageQueryResponse> NaturalLanguageAsync(
        string input,
        [Service] QueryService queryService,
        CancellationToken cancellationToken)
    {
        var execution = await queryService.ExecuteNaturalLanguageAsync(input, cancellationToken);
        return execution.ToResponse();
    }

    public async Task<ImpactAnalysisResponse> ImpactAsync(
        string entityId,
        [Service] GraphExplorationService explorationService,
        CancellationToken cancellationToken,
        int depth = 3)
    {
        var result = await explorationService.AnalyzeImpactAsync(new ImpactAnalysisRequest(entityId, depth), cancellationToken);
        return result.ToResponse();
    }

    public async Task<ExplainRelationshipResponse?> ExplainRelationshipAsync(
        string relationshipId,
        [Service] GraphExplorationService explorationService,
        CancellationToken cancellationToken)
    {
        var result = await explorationService.ExplainRelationshipAsync(relationshipId, cancellationToken);
        return result?.ToResponse();
    }
}
