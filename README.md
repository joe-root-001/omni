# OmniGraph AI

OmniGraph AI is a graph-first knowledge engine that replaces vector-only RAG with deterministic, explainable reasoning across Excel, PDFs, images, logs, and source code.

The repository is intentionally scaffolded around a production architecture:

- `src/OmniGraph.Api`: ASP.NET Core REST + GraphQL query plane
- `src/OmniGraph.Application`: orchestration and stable service abstractions
- `src/OmniGraph.Domain`: entities, relationships, provenance, and temporal versioning
- `src/OmniGraph.Infrastructure`: parser registry, repository adapters, query planning, and messaging
- `src/OmniGraph.Ingestion.Worker`: ingestion consumer shell for Kafka-driven intake
- `src/OmniGraph.Processing.Worker`: parser + graph projection worker shell
- `apps/omnigraph-web`: React/Vite product UI shell with graph canvas and workbench panels
- `infra`: local developer infrastructure definitions
- `docs`: architecture, schema, and delivery notes

## Quick start

```powershell
dotnet build .\OmniGraphAI.slnx
dotnet test .\OmniGraphAI.slnx
dotnet run --project .\src\OmniGraph.Api\OmniGraph.Api.csproj
```

Frontend:

```powershell
cd .\apps\omnigraph-web
npm install
npm run dev
```

Local infrastructure:

```powershell
docker compose -f .\infra\docker-compose.yml up -d
```

## Current scaffold notes

- The repository currently uses an in-memory graph repository to keep the first slice deterministic and runnable.
- Kafka, PostgreSQL, and Neo4j are modeled in configuration and local infrastructure, with adapter seams ready for production connectors.
- The architecture plan and GraphQL schema live in `docs/architecture.md` and `docs/graphql-schema.graphql`.
- Beginner setup and provider wiring are documented in `docs/getting-started.md` and `docs/configuration-reference.md`.
