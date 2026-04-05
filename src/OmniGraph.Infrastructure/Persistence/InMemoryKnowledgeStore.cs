using OmniGraph.Domain.Enums;
using OmniGraph.Domain.Models;

namespace OmniGraph.Infrastructure.Persistence;

public sealed class InMemoryKnowledgeStore
{
    private readonly object _gate = new();
    private readonly Dictionary<string, SourceArtifact> _artifacts;
    private readonly Dictionary<string, KnowledgeEntity> _entities;
    private readonly Dictionary<string, KnowledgeRelationship> _relationships;

    public InMemoryKnowledgeStore()
    {
        (_artifacts, _entities, _relationships) = Seed();
    }

    public IReadOnlyCollection<SourceArtifact> GetArtifacts()
    {
        lock (_gate)
        {
            return _artifacts.Values.ToArray();
        }
    }

    public SourceArtifact? GetArtifact(string artifactId)
    {
        lock (_gate)
        {
            return _artifacts.TryGetValue(artifactId, out var artifact) ? artifact : null;
        }
    }

    public IReadOnlyCollection<KnowledgeEntity> GetEntities()
    {
        lock (_gate)
        {
            return _entities.Values.ToArray();
        }
    }

    public KnowledgeEntity? GetEntity(string entityId)
    {
        lock (_gate)
        {
            return _entities.TryGetValue(entityId, out var entity) ? entity : null;
        }
    }

    public IReadOnlyCollection<KnowledgeRelationship> GetRelationships()
    {
        lock (_gate)
        {
            return _relationships.Values.ToArray();
        }
    }

    public KnowledgeRelationship? GetRelationship(string relationshipId)
    {
        lock (_gate)
        {
            return _relationships.TryGetValue(relationshipId, out var relationship) ? relationship : null;
        }
    }

    public void Upsert(ExtractionResult extractionResult)
    {
        lock (_gate)
        {
            _artifacts[extractionResult.Artifact.Id] = extractionResult.Artifact;

            foreach (var entity in extractionResult.Entities)
            {
                _entities[entity.Id] = entity;
            }

            foreach (var relationship in extractionResult.Relationships)
            {
                _relationships[relationship.Id] = relationship;
            }
        }
    }

