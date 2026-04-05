namespace OmniGraph.Infrastructure.Options;

public sealed class KafkaOptions
{
    public const string SectionName = "Kafka";

    public string BootstrapServers { get; set; } = "localhost:9092";

    public string RawTopic { get; set; } = "omnigraph.ingestion.raw";

    public string ExtractedTopic { get; set; } = "omnigraph.graph.extracted";

    public string ConsumerGroup { get; set; } = "omnigraph-processing";
}
