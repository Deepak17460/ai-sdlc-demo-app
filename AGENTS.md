# Project conventions for AI agents

Invoice approval demo app: ASP.NET Core Web API with a small static web UI.

## Layout

- `backend/src/InvoiceApp.Domain`: business rules, no web or storage code. `ApprovalService` owns the invoice lifecycle (Draft, PendingApproval, Approved, Rejected) and approval limits by role.
- `backend/src/InvoiceApp.Api`: controllers, sign-in (`Auth/`), in-memory repository, and the web UI under `wwwroot/`.
- `backend/tests/InvoiceApp.Tests`: xUnit tests, one test class per class under test, named `<ClassUnderTest>Tests`.
- `e2e`: Playwright end-to-end tests (`tests/`) and page objects (`pages/`).
- `docs/use-cases`: short briefs for features we may build.
- `openspec/`: OpenSpec change proposals, design, tasks, and delta specs.
- `.cursor/skills/`: project skills agents must load for approval work and PR review.
- `.cursor/hooks/`: hooks that keep AI interaction logs and structured commit/review output consistent.
- `.cursor/agents/`: named agent and sub-agent briefs for this assignment.

## Approval limits (source of truth)

These rules live in `ApprovalService`. Controllers must not re-implement them.

| Role | Solo approval limit |
| --- | --- |
| Manager (also legacy `Approver`) | up to 50,000 |
| Director (also `Admin`) | up to 500,000 |

Additional rules:

1. Amounts **above 500,000** need **two distinct approvers**. The second approver **must be a Director** (or Admin).
2. The **same user may never count as both approvers** on one invoice.
3. A rejected approval attempt returns a **clear reason** (`ArgumentException` → HTTP 400) and **must not change** the invoice (status, approvers, timestamps, notes).
4. On a high-value invoice, the first successful approval records `FirstApprovedBy` / `FirstApprovedAt` and leaves status `PendingApproval`. Only the valid second approval moves it to `Approved`.

## Conventions

- Controllers live in `Controllers/`, one per resource, route `api/[controller]`. Request bodies are `record` types declared inside the controller.
- Controllers inherit `ApiControllerBase` and use `Handle(...)` so service errors map to 404, 409 and 400.
- Every endpoint is behind sign-in. Restrict actions with `[RequireRole("...")]`: destructive actions are Admin only; approvals are Approver, Manager, Director, or Admin.
- Pass both username and role from the session into `ApprovalService.Approve`.
- Business rules go in the Domain project, never in controllers.
- Register services in `Program.cs`. Repositories are interfaces in Domain with an in-memory implementation in Api.
- Every new rule or endpoint needs tests. Use `FakeInvoiceRepository` in tests.
- Use clear names; no one-letter variables or `flag` parameters.
- End-to-end tests use page objects and role-based locators (`getByRole`, `getByLabel`), never CSS or XPath selectors.

## Commands

Run these from the repo root. Prefer WSL when working on Windows without a local .NET SDK:

- Run the app: `dotnet run --project backend/src/InvoiceApp.Api` then open http://localhost:5000
- Run the tests: `dotnet test backend/InvoiceApp.sln`
- Run end-to-end tests: `cd e2e && npm install && npx playwright install chromium && npm test`
- Mutation testing (Stryker): `dotnet tool install -g dotnet-stryker --version 4.13.0` then from `backend/`: `dotnet stryker`

## Demo accounts (not real credentials)

| User | Password | Role |
| --- | --- | --- |
| `demo.manager` | `ChangeMe123!` | Manager |
| `demo.director` | `ChangeMe123!` | Director |
| `demo.director2` | `ChangeMe123!` | Director |
| `demo.approver` | `ChangeMe123!` | Approver (Manager limits) |
| `demo.admin` | `ChangeMe123!` | Admin (Director limits) |

## Agent skills (load these)

- `.cursor/skills/approval-limits/SKILL.md` — implement or change approval rules and tests.
- `.cursor/skills/pr-review/SKILL.md` — review a PR diff and produce findings in the required verdict format.
- `.cursor/skills/ai-interaction-log/SKILL.md` — append every AI interaction to `AI_INTERACTION_LOG.md`.

## Hooks

Project hooks in `.cursor/hooks.json` append structured entries to `AI_INTERACTION_LOG.md` and remind agents to use the OpenSpec task checklist. Do not bypass them for this assignment.

## Review expectations

When reviewing a pull request (or asking an agent to), every finding must be listed with exactly one verdict:

- **fixed** — change made in this PR
- **dismissed** — not a real issue, with a short reason
- **deferred** — real but out of scope, with a short reason

Prefer findings that would break an approval rule, leak state on a rejected attempt, or skip a negative test. Style-only nits are optional.

## Multi-agent workflow

Use the briefs under `.cursor/agents/`:

1. **spec-agent** — own OpenSpec proposal/design/tasks.
2. **implementer-agent** — land single-approver limits first, then dual-approver, each as its own commit.
3. **test-agent** (sub-agent) — unit tests for every rule, including the two negative rules.
4. **e2e-agent** (sub-agent) — Playwright coverage for a rejected over-limit approval.
5. **review-agent** — run the PR review skill against the diff and write verdicts into the PR description.

## Never

- Commit real credentials, tokens or customer data.
- Add business logic to `Program.cs` or controllers.
- Implement the whole approval-limits feature in a single prompt or a single commit.
- Soften a failed over-limit approval into a silent no-op without a clear error message.
