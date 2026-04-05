import { UiGraphEdge, UiGraphNode } from "../lib/mockData";

type GraphCanvasProps = {
  nodes: UiGraphNode[];
  edges: UiGraphEdge[];
  selectedNodeId: string;
  onSelectNode: (nodeId: string) => void;
};

export function GraphCanvas({
  nodes,
  edges,
  selectedNodeId,
  onSelectNode
}: GraphCanvasProps) {
  return (
    <section className="panel graph-panel">
      <div className="panel-header">
        <div>
          <p className="eyebrow">Knowledge Graph</p>
          <h2>Live relationship canvas</h2>
        </div>
        <p className="muted">Temporal, explainable, and multi-modal.</p>
      </div>
      <svg viewBox="0 0 900 460" className="graph-svg" role="img" aria-label="Knowledge graph">
        {edges.map((edge) => {
          const from = nodes.find((node) => node.id === edge.from)!;
          const to = nodes.find((node) => node.id === edge.to)!;

          return (
            <g key={edge.id}>
              <line x1={from.x} y1={from.y} x2={to.x} y2={to.y} className="graph-edge" />
              <text x={(from.x + to.x) / 2} y={(from.y + to.y) / 2 - 8} className="graph-edge-label">
                {edge.type}
              </text>
            </g>
          );
        })}
        {nodes.map((node) => (
          <g key={node.id} transform={`translate(${node.x}, ${node.y})`} onClick={() => onSelectNode(node.id)}>
            <circle
              r={selectedNodeId === node.id ? 34 : 28}
              className={`graph-node graph-node-${node.type.toLowerCase()} ${selectedNodeId === node.id ? "selected" : ""}`}
            />
            <text textAnchor="middle" className="graph-node-label" y={4}>
              {node.label}
            </text>
          </g>
        ))}
      </svg>
    </section>
  );
}
