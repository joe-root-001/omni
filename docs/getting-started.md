# OmniGraph AI Getting Started

This guide is written for someone who is starting from zero.

## 1. What this system does

OmniGraph AI accepts files such as:

- Excel files
- PDF files
- images
- logs
- source code files

Then it does this:

1. stores the raw file
2. detects the file type
3. runs the correct parser
4. extracts entities and relationships
5. writes the result into the knowledge graph
6. lets the user query the graph using REST, GraphQL, or natural language

## 2. What is already in this repository

You already have:

- ASP.NET Core API
- GraphQL endpoint
- ingestion worker
- processing worker
- graph domain model
- frontend UI shell
- local Docker infrastructure for Kafka, PostgreSQL, Neo4j, and Ollama

What is still a scaffold:

- the real production connectors for Neo4j
- the real Kafka producer/consumer code
- the real LLM API calls inside parser implementations

So this repo is the correct architecture and wiring foundation, but not yet the final full product.

## 3. The simplest way to run it locally

### Step 1: Start infrastructure

Run:

```powershell
docker compose -f .\infra\docker-compose.yml up -d
docker compose -f .\infra\docker-compose.ollama.yml up -d
```

This starts:

- Kafka on `localhost:9092`
- PostgreSQL on `localhost:5432`
- Neo4j on `localhost:7687`
- Kafka UI on `http://localhost:8080`
- Ollama on `http://localhost:11434`

### Step 2: Pull local Ollama models

Run:

```powershell
docker exec -it omnigraph-ollama ollama pull llama3.1:8b
docker exec -it omnigraph-ollama ollama pull llava:7b
docker exec -it omnigraph-ollama ollama pull nomic-embed-text
```

Use:

- `llama3.1:8b` for local chat/text reasoning
- `llava:7b` for image understanding
- `nomic-embed-text` only if you later enable optional embeddings

### Step 3: Configure the app

Open:

- `src/OmniGraph.Api/appsettings.Development.json`
- `src/OmniGraph.Processing.Worker/appsettings.Development.json`
- `config/omnigraph.settings.example.json`

Important:

- The `config/omnigraph.settings.example.json` file is your master example.
- The API and worker `appsettings.Development.json` files are the files the current code actually reads.

### Step 4: Choose your LLM mode

You have 3 common choices.

#### Option A: Local only with Ollama

Use this when you want everything local.

Set:

```json
"AI": {
  "DefaultProvider": "Ollama",
  "TextProvider": "Ollama",
  "VisionProvider": "Ollama"
}
```

Note:

- this is cheapest
- it is private
- quality may be lower than GPT for complex extraction

#### Option B: Hybrid

Use local for text and OpenAI for vision or better extraction.

Set:

```json
"AI": {
  "DefaultProvider": "Ollama",
  "TextProvider": "Ollama",
  "VisionProvider": "OpenAI",
  "EmbeddingProvider": "OpenAI"
}
```

This is usually the best startup choice.

#### Option C: Cloud only

Use OpenAI or Anthropic everywhere.

Set:

```json
"AI": {
  "DefaultProvider": "OpenAI",
  "TextProvider": "OpenAI",
  "VisionProvider": "OpenAI",
  "EmbeddingProvider": "OpenAI"
}
```

## 4. How to connect to OpenAI

Open:

- `src/OmniGraph.Api/appsettings.Development.json`
- `src/OmniGraph.Processing.Worker/appsettings.Development.json`

Fill:

```json
"OpenAI": {
  "Enabled": true,
  "BaseUrl": "https://api.openai.com/v1",
  "ApiKey": "your-real-openai-key",
  "ChatModel": "gpt-4.1-mini",
  "VisionModel": "gpt-4.1-mini",
  "EmbeddingModel": "text-embedding-3-small"
}
```

Better security:

- do not keep real keys in committed files
- use `dotnet user-secrets`
- or use environment variables and load them into configuration later

## 5. How to connect to a local LLM

The easiest local choice is Ollama.

Check Ollama:

```powershell
curl http://localhost:11434/api/tags
```

If it returns models, it is ready.

Your config should contain:

```json
"Ollama": {
  "BaseUrl": "http://localhost:11434",
  "ChatModel": "llama3.1:8b",
  "VisionModel": "llava:7b",
  "EmbeddingModel": "nomic-embed-text"
}
```

## 6. How to connect to Anthropic or Azure OpenAI

### Anthropic

Fill:

```json
"Anthropic": {
  "Enabled": true,
  "BaseUrl": "https://api.anthropic.com",
  "ApiKey": "your-anthropic-key",
  "ChatModel": "claude-3-5-sonnet-latest",
  "VisionModel": "claude-3-5-sonnet-latest"
}
```

