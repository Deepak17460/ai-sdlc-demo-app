# Invoice Approval Demo App

A small invoice approval app used for live AI-in-SDLC demos. It is deliberately simple: an ASP.NET Core API, a plain web UI, and an in-memory store, so it runs with one command and no database.

Target framework: **net10.0** (runs on the .NET SDK available in WSL).

## Run it

```
dotnet run --project backend/src/InvoiceApp.Api
```

Open http://localhost:5000 and sign in:

| User | Password | Role |
| --- | --- | --- |
| `demo.manager` | `ChangeMe123!` | Manager (solo approve ≤ 50,000) |
| `demo.director` | `ChangeMe123!` | Director (solo approve ≤ 500,000) |
| `demo.director2` | `ChangeMe123!` | Director (second approver demos) |
| `demo.approver` | `ChangeMe123!` | Approver (same limits as Manager) |
| `demo.admin` | `ChangeMe123!` | Admin (same limits as Director) |

These are demo accounts for a local, in-memory app. They are not real credentials.

## What it does

1. Create an invoice (it starts as a draft).
2. Submit it for approval.
3. An approver approves it within their role limit, or rejects it with a reason.
4. Invoices above 500,000 need two distinct approvers; the second must be a Director.

Invoices move through Draft, Pending Approval, then Approved or Rejected.

## Tests

```
dotnet test backend/InvoiceApp.sln
```

End-to-end:

```
cd e2e && npm install && npx playwright install chromium && npm test
```

## Project layout

```
backend/
  src/InvoiceApp.Domain   business rules
  src/InvoiceApp.Api      controllers, sign-in, web UI (wwwroot)
  tests/InvoiceApp.Tests  xUnit tests
e2e/                      Playwright tests
openspec/                 OpenSpec change proposal and tasks
.cursor/                  skills, hooks, agent briefs
docs/use-cases            short feature briefs
AGENTS.md                 conventions for AI coding agents
ASSIGNMENT.md             assignment write-up
AI_INTERACTION_LOG.md     AI interaction log
```

## Use cases

- [UC-1 Bulk invoice approval](docs/use-cases/UC-1-bulk-approval.md)
- [UC-2 Partial approval](docs/use-cases/UC-2-partial-approval.md)
