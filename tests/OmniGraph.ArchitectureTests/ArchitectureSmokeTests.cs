using OmniGraph.Application.Models;
using OmniGraph.Domain.Enums;
using OmniGraph.Infrastructure.Persistence;
using OmniGraph.Infrastructure.Querying;

namespace OmniGraph.ArchitectureTests;

public sealed class ArchitectureSmokeTests
{
    [Fact]
    public async Task NaturalLanguageTranslatorBuildsPolicyAwareQueryPlan()
    {
        var translator = new DeterministicNaturalLanguageQueryTranslator();

        var plan = await translator.TranslateAsync("Show risky transactions related to policy x", CancellationToken.None);

        Assert.Equal(KnowledgeEntityType.Transaction, plan.Request.EntityType);
        Assert.Equal(RiskLevel.High, plan.Request.MinimumRisk);
        Assert.Equal("Policy X", plan.Request.RelatedToPolicy);
        Assert.Contains("MATCH", plan.PlannedCypher, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task RepositoryReturnsCriticalTransactionForPolicyQuery()
    {
        var repository = new InMemoryGraphRepository(new InMemoryKnowledgeStore());

        var result = await repository.SearchAsync(
            new GraphQueryRequest(
                KnowledgeEntityType.Transaction,
                RiskLevel.High,
                "Transaction",
                "Policy X",
                2,
                10),
            CancellationToken.None);

        Assert.Contains(result.Nodes, node => node.Id == "transaction-88421");
        Assert.Contains(result.Nodes, node => node.Id == "policy-x");
        Assert.Contains(result.Relationships, edge => edge.Id == "rel-transaction-violates-policy");
    }

    [Fact]
    public async Task ImpactAnalysisTraversesConnectedOperationalEntities()
    {
        var repository = new InMemoryGraphRepository(new InMemoryKnowledgeStore());

        var impact = await repository.AnalyzeImpactAsync(new ImpactAnalysisRequest("policy-x", 3), CancellationToken.None);

        Assert.Equal("policy-x", impact.RootEntityId);
        Assert.Contains(impact.AffectedEntities, entity => entity.Id == "service-payments");
        Assert.Contains(impact.AffectedEntities, entity => entity.Id == "transaction-88421");
    }
}
