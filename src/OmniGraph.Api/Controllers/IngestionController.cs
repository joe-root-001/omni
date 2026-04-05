using Microsoft.AspNetCore.Mvc;
using OmniGraph.Api.Mappings;
using OmniGraph.Application.Commands;
using OmniGraph.Application.Services;
using OmniGraph.Contracts.Ingestion;

namespace OmniGraph.Api.Controllers;

[ApiController]
[Route("api/ingestion/jobs")]
public sealed class IngestionController(IngestionOrchestrator orchestrator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(IngestionJobResponse), StatusCodes.Status202Accepted)]
    public async Task<ActionResult<IngestionJobResponse>> SubmitAsync(
        [FromBody] SubmitIngestionRequest request,
        CancellationToken cancellationToken)
    {
        var job = await orchestrator.SubmitAsync(
            new SubmitIngestionCommand(
                request.ArtifactUri,
                request.ArtifactKind,
                request.RequestedBy,
                request.CorrelationId,
                request.Metadata),
            cancellationToken);

        return Accepted($"/api/ingestion/jobs/{job.Id}", job.ToResponse());
    }
}
