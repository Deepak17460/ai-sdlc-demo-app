# AI in SDLC — Assignment write-up

Assignment brief (kept in this repo): [`docs/assignment/AI-in-SDLC-Home-Assignment_1.pdf`](docs/assignment/AI-in-SDLC-Home-Assignment_1.pdf)

## Feature

Role-based approval limits on the invoice approval service:

- Manager may approve up to **50,000**
- Director may approve up to **500,000**
- Above **500,000** needs two distinct approvers; the second must be a Director
- Same user may never count as both approvers
- Rejected attempts return a clear reason and leave the invoice unchanged

## How the work was done

1. **Context first** — Extended `AGENTS.md`, added OpenSpec change files, Cursor skills, hooks, and named agents/sub-agents under `.cursor/`.
2. **Spec-driven** — OpenSpec change at `openspec/changes/add-approval-limits-by-role/` (proposal, design, tasks, delta spec).
3. **Two implementation steps** (separate commits)
   - Single-approver limits + unit tests
   - Dual-approver rule + negative-rule unit tests + UI first-approval hint + Playwright
4. **E2E** — Playwright test for a Manager over-limit rejection through the UI.
5. **Review** — Findings judged below (fixed / dismissed / deferred); same text in `PR_DESCRIPTION.md` for the PR body.
6. **AI log** — `AI_INTERACTION_LOG.md` plus hook-driven appends from `.cursor/hooks/`.

## Commands used (WSL for .NET)

Copy-paste script for the demo: [`demo.txt`](demo.txt)

```bash
dotnet test backend/InvoiceApp.sln
cd e2e && npm install && npx playwright install chromium && npm test
cd backend && ./run-stryker.sh   # needs: dotnet tool install -g dotnet-stryker --version 4.13.0
```

## Test evidence

- Unit tests: **15 passed** (`ApprovalServiceTests`, including both negative dual-approver rules).
- Playwright: **1 passed** — Manager over-limit approval rejected in UI; invoice stays Pending Approval.

## Mutation scores (Stryker.NET 4.13.0 on `ApprovalService.cs`)

| Run | Mutants tested | Killed | Survived | Score |
| --- | --- | --- | --- | --- |
| Before (3 lifecycle tests only) | 24 | 12 | 12 | **24.49%** |
| After (full approval-limit suite, 15 tests) | 46 | 35 | 11 | **71.43%** |

**Note:** Target framework is **net10.0** so the app runs on the WSL SDK (net8 runtime was not available). Stryker 4.13.0 was used as required by the assignment install note.

## Demo video

_Link to 8–10 minute MP4 (Teams recording or other):_ **TODO — upload and paste URL**

Demo should show live: UI over-limit rejection, unit tests, mutation run, and the Playwright e2e test. Use `demo.txt` while recording.

## Agent review findings and verdicts

(Also copied to `PR_DESCRIPTION.md` for the pull request description.)

### Finding 1: Approve signature dropped role → limits unenforceable at API
- Severity: high
- Verdict: **fixed**
- Reason: `ApprovalsController` now passes `session.Username` and `session.Role` into `ApprovalService.Approve`.

### Finding 2: Over-limit path could leave partial mutations
- Severity: high
- Verdict: **fixed**
- Reason: Limit and dual-approver guards throw `ArgumentException` before setting `Status` / `DecidedBy`; unit tests assert unchanged state.

### Finding 3: Same user as both high-value approvers
- Severity: high
- Verdict: **fixed**
- Reason: Explicit check against `FirstApprovedBy` with a clear error; covered by unit test.

### Finding 4: Second approver not required to be Director
- Severity: high
- Verdict: **fixed**
- Reason: Second approval requires Director/Admin authority; covered by unit test.

### Finding 5: Legacy Approver/Admin accounts left without a mapping
- Severity: medium
- Verdict: **fixed**
- Reason: Approver maps to Manager limits; Admin maps to Director limits so existing demo accounts keep working.

### Finding 6: No UI coverage for over-limit rejection
- Severity: medium
- Verdict: **fixed**
- Reason: Playwright test `e2e/tests/approval-limits.spec.ts` covers the path.

### Finding 7: Persist approvals in a real database
- Severity: low
- Verdict: **deferred**
- Reason: Demo app is intentionally in-memory; out of assignment scope.

### Finding 8: Exact culture formatting of `N0` / `N2` in error messages across locales
- Severity: low
- Verdict: **dismissed**
- Reason: Demo runs with invariant-friendly numeric formatting in tests via substring checks on `50,000` / `500,000`; not a product requirement for this assignment.

## Deliverable map (matches “What to hand in”)

| # | Requirement | Status | Where |
| --- | --- | --- | --- |
| 1 | PR on fork, ≥ 3 standalone commits | Ready locally on `feature/approval-limits-by-role` — push/open PR on your fork | git + `PR_DESCRIPTION.md` |
| 2 | Updated AGENTS.md | Done | `AGENTS.md` |
| 3 | Spec files (OpenSpec) | Done | `openspec/changes/add-approval-limits-by-role/` |
| 4 | Unit tests for every rule (+ negatives) | Done | `backend/tests/InvoiceApp.Tests/ApprovalServiceTests.cs` |
| 5 | Playwright over-limit rejection | Done | `e2e/tests/approval-limits.spec.ts` |
| 6 | Mutation scores before/after | Done | table above |
| 7 | PR description with finding verdicts | Done (paste) | `PR_DESCRIPTION.md` |
| 8 | AI interaction log | Done | `AI_INTERACTION_LOG.md`, `.cursor/hooks/` |
| 9 | ASSIGNMENT.md + video link | Write-up done; **video link TODO** | this file |
| 10 | 8–10 min demo video | **TODO — record & link** | use `demo.txt` |

## Related

- Skills / hooks / multi-agents: `.cursor/skills`, `.cursor/hooks`, `.cursor/agents`
- Demo command sheet: `demo.txt`
