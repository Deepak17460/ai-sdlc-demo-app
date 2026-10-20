# Pull request description (paste into GitHub)

## Summary

Adds role-based invoice approval limits to the demo app, implemented with OpenSpec, agent skills/hooks, staged commits, unit tests, Playwright e2e, and mutation testing evidence.

- Managers solo-approve up to 50,000; Directors up to 500,000
- Above 500,000 requires two distinct approvers; second must be a Director
- Rejected attempts return a clear reason and leave the invoice unchanged

## Spec / context

- OpenSpec: `openspec/changes/add-approval-limits-by-role/`
- Agent context: `AGENTS.md`, `.cursor/skills`, `.cursor/hooks`, `.cursor/agents`
- Assignment brief (reference): `docs/assignment/AI-in-SDLC-Home-Assignment_1.pdf`
- Write-up: `ASSIGNMENT.md`
- AI log: `AI_INTERACTION_LOG.md`

## Test plan

- [x] `dotnet test backend/InvoiceApp.sln` — 15 passed
- [x] `cd e2e && npm test` — over-limit UI rejection
- [x] Stryker on `ApprovalService` — before **24.49%**, after **71.43%**
- [ ] Manual UI smoke (Manager 50000.01 rejected)

## Demo video

https://nagarro-my.sharepoint.com/:v:/p/deepak_kumar25/IQBbHU6PEhEGTKgidjd9h9K3AbJYKu88HneJ6TPtRinOJlU?nav=eyJyZWZlcnJhbEluZm8iOnsicmVmZXJyYWxBcHAiOiJPbmVEcml2ZUZvckJ1c2luZXNzIiwicmVmZXJyYWxBcHBQbGF0Zm9ybSI6IldlYiIsInJlZmVycmFsTW9kZSI6InZpZXciLCJyZWZlcnJhbFZpZXciOiJNeUZpbGVzTGlua0NvcHkifX0&e=VMmFyO

## Agent review findings and verdicts

### Finding 1: Approve signature dropped role → limits unenforceable at API
- Severity: high
- Verdict: **fixed**
- Reason: `ApprovalsController` now passes `session.Username` and `session.Role` into `ApprovalService.Approve`.

### Finding 2: Over-limit path could leave partial mutations
- Severity: high
- Verdict: **fixed**
- Reason: Guards throw `ArgumentException` before mutating status/decision fields; unit tests assert unchanged state.

### Finding 3: Same user as both high-value approvers
- Severity: high
- Verdict: **fixed**
- Reason: Explicit check against `FirstApprovedBy`; covered by unit test.

### Finding 4: Second approver not required to be Director
- Severity: high
- Verdict: **fixed**
- Reason: Second approval requires Director/Admin authority; covered by unit test.

### Finding 5: Legacy Approver/Admin accounts left without a mapping
- Severity: medium
- Verdict: **fixed**
- Reason: Approver → Manager limits; Admin → Director limits.

### Finding 6: No UI coverage for over-limit rejection
- Severity: medium
- Verdict: **fixed**
- Reason: `e2e/tests/approval-limits.spec.ts`.

### Finding 7: Persist approvals in a real database
- Severity: low
- Verdict: **deferred**
- Reason: Demo app is intentionally in-memory; out of scope.

### Finding 8: Exact culture formatting of `N0` / `N2` in error messages
- Severity: low
- Verdict: **dismissed**
- Reason: Tests assert on `50,000` / `500,000` substrings; not a product requirement for this assignment.
