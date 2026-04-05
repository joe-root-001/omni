using System.Text.RegularExpressions;
using OmniGraph.Application.Abstractions;
using OmniGraph.Application.Models;
using OmniGraph.Domain.Enums;

namespace OmniGraph.Infrastructure.Querying;

public sealed partial class DeterministicNaturalLanguageQueryTranslator : INaturalLanguageQueryTranslator
{
    public Task<NaturalLanguageQueryPlan> TranslateAsync(string input, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var normalized = input.Trim();
        var entityType = normalized.Contains("transaction", StringComparison.OrdinalIgnoreCase)
            ? KnowledgeEntityType.Transaction
            : normalized.Contains("policy", StringComparison.OrdinalIgnoreCase)
                ? KnowledgeEntityType.Policy
                : (KnowledgeEntityType?)null;

        var minimumRisk = normalized.Contains("critical", StringComparison.OrdinalIgnoreCase)
            ? RiskLevel.Critical
            : normalized.Contains("risky", StringComparison.OrdinalIgnoreCase) ||
              normalized.Contains("risk", StringComparison.OrdinalIgnoreCase)
                ? RiskLevel.High
                : (RiskLevel?)null;

        var policyMatch = PolicyRegex().Match(normalized);
        var relatedPolicy = policyMatch.Success
            ? $"Policy {policyMatch.Groups["policy"].Value.Trim().ToUpperInvariant()}"
            : null;
        var text = entityType is null ? normalized : null;

        var request = new GraphQueryRequest(
            entityType,
            minimumRisk,
            text,
            relatedPolicy,
            Depth: 2,
            Limit: 25);

        var cypher = BuildCypher(request);
        var intent = relatedPolicy is not null ? "risk_and_policy_correlation" : "general_graph_search";
        var structuredFilter = $"entityType={entityType}; minimumRisk={minimumRisk}; relatedToPolicy={relatedPolicy ?? "n/a"}";

        return Task.FromResult(new NaturalLanguageQueryPlan(normalized, intent, structuredFilter, cypher, request));
    }

    private static string BuildCypher(GraphQueryRequest request)
    {
        var labels = request.EntityType?.ToString() ?? "Entity";
        var riskPredicate = request.MinimumRisk is null
            ? "true"
            : $"n.riskLevel IN ['{request.MinimumRisk}', 'Critical']";
        var policyPredicate = string.IsNullOrWhiteSpace(request.RelatedToPolicy)
            ? "true"
            : $"policy.displayName CONTAINS '{request.RelatedToPolicy}'";

        return
            $"MATCH (n:{labels}) OPTIONAL MATCH path=(n)-[*1..{request.Depth}]-(policy:Policy) WHERE {riskPredicate} AND {policyPredicate} RETURN n, path LIMIT {request.Limit}";
    }

    [GeneratedRegex("policy\\s+(?<policy>[a-z0-9-]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex PolicyRegex();
}