### Azure OpenAI

Fill:

```json
"AzureOpenAI": {
  "Enabled": true,
  "Endpoint": "https://your-resource.openai.azure.com/",
  "ApiKey": "your-azure-openai-key",
  "ChatDeployment": "gpt-4.1-mini",
  "VisionDeployment": "gpt-4.1-mini",
  "EmbeddingDeployment": "text-embedding-3-small"
}
```

## 7. What happens when a user uploads a document

This is the most important flow.

### Step 1: User uploads a file

Examples:

- `transactions.xlsx`
- `policy-x.pdf`
- `payments.log`
- `RiskGuard.cs`
- `screenshot.png`

The API should save the raw file into object storage or a file storage location.

The current repository now includes a starter upload endpoint:

- `POST /api/uploads`

The starter version does this:

1. accepts a multipart form upload
2. infers the artifact kind from file extension
3. saves the file under `data/raw-artifacts`
4. creates an ingestion job
5. passes that job into the ingestion orchestration flow

Examples:

- local folder
- S3
- Azure Blob
- MinIO

### Step 2: API creates an ingestion job

The API creates a record like:

```json
{
  "jobId": "job-123",
  "artifactKind": "Pdf",
  "artifactUri": "s3://omnigraph/raw/policy-x.pdf",
  "status": "Queued"
}
```

### Step 3: API publishes a Kafka message

The API sends a message to Kafka topic:

- `omnigraph.ingestion.raw`

This message contains:

- file location
- file type
- checksum
- metadata
- correlation id

### Step 4: Ingestion worker receives the job

The ingestion worker:

- validates file metadata
- confirms the extension/type
- optionally virus scans or checks size
- routes the file to the next topic

### Step 5: Processing worker picks the file

The processing worker reads the artifact reference and decides which parser to use:

- Excel parser
- PDF parser
- image parser
- log parser
- code parser

### Step 6: Parser extracts structured information

Examples:

- Excel parser extracts rows, users, metrics, transactions
- PDF parser extracts policy clauses and thresholds
- image parser runs OCR and captioning
- log parser extracts events and correlation ids
- code parser builds AST and dependency relationships

### Step 7: LLM can help only where needed

Important design rule:

- deterministic parsing first
- LLM second

Example:

- Excel headers and rows should be parsed deterministically
- OCR text cleanup may use an LLM
- policy clause classification may use an LLM
- code AST should come from a parser, not from guessing with an LLM

### Step 8: Build graph entities and edges

Example result:

- `Transaction(TX-88421)`
- `Policy(Policy X)`
- `User(Ava Patel)`
- `Service(Payments Service)`
- `Transaction -> violates -> Policy`
- `Transaction -> belongs_to -> User`
- `Policy -> affects -> Service`

### Step 9: Save to graph store

Write to Neo4j:

- nodes
- relationships
- provenance
- timestamps
- revision numbers

Write to PostgreSQL:

- ingestion job status
- parser history
- audit information
- denormalized reporting views

### Step 10: User can query the result

The user can now:

- open graph UI
- click nodes
- ask a natural-language question
- run GraphQL queries
- run impact analysis
- ask “explain this connection”

## 8. How to run the current project

### Backend API

```powershell
dotnet run --project .\src\OmniGraph.Api\OmniGraph.Api.csproj
```

GraphQL endpoint:

- `http://localhost:5000/graphql`

Starter upload endpoint:

- `http://localhost:5000/api/uploads`

### Ingestion worker

```powershell
dotnet run --project .\src\OmniGraph.Ingestion.Worker\OmniGraph.Ingestion.Worker.csproj
```

### Processing worker

```powershell
dotnet run --project .\src\OmniGraph.Processing.Worker\OmniGraph.Processing.Worker.csproj
```

### Frontend

```powershell
cd .\apps\omnigraph-web
npm install
npm run dev
```

## 9. What is missing before this becomes fully real

To complete the system, build these next:

1. Replace local upload storage with S3, MinIO, or Azure Blob.
2. Real Kafka producers and consumers.
3. Real Neo4j repository implementation.
4. Real PostgreSQL persistence for jobs and parser runs.
5. Real LLM client implementations for Ollama/OpenAI/Anthropic.
6. Real parser logic for each file type.
7. Auth, tenancy, and audit controls.

## 10. Very practical recommendation

If you are new, build in this order:

1. make upload work for PDF and Excel only
2. store raw files locally first
3. push ingestion jobs to Kafka
4. process only PDF and Excel first
5. save extracted graph to Neo4j
6. query from GraphQL
7. only after that add images, logs, and code
8. only after that add LLM provider switching

That order is much easier than trying to build everything at once.
