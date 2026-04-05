import { useState } from "react";
import { DataWorkbench } from "./components/DataWorkbench";
import { GraphCanvas } from "./components/GraphCanvas";
import { NodeInspector } from "./components/NodeInspector";
import { QueryWorkbench } from "./components/QueryWorkbench";
import { graphEdges, graphNodes, metrics, sampleQueries, transactionRows } from "./lib/mockData";

export default function App() {
  const [selectedNodeId, setSelectedNodeId] = useState("transaction-88421");
  const [query, setQuery] = useState(sampleQueries[0]);

  const selectedNode = graphNodes.find((node) => node.id === selectedNodeId) ?? graphNodes[0];
  const selectedEdges = graphEdges.filter(
    (edge) => edge.from === selectedNode.id || edge.to === selectedNode.id
  );

  return (
    <div className="app-shell">
      <header className="hero">
        <div>
          <p className="eyebrow">OmniGraph AI</p>
          <h1>Multi-modal knowledge engine for operational reasoning</h1>
          <p className="hero-copy">
            Replace vector-only RAG with deterministic graph intelligence across
            Excel, PDFs, logs, code, and images.
          </p>
        </div>
        <div className="hero-stats">
          <div>
            <span>Streaming</span>
            <strong>Kafka-native</strong>
          </div>
          <div>
            <span>Storage</span>
            <strong>Graph-first</strong>
          </div>
          <div>
            <span>Explainability</span>
            <strong>Source traced</strong>
          </div>
        </div>
      </header>

      <main className="dashboard-grid">
        <QueryWorkbench query={query} onQueryChange={setQuery} examples={sampleQueries} />
        <GraphCanvas
          nodes={graphNodes}
          edges={graphEdges}
          selectedNodeId={selectedNode.id}
          onSelectNode={setSelectedNodeId}
        />
        <NodeInspector node={selectedNode} edges={selectedEdges} />
        <DataWorkbench metrics={metrics} rows={transactionRows} />
      </main>
    </div>
  );
}
