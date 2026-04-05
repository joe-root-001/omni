export type UiGraphNode = {
  id: string;
  label: string;
  type: string;
  risk: string;
  x: number;
  y: number;
  properties: Record<string, string>;
};

export type UiGraphEdge = {
  id: string;
  from: string;
  to: string;
  type: string;
  explanation: string;
};

export const graphNodes: UiGraphNode[] = [
  {
    id: "policy-x",
    label: "Policy X",
    type: "Policy",
    risk: "High",
    x: 90,
    y: 70,
    properties: { owner: "Compliance", version: "v3", changeWindow: "2026-Q2" }
  },
  {
    id: "transaction-88421",
    label: "TX-88421",
    type: "Transaction",
    risk: "Critical",
    x: 310,
    y: 160,
    properties: { amount: "175000 USD", region: "APAC", flag: "manual review" }
  },
  {
    id: "event-velocity-breach",
    label: "Velocity Breach",
    type: "Event",
    risk: "High",
    x: 280,
    y: 320,
    properties: { eventCode: "PAY-429", service: "payments-service" }
  },
  {
    id: "service-payments",
    label: "Payments Service",
    type: "Service",
    risk: "High",
    x: 550,
    y: 220,
    properties: { environment: "prod", tier: "critical" }
  },
  {
    id: "function-risk-evaluator",
    label: "EvaluateTransaction",
    type: "Function",
    risk: "Medium",
    x: 760,
    y: 130,
    properties: { module: "RiskGuard", language: "C#" }
  },
  {
    id: "user-1007",
    label: "Ava Patel",
    type: "User",
    risk: "Medium",
    x: 560,
    y: 390,
    properties: { department: "Treasury", country: "IN" }
  }
];

export const graphEdges: UiGraphEdge[] = [
  {
    id: "rel-transaction-violates-policy",
    from: "transaction-88421",
    to: "policy-x",
    type: "violates",
    explanation: "Transaction 88421 breaches Policy X daily volume limits."
  },
  {
    id: "rel-event-triggers-transaction",
    from: "event-velocity-breach",
    to: "transaction-88421",
    type: "triggers",
    explanation: "A velocity breach event preceded the transaction."
  },
  {
    id: "rel-service-produces-event",
    from: "service-payments",
    to: "event-velocity-breach",
    type: "produces",
    explanation: "Payments Service emitted the breach event."
  },
  {
    id: "rel-service-depends-function",
    from: "service-payments",
    to: "function-risk-evaluator",
    type: "depends_on",
    explanation: "Payments Service depends on the RiskGuard evaluator."
  },
  {
    id: "rel-policy-affects-service",
    from: "policy-x",
    to: "service-payments",
    type: "affects",
    explanation: "Policy X is enforced by Payments Service."
  },
  {
    id: "rel-transaction-belongs-user",
    from: "transaction-88421",
    to: "user-1007",
    type: "belongs_to",
    explanation: "The finance workbook links the transaction to Ava Patel."
  }
];

export const sampleQueries = [
  "Show risky transactions related to policy X",
  "If policy X changes, what is affected?",
  "Explain why TX-88421 is linked to Payments Service"
];

export const transactionRows = [
  { id: "TX-88421", owner: "Ava Patel", amount: "175000", risk: "Critical", region: "APAC" },
  { id: "TX-88418", owner: "Ken Wu", amount: "92000", risk: "High", region: "NA" },
  { id: "TX-88409", owner: "Lea Costa", amount: "81000", risk: "Medium", region: "EU" }
];

export const metrics = [
  { label: "Artifacts Ingested", value: "12.4k" },
  { label: "Active Policies", value: "381" },
  { label: "High Risk Transactions", value: "219" },
  { label: "Parser Accuracy", value: "96.2%" }
];
