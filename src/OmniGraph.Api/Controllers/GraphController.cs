using Microsoft.AspNetCore.Mvc;
using OmniGraph.Api.Mappings;
using OmniGraph.Application.Models;
using OmniGraph.Application.Services;
using OmniGraph.Contracts.Analysis;
using OmniGraph.Contracts.Graph;

namespace OmniGraph.Api.Controllers;

[ApiController]
[Route("api/graph")]
public sealed class GraphController(GraphExplorationService explorationService) : ControllerBase
{
    [HttpGet("entities/{entityId}")]
    [ProducesResponseType(typeof(EntityDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EntityDetailResponse>> GetEntityAsync(
        string entityId,
        [FromQuery] int depth = 2,
        CancellationToken cancellationToken = default)
    {
        var entity = await explorationService.GetEntityAsync(entityId, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        var graph = await explorationService.GetEntityNeighborhoodAsync(entityId, depth, cancellationToken);
        var relationships = graph.Relationships
            .Where(relationship => relationship.FromEntityId == entityId || relationship.ToEntityId == entityId)
            .Select(relationship => relationship.ToResponse())
            .ToArray();

        return Ok(new EntityDetailResponse(entity.ToResponse(), relationships));
    }

    [HttpGet("entities/{entityId}/neighbors")]
    [ProducesResponseType(typeof(GraphQueryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GraphQueryResponse>> GetNeighborsAsync(
        string entityId,
        [FromQuery] int depth = 2,
        CancellationToken cancellationToken = default)
    {
        var result = await explorationService.GetEntityNeighborhoodAsync(entityId, depth, cancellationToken);
        return Ok(result.ToResponse());
    }

    [HttpPost("impact")]
    [ProducesResponseType(typeof(ImpactAnalysisResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ImpactAnalysisResponse>> AnalyzeImpactAsync(
        [FromBody] ImpactAnalysisRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await explorationService.AnalyzeImpactAsync(
            new ImpactAnalysisRequest(request.EntityId, request.Depth),
            cancellationToken);

        return Ok(result.ToResponse());
    }

    [HttpGet("relationships/{relationshipId}/explain")]
    [ProducesResponseType(typeof(ExplainRelationshipResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ExplainRelationshipResponse>> ExplainRelationshipAsync(
        string relationshipId,
        CancellationToken cancellationToken)
    {
        var result = await explorationService.ExplainRelationshipAsync(relationshipId, cancellationToken);
        return result is null ? NotFound() : Ok(result.ToResponse());
    }
}
