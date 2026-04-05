namespace OmniGraph.Infrastructure.Options;

public sealed class AIOptions
{
    public const string SectionName = "AI";

    public string DefaultProvider { get; set; } = "Ollama";

    public string TextProvider { get; set; } = "Ollama";

    public string VisionProvider { get; set; } = "OpenAI";

    public string EmbeddingProvider { get; set; } = "OpenAI";

    public bool EnableVisionModels { get; set; } = true;

    public bool EnableOptionalEmbeddings { get; set; }

    public string ModelName { get; set; } = "gpt-4.1-mini";

    public OllamaProviderOptions Ollama { get; set; } = new();

    public OpenAIProviderOptions OpenAI { get; set; } = new();

    public AnthropicProviderOptions Anthropic { get; set; } = new();

    public AzureOpenAIProviderOptions AzureOpenAI { get; set; } = new();
}

public sealed class OllamaProviderOptions
{
    public string BaseUrl { get; set; } = "http://localhost:11434";

    public string ChatModel { get; set; } = "llama3.1:8b";

    public string VisionModel { get; set; } = "llava:7b";

    public string EmbeddingModel { get; set; } = "nomic-embed-text";
}

public sealed class OpenAIProviderOptions
{
    public bool Enabled { get; set; } = true;

    public string BaseUrl { get; set; } = "https://api.openai.com/v1";

    public string ApiKey { get; set; } = "";

    public string ChatModel { get; set; } = "gpt-4.1-mini";

    public string VisionModel { get; set; } = "gpt-4.1-mini";

    public string EmbeddingModel { get; set; } = "text-embedding-3-small";
}

public sealed class AnthropicProviderOptions
{
    public bool Enabled { get; set; }

    public string BaseUrl { get; set; } = "https://api.anthropic.com";

    public string ApiKey { get; set; } = "";

    public string ChatModel { get; set; } = "claude-3-5-sonnet-latest";

    public string VisionModel { get; set; } = "claude-3-5-sonnet-latest";
}

public sealed class AzureOpenAIProviderOptions
{
    public bool Enabled { get; set; }

    public string Endpoint { get; set; } = "";

    public string ApiKey { get; set; } = "";

    public string ChatDeployment { get; set; } = "gpt-4.1-mini";

    public string VisionDeployment { get; set; } = "gpt-4.1-mini";

    public string EmbeddingDeployment { get; set; } = "text-embedding-3-small";
}
