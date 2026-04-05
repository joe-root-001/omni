using Microsoft.AspNetCore.Mvc;
using OmniGraph.Api.Mappings;
using OmniGraph.Application.Services;
using OmniGraph.Contracts.Query;

namespace OmniGraph.Api.Controllers;

[ApiController]
[Route("api/query")]
public sealed class QueryController(QueryService queryService) : ControllerBase
{
    [HttpPost("natural-language")]
    [ProducesResponseType(typeof(NaturalLanguageQueryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<NaturalLanguageQueryResponse>> ExecuteNaturalLanguageAsync(
        [FromBody] NaturalLanguageQueryRequest request,
        CancellationToken cancellationToken)
    {
        var execution = await queryService.ExecuteNaturalLanguageAsync(request.Input, cancellationToken);
        return Ok(execution.ToResponse());
    }
}
