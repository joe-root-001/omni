using OmniGraph.Application.Models;
using OmniGraph.Contracts.Analysis;
using OmniGraph.Contracts.Graph;
using OmniGraph.Contracts.Ingestion;
using OmniGraph.Contracts.Query;
using OmniGraph.Domain.Models;

namespace OmniGraph.Api.Mappings;

public static class ContractMappings
{
    public static IngestionJobResponse ToResponse(this IngestionJob job) =>
        new(
            job.Id,
            job.CorrelationId,
            job.Status.ToString(),
            job.SubmittedAtUtc,
            job.ArtifactKind.ToString(),
            job.ArtifactUri);

    public static KnowledgeNodeResponse ToResponse(this KnowledgeEntity entity) =>
        new(
            entity.Id,
            entity.Type.ToString(),
            entity.DisplayName,
            entity.ExternalId,
            entity.RiskLevel.ToString(),
            entity.Version.ValidFromUtc,
            entity.Version.ValidToUtc,
            entity.Version.Revision,
            entity.Properties
                .Select(property => new PropertyEntryResponse(property.Key, property.Value))
                .ToArray());

    public static KnowledgeEdgeResponse ToResponse(this KnowledgeRelationship relationship) =>
        new(
            relationship.Id,
            relationship.FromEntityId,
            relationship.ToEntityId,
            relationship.Type.ToString(),
            relationship.Confidence,
            relationship.Explanation,
            relationship.Version.ValidFromUtc,
            relationship.Version.ValidToUtc,
            relationship.Version.Revision);

    public static GraphQueryResponse ToResponse(this GraphQueryResult result) =>
        new(
            result.PlannedQuery,
            result.Nodes.Select(ToResponse).ToArray(),
            result.Relationships.Select(ToResponse).ToArray());

    public static ImpactAnalysisResponse ToResponse(this ImpactAnalysisResult result) =>
        new(
            result.RootEntityId,
            result.Summary,
            result.AffectedEntities.Select(ToResponse).ToArray(),
            result.TraversedRelationships.Select(ToResponse).ToArray());

    public static ExplainRelationshipResponse ToResponse(this ExplainabilityResult result) =>
        new(
            result.RelationshipId,
            result.Explanation,
            result.SourceArtifact.Id,
            result.SourceArtifact.Uri,
            result.Evidence.Select(evidence =>
                new SourceEvidenceResponse(
                    evidence.ArtifactId,
                    evidence.Section,
                    evidence.Snippet,
                    evidence.ExtractionRule)).ToArray());

    public static NaturalLanguageQueryResponse ToResponse(
        this (NaturalLanguageQueryPlan Plan, GraphQueryResult Result) execution) =>
        new(
            execution.Plan.Intent,
            execution.Plan.StructuredFilter,
            execution.Result.ToResponse());
}