    private static (
        Dictionary<string, SourceArtifact> Artifacts,
        Dictionary<string, KnowledgeEntity> Entities,
        Dictionary<string, KnowledgeRelationship> Relationships) Seed()
    {
        var capturedAt = new DateTimeOffset(2026, 4, 1, 8, 30, 0, TimeSpan.Zero);
        var version = new VersionStamp(capturedAt, capturedAt, null, 3);

        var artifacts = new[]
        {
            new SourceArtifact(
                "artifact-policy-x-pdf",
                ArtifactKind.Pdf,
                "s3://omnigraph/raw/policies/policy-x-v3.pdf",
                "sha256-policy-x",
                capturedAt,
                new Dictionary<string, string>
                {
                    ["owner"] = "compliance",
                    ["documentVersion"] = "3"
                }),
            new SourceArtifact(
                "artifact-transactions-excel",
                ArtifactKind.Excel,
                "s3://omnigraph/raw/finance/high-risk-transactions.xlsx",
                "sha256-transactions",
                capturedAt,
                new Dictionary<string, string>
                {
                    ["sheet"] = "Q2-Review",
                    ["dataset"] = "high-risk-transactions"
                }),
            new SourceArtifact(
                "artifact-payments-log",
                ArtifactKind.Log,
                "s3://omnigraph/raw/logs/payments-service.log",
                "sha256-payments-log",
                capturedAt,
                new Dictionary<string, string>
                {
                    ["service"] = "payments-service",
                    ["environment"] = "prod"
                }),
            new SourceArtifact(
                "artifact-payments-code",
                ArtifactKind.Code,
                "git://omnigraph/services/payments-service/RiskGuard.cs",
                "sha256-payments-code",
                capturedAt,
                new Dictionary<string, string>
                {
                    ["repository"] = "payments-service",
                    ["language"] = "csharp"
                })
        }.ToDictionary(x => x.Id, x => x);

        var entities = new[]
        {
            new KnowledgeEntity(
                "policy-x",
                KnowledgeEntityType.Policy,
                "Policy X",
                "POL-001",
                RiskLevel.High,
                version,
                "artifact-policy-x-pdf",
                new Dictionary<string, string>
                {
                    ["category"] = "Transaction Monitoring",
                    ["changeWindow"] = "2026-Q2",
                    ["owner"] = "Compliance"
                }),
            new KnowledgeEntity(
                "transaction-88421",
                KnowledgeEntityType.Transaction,
                "Transaction 88421",
                "TX-88421",
                RiskLevel.Critical,
                version,
                "artifact-transactions-excel",
                new Dictionary<string, string>
                {
                    ["amount"] = "175000",
                    ["currency"] = "USD",
                    ["region"] = "APAC"
                }),
            new KnowledgeEntity(
                "user-1007",
                KnowledgeEntityType.User,
                "Ava Patel",
                "USR-1007",
                RiskLevel.Medium,
                version,
                "artifact-transactions-excel",
                new Dictionary<string, string>
                {
                    ["department"] = "Treasury",
                    ["country"] = "IN"
                }),
            new KnowledgeEntity(
                "service-payments",
                KnowledgeEntityType.Service,
                "Payments Service",
                "svc-payments",
                RiskLevel.High,
                version,
                "artifact-payments-log",
                new Dictionary<string, string>
                {
                    ["environment"] = "prod",
                    ["tier"] = "critical"
                }),
            new KnowledgeEntity(
                "event-velocity-breach",
                KnowledgeEntityType.Event,
                "Velocity Threshold Breach",
                "evt-velocity-breach",
                RiskLevel.High,
                version,
                "artifact-payments-log",
                new Dictionary<string, string>
                {
                    ["eventCode"] = "PAY-429",
                    ["severity"] = "high"
                }),
            new KnowledgeEntity(
                "function-risk-evaluator",
                KnowledgeEntityType.Function,
                "RiskGuard.EvaluateTransaction",
                "RiskGuard::EvaluateTransaction",
                RiskLevel.Medium,
                version,
                "artifact-payments-code",
                new Dictionary<string, string>
                {
                    ["language"] = "C#",
                    ["module"] = "RiskGuard"
                })
        }.ToDictionary(x => x.Id, x => x);

        var relationships = new[]
        {
            new KnowledgeRelationship(
                "rel-transaction-violates-policy",
                "transaction-88421",
                "policy-x",
                KnowledgeRelationshipType.Violates,
                0.98,
                "Transaction 88421 breaches Policy X daily volume limits.",
                version,
                "artifact-policy-x-pdf",
                new List<SourceEvidence>
                {
                    new("artifact-policy-x-pdf", "Policy Section 4.2", "Transactions above 100k require manual review.", "policy-threshold-rule"),
                    new("artifact-transactions-excel", "Q2-Review!A28:G28", "TX-88421 amount=175000 flagged=TRUE", "excel-threshold-parser")
                }),
            new KnowledgeRelationship(
                "rel-transaction-belongs-user",
                "transaction-88421",
                "user-1007",
                KnowledgeRelationshipType.BelongsTo,
                0.94,
                "The transaction owner in the finance workbook is Ava Patel.",
                version,
                "artifact-transactions-excel",
                new List<SourceEvidence>
                {
                    new("artifact-transactions-excel", "Q2-Review!A28:G28", "owner=USR-1007", "excel-row-linker")
                }),
            new KnowledgeRelationship(
                "rel-event-triggers-transaction",
                "event-velocity-breach",
                "transaction-88421",
                KnowledgeRelationshipType.Triggers,
                0.92,
                "The payments service emitted a breach event moments before the risky transaction was committed.",
                version,
                "artifact-payments-log",
                new List<SourceEvidence>
                {
                    new("artifact-payments-log", "line 8831", "PAY-429 threshold breach for TX-88421", "log-event-parser")
                }),
            new KnowledgeRelationship(
                "rel-service-produces-event",
                "service-payments",
                "event-velocity-breach",
                KnowledgeRelationshipType.ProducedBy,
                0.99,
                "The payments service emitted the breach event.",
                version,
                "artifact-payments-log",
                new List<SourceEvidence>
                {
                    new("artifact-payments-log", "line 8831", "service=payments-service event=PAY-429", "log-service-linker")
                }),
            new KnowledgeRelationship(
                "rel-service-depends-function",
                "service-payments",
                "function-risk-evaluator",
                KnowledgeRelationshipType.DependsOn,
                0.95,
                "Payments Service depends on the RiskGuard evaluator implementation.",
                version,
                "artifact-payments-code",
                new List<SourceEvidence>
                {
                    new("artifact-payments-code", "RiskGuard.cs:18-44", "public RiskDecision EvaluateTransaction(...)", "ast-call-graph")
                }),
            new KnowledgeRelationship(
                "rel-policy-affects-service",
                "policy-x",
                "service-payments",
                KnowledgeRelationshipType.Affects,
                0.89,
                "Policy X configuration is enforced by Payments Service.",
                version,
                "artifact-payments-code",
                new List<SourceEvidence>
                {
                    new("artifact-payments-code", "RiskGuard.cs:31", "if (policy.Name == \"Policy X\")", "ast-constant-matcher")
                })
        }.ToDictionary(x => x.Id, x => x);

        return (artifacts, entities, relationships);
    }
}
