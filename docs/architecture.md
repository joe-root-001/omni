# OmniGraph AI Architecture

## 1. Product stance

OmniGraph AI is not a vector-first retrieval system. It is a graph-first knowledge platform that normalizes heterogeneous artifacts into deterministic entities, relationships, provenance, and temporal versions. Embeddings are optional and secondary, used only to improve recall for low-confidence matching or image caption enrichment.

## 2. High-level architecture diagram

```text
                           +------------------------------+
                           |        OmniGraph Web         |
                           | React + Graph Canvas + Chat  |
                           +--------------+---------------+
                                          |
                             REST + GraphQL Query Plane
                                          |
                   +----------------------+----------------------+
                   |      ASP.NET Core OmniGraph API            |
                   |  auth, query planning, explainability      |
                   +-----------+----------------+---------------+
                               |                |
                       PostgreSQL read models   Neo4j graph traversal
                               |                |
                   +-----------+----------------+---------------+
                   |       Application + Infrastructure         |
                   | orchestration, parsers, NL translator      |
                   +-----------+----------------+---------------+
                               |                |
                    Kafka extracted topic       Kafka raw topic
                               |                |
                     +---------+--+      +------+----------------+
                     | Processing  |      | Ingestion Worker     |
                     | Worker      |      | validation + routing |
                     +------+------+      +-----------+----------+
                            |                         |
                    Parser registry                   |
       +-----------+----------+-----------+----------+-----------+
       | Excel     | PDF      | Image/OCR | Logs     | Code/AST  |
       +-----------+----------+-----------+----------+-----------+
                            |
                   Raw artifact storage / object store
```

## 3. Backend folder structure

```text
src/
  OmniGraph.Api/
    Controllers/
    GraphQL/
    Mappings/
  OmniGraph.Application/
    Abstractions/
    Commands/
    Models/
    Services/
    DependencyInjection/
  OmniGraph.Contracts/
    Analysis/
    Graph/
    Ingestion/
    Query/
  OmniGraph.Domain/
    Enums/
    Models/
  OmniGraph.Infrastructure/
    DependencyInjection/
    Explainability/
    Messaging/
    Options/
    Parsing/
    Persistence/
    Querying/
  OmniGraph.Ingestion.Worker/
  OmniGraph.Processing.Worker/
tests/
  OmniGraph.ArchitectureTests/
apps/
  omnigraph-web/
infra/
docs/
```

## 4. Knowledge model

### Core entities

| Entity | Purpose | Source examples |
|---|---|---|
| `User` | people, owners, approvers | Excel rows, logs, HR feeds |
| `Transaction` | financial or business events | Excel, CSV, logs |
| `Policy` | governance and compliance rules | PDFs, docs |
| `Service` | deployable systems | logs, code, CMDB |
| `Event` | runtime signals and alerts | logs, monitoring exports |
| `Function` | code-level units and call graph nodes | AST parsers |
| `Document` | OCR/image-derived documents | screenshots, scans |
| `Metric` | KPI or aggregate measurements | Excel, BI exports |

### Core relationships

| Relationship | Meaning |
|---|---|
| `depends_on` | service/function or component dependency |
| `triggers` | one event causes another action |
| `violates` | entity breaches a policy or control |
| `belongs_to` | ownership or membership |
| `derived_from` | extracted/normalized from source |
| `references` | document or code reference |
| `affects` | impact propagation edge |
| `produced_by` | event/source generation |

### Temporal versioning model

Every entity and relationship carries:

- `ObservedAtUtc`
- `ValidFromUtc`
- `ValidToUtc`
- `Revision`
- `SourceArtifactId`

This enables:

- graph snapshots at any point in time
- audit trails for policy or code drift
- comparison between revisions
- impact analysis on future-dated changes

## 5. Parser and extraction design

### Excel

- infer sheet schema
- detect headers, types, and primary row identity
- extract rows as `Transaction`, `User`, `Metric`, or `Table`
- create row-to-owner and row-to-policy edges

### PDF

- extract text blocks and section hierarchy
- identify policy clauses, exceptions, thresholds, owners
- emit `Policy`, `Document`, and `references` edges

### Images

- OCR text extraction
- captioning and layout understanding
- detect forms, screenshots, or scanned evidence
- attach confidence and provenance snippets

### Logs

- parse timestamp, level, service, event code, correlation id
- normalize into `Event` nodes and causal `triggers` edges
- attach runtime provenance for explainability

### Code

- parse AST
- extract `Function`, `Service`, import/call dependencies
- detect constants or policy references in source
- create `depends_on`, `references`, and `affects` edges

## 6. Kafka pipeline design

### Topics

