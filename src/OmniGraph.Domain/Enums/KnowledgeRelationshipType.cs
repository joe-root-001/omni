namespace OmniGraph.Domain.Enums;

public enum KnowledgeRelationshipType
{
    DependsOn = 1,
    Triggers = 2,
    Violates = 3,
    BelongsTo = 4,
    DerivedFrom = 5,
    References = 6,
    Affects = 7,
    GovernedBy = 8,
    ProducedBy = 9
}
