type QueryWorkbenchProps = {
  query: string;
  onQueryChange: (value: string) => void;
  examples: string[];
};

export function QueryWorkbench({ query, onQueryChange, examples }: QueryWorkbenchProps) {
  return (
    <section className="panel query-panel">
      <div className="panel-header">
        <div>
          <p className="eyebrow">Query Engine</p>
          <h2>Ask in natural language</h2>
        </div>
      </div>
      <textarea
        value={query}
        onChange={(event) => onQueryChange(event.target.value)}
        className="query-input"
      />
      <div className="chip-row">
        {examples.map((example) => (
          <button key={example} className="chip" onClick={() => onQueryChange(example)}>
            {example}
          </button>
        ))}
      </div>
      <div className="planner-card">
        <p className="eyebrow">Structured plan</p>
        <code>{`MATCH (tx:Transaction)-[:VIOLATES]->(policy:Policy {displayName: "Policy X"}) RETURN tx, policy`}</code>
      </div>
    </section>
  );
}
