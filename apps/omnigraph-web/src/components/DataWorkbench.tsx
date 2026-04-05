type Metric = {
  label: string;
  value: string;
};

type TransactionRow = {
  id: string;
  owner: string;
  amount: string;
  risk: string;
  region: string;
};

type DataWorkbenchProps = {
  metrics: Metric[];
  rows: TransactionRow[];
};

export function DataWorkbench({ metrics, rows }: DataWorkbenchProps) {
  return (
    <section className="panel data-panel">
      <div className="panel-header">
        <div>
          <p className="eyebrow">Structured Views</p>
          <h2>Excel and operational evidence</h2>
        </div>
      </div>
      <div className="metric-grid">
        {metrics.map((metric) => (
          <div key={metric.label} className="metric-card">
            <span>{metric.label}</span>
            <strong>{metric.value}</strong>
          </div>
        ))}
      </div>
      <table className="data-table">
        <thead>
          <tr>
            <th>Transaction</th>
            <th>Owner</th>
            <th>Amount</th>
            <th>Risk</th>
            <th>Region</th>
          </tr>
        </thead>
        <tbody>
          {rows.map((row) => (
            <tr key={row.id}>
              <td>{row.id}</td>
              <td>{row.owner}</td>
              <td>{row.amount}</td>
              <td>{row.risk}</td>
              <td>{row.region}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </section>
  );
}