| Topic | Producer | Consumer | Payload |
|---|---|---|---|
| `omnigraph.ingestion.raw` | API / upload service | ingestion worker | artifact metadata, URI, checksum |
| `omnigraph.ingestion.validated` | ingestion worker | processing worker | validated artifact envelope |
| `omnigraph.graph.extracted` | processing worker | graph projector | entities, relationships, provenance |
| `omnigraph.graph.projected` | graph projector | query cache/read model | graph write result |
| `omnigraph.dlq` | any stage | operations | poison messages and failure context |

### Message envelope

```json
{
  "jobId": "job-abc",
  "artifactId": "artifact-policy-x-pdf",
  "artifactKind": "Pdf",
  "uri": "s3://omnigraph/raw/policies/policy-x-v3.pdf",
  "checksum": "sha256-policy-x",
  "capturedAtUtc": "2026-04-01T08:30:00Z",
  "requestedBy": "analyst@omnigraph.ai",
  "correlationId": "corr-123",
  "metadata": {
    "owner": "compliance"
  }
}
```

### Production rules

- use idempotent producers
- use schema-registry-backed contracts in production
- keep artifact blobs outside Kafka; Kafka carries references and metadata
- emit retryable failures to stage-specific retry topics
- persist write status to PostgreSQL for operational visibility

## 7. Storage strategy

### Neo4j

- primary store for knowledge graph traversal
- temporal edges and versioned nodes
- impact analysis and path explanation

### PostgreSQL

- ingestion jobs
- parser run history
- user sessions and saved queries
- denormalized reporting views
- audit log for change application

### Optional vector store

Use only for:

- semantic fallback when deterministic linking fails
- image caption recall
- long-form document chunk assist

It should never replace the graph as the system of record.

## 8. Query engine design

### Query flow

1. User sends NL query to API.
2. Deterministic translator maps intent to structured filters.
3. API composes graph traversal query.
4. Neo4j returns nodes, edges, provenance, and temporal metadata.
5. API can optionally join PostgreSQL read models for tables and charts.

### Example

Input:

```text
Show risky transactions related to policy X
```

Planned graph query:

```cypher
MATCH (tx:Transaction)-[r:VIOLATES]->(policy:Policy)
WHERE tx.riskLevel IN ['High', 'Critical']
  AND policy.displayName CONTAINS 'Policy X'
RETURN tx, policy, r
LIMIT 25
```

## 9. Sample API endpoints

### REST

- `POST /api/uploads`
- `POST /api/ingestion/jobs`
- `POST /api/query/natural-language`
- `GET /api/graph/entities/{entityId}`
- `GET /api/graph/entities/{entityId}/neighbors?depth=2`
- `POST /api/graph/impact`
- `GET /api/graph/relationships/{relationshipId}/explain`

### GraphQL

See `docs/graphql-schema.graphql`.

Key operations:

- `entity(entityId: String!)`
- `search(entityType, minimumRisk, text, relatedToPolicy, depth, limit)`
- `naturalLanguage(input: String!)`
- `impact(entityId: String!, depth: Int)`
- `explainRelationship(relationshipId: String!)`
- `submitIngestion(input: SubmitIngestionInput!)`

## 10. Frontend architecture

### Major surfaces

- graph canvas for nodes and edges
- query workbench with NL and structured query modes
- node inspector with provenance and relationship reasoning
- structured data workbench for Excel-style tables and metrics
- timeline mode for revision drift and before/after analysis

### Recommended frontend stack

- React + TypeScript
- Apollo Client for GraphQL
- React Flow or Cytoscape for advanced graph rendering
- ECharts or Recharts for metric panels
- Zustand or Redux Toolkit for graph/query workspace state

## 11. Step-by-step implementation plan

### Phase 1: foundation

1. Finalize graph ontology and event contracts.
2. Stand up Kafka, PostgreSQL, and Neo4j in dev and staging.
3. Implement artifact upload and ingestion job lifecycle.

### Phase 2: deterministic extraction

1. Build Excel, PDF, log, code, and image parser adapters.
2. Emit normalized extraction envelopes with provenance.
3. Project entities and relationships into Neo4j and PostgreSQL.

### Phase 3: query plane

1. Build GraphQL schema and query resolvers.
2. Implement NL translator with deterministic templates first.
3. Add explainability and impact analysis endpoints.

### Phase 4: product UX

1. Build graph canvas and node inspector.
2. Add saved views, filters, and temporal timeline controls.
3. Add streaming updates for live graph mutations.

### Phase 5: production hardening

1. Add authn/authz and tenant isolation.
2. Add observability: OpenTelemetry, Prometheus, structured logs.
3. Add retry policies, DLQ dashboards, schema evolution tests, and SLOs.

## 12. Non-functional requirements

- deterministic linking before probabilistic linking
- idempotent ingestion and graph projection
- provenance on every edge used for decisioning
- clear separation between raw artifact storage and graph storage
- horizontal scaling at the worker tier
- zero-trust service-to-service auth in production
