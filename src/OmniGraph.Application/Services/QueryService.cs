using OmniGraph.Application.Abstractions;

namespace OmniGraph.Application.Services;

public sealed class QueryService(
    INaturalLanguageQueryTranslator translator,
    IGraphRepository graphRepository)
{
    public async Task<(Models.NaturalLanguageQueryPlan Plan, Models.GraphQueryResult Result)> ExecuteNaturalLanguageAsync(
        string input,
        CancellationToken cancellationToken)
    {
        var plan = await translator.TranslateAsync(input, cancellationToken);
        var result = await graphRepository.SearchAsync(plan.Request, cancellationToken);
        return (plan, result with { PlannedQuery = plan.PlannedCypher });
    }
}
