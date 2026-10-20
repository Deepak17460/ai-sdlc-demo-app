# Tasks: Approval limits by role

## 1. Context and specification

- [x] Extend `AGENTS.md` with approval rules, commands, skills, hooks, review expectations
- [x] Add OpenSpec change under `openspec/changes/add-approval-limits-by-role/`
- [x] Add project skills, hooks, agent briefs, and AI interaction log

## 2. Single-approver limits (commit 1)

- [x] Add Manager/Director demo users
- [x] Extend `Approve(invoiceId, approver, role)` with 50,000 / 500,000 limits
- [x] Wire role from `ApprovalsController`
- [x] Unit tests: Manager within/over limit; Director within/over limit; rejected attempt leaves invoice unchanged

## 3. Dual-approver rule (commit 2)

- [x] Add `FirstApprovedBy` / `FirstApprovedAt` on `Invoice`
- [x] Implement two-approver path for amounts above 500,000
- [x] Enforce distinct users and Director-as-second
- [x] Unit tests for happy path and both negative rules
- [x] UI hint when first approval is recorded

## 4. End-to-end and evidence

- [x] Playwright page objects + rejected over-limit test
- [ ] Mutation scores before/after (record in `ASSIGNMENT.md`)
- [x] AI interaction log entries for the work
- [x] `ASSIGNMENT.md` write-up template (video link filled by student)

## 5. Review

- [x] Agent review findings with fixed / dismissed / deferred verdicts in PR description draft