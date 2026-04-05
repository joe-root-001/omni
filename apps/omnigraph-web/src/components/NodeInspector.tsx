import { UiGraphEdge, UiGraphNode } from "../lib/mockData";

type NodeInspectorProps = {
  node: UiGraphNode;
  edges: UiGraphEdge[];
};

export function NodeInspector({ node, edges }: NodeInspectorProps) {
  return (
    <section className="panel inspector-panel">
      <div className="panel-header">
        <div>
          <p className="eyebrow">Node Inspector</p>
          <h2>{node.label}</h2>
        </div>
        <span className={`risk-pill risk-${node.risk.toLowerCase()}`}>{node.risk}</span>
      </div>
      <div className="inspector-block">
        <span className="muted">Type</span>
        <strong>{node.type}</strong>
      </div>
      <div className="inspector-block">
        <span className="muted">Properties</span>
        <ul className="property-list">
          {Object.entries(node.properties).map(([key, value]) => (
            <li key={key}>
              <span>{key}</span>
              <strong>{value}</strong>
            </li>
          ))}
        </ul>
      </div>
      <div className="inspector-block">
        <span className="muted">Connected edges</span>
        <ul className="connection-list">
          {edges.map((edge) => (
            <li key={edge.id}>
              <strong>{edge.type}</strong>
              <span>{edge.explanation}</span>
            </li>
          ))}
        </ul>
      </div>
    </section>
  );
}
