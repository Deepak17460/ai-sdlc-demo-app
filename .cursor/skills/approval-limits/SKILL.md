---
name: approval-limits
description: Implement or change invoice approval limits by role in ApprovalService. Use when editing approval rules, dual-approver logic, or related unit tests.
---

# Approval limits skill

## Rules (do not invent alternatives)

- Manager / Approver solo limit: **50,000**
- Director / Admin solo limit: **500,000**
- Above **500,000**: two distinct approvers; second must be Director/Admin
- Same user cannot be both approvers
- Failed attempts throw `ArgumentException` with a clear message and mutate nothing

## Where to edit

- Domain: `backend/src/InvoiceApp.Domain/ApprovalService.cs`, `Invoice.cs`
- API: `ApprovalsController` (pass role), `appsettings.json` demo users
- Tests: `backend/tests/InvoiceApp.Tests/ApprovalServiceTests.cs`

## Implementation order

1. Single-approver limits + tests (own commit)
2. Dual-approver path + tests (own commit)
3. Playwright over-limit rejection

## Test checklist

- Manager at 50,000 succeeds; 50,000.01 fails unchanged
- Director at 500,000 succeeds; above 500,000 starts/continues dual path
- Same user as second approver fails unchanged
- Non-Director as second approver fails unchanged
- Happy dual path: Manager then Director (different users)