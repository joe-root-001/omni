# OmniGraph AI Configuration Reference

Use [omnigraph.settings.example.json](C:/Users/hp/Documents/New project/config/omnigraph.settings.example.json) as the full reference file.

## Main config files

- API runtime config: [appsettings.json](C:/Users/hp/Documents/New project/src/OmniGraph.Api/appsettings.json)
- API dev config: [appsettings.Development.json](C:/Users/hp/Documents/New project/src/OmniGraph.Api/appsettings.Development.json)
- Processing worker dev config: [appsettings.Development.json](C:/Users/hp/Documents/New project/src/OmniGraph.Processing.Worker/appsettings.Development.json)
- Full example config: [omnigraph.settings.example.json](C:/Users/hp/Documents/New project/config/omnigraph.settings.example.json)

## Important sections

### `Kafka`

Controls:

- broker address
- raw topic
- extracted topic
- consumer group names

### `PostgreSql`

Controls:

- relational database connection string
- job tracking and reporting storage

### `Neo4j`

Controls:

- graph database connection
- username
- password
- database name

### `AI`

Controls:

- which LLM provider is the default
- which provider is used for text
- which provider is used for vision
- which provider is used for embeddings

Possible provider names you can use:

- `Ollama`
- `OpenAI`
- `Anthropic`
- `AzureOpenAI`

### `ObjectStorage`

Controls where uploaded raw files are stored.

Possible first implementation choices:

- local file system
- S3
- MinIO
- Azure Blob

### `Uploads`

Controls:

- allowed extensions
- max file size

### `Parsers`

Controls:

- Excel parser behavior
- PDF OCR fallback
- image OCR and captioning
- log parsing behavior
- code AST options

## Provider routing examples

### Local-first

```json
"AI": {
  "DefaultProvider": "Ollama",
  "TextProvider": "Ollama",
  "VisionProvider": "Ollama",
  "EmbeddingProvider": "Ollama"
}
```

### Hybrid startup mode

```json
"AI": {
  "DefaultProvider": "Ollama",
  "TextProvider": "Ollama",
  "VisionProvider": "OpenAI",
  "EmbeddingProvider": "OpenAI"
}
```

### Cloud-first

```json
"AI": {
  "DefaultProvider": "OpenAI",
  "TextProvider": "OpenAI",
  "VisionProvider": "OpenAI",
  "EmbeddingProvider": "OpenAI"
}
```

## Security recommendation

Do not commit real secrets in JSON files.

Use one of these:

- `dotnet user-secrets`
- environment variables
- Azure Key Vault
- AWS Secrets Manager
- HashiCorp Vault

## Important honesty note

The config structure is ready now, but the current repository still needs actual LLM client implementations to call Ollama, OpenAI, Anthropic, or Azure OpenAI at runtime.
