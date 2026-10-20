# Design: Approval limits by role

## Approach

Keep all rules in `ApprovalService`. Extend `Approve` to accept the approver's **role** from the session. Controllers stay thin: read session, call service, map exceptions.

## Domain model

Add optional first-approval fields on `Invoice`:

- `FirstApprovedBy`
- `FirstApprovedAt`

Status stays `PendingApproval` after the first high-value approval. Final approval sets `Approved`, `DecidedBy`, and `DecidedAt`.

## Role mapping

| Session role | Limit / authority |
| --- | --- |
| Manager | Solo ≤ 50,000; may be first approver above 500,000 |
| Approver | Same as Manager (legacy demo account) |
| Director | Solo ≤ 500,000; may be first or second approver above 500,000 |
| Admin | Same as Director |

## Decision flow

1. Invoice must be `PendingApproval` (existing guard).
2. If amount ≤ 500,000 → enforce solo limit for the role; on success, approve fully.
3. If amount > 500,000:
   - No `FirstApprovedBy` → record first approver (Manager/Director family), leave pending.
   - Has `FirstApprovedBy` → reject if same user; reject if second role is not Director/Admin; otherwise approve fully.
4. Any rule failure throws `ArgumentException` **before** mutating fields so a rejected attempt changes nothing.

## API / UI

- `ApprovalsController.Approve` passes `session.Username` and `session.Role`.
- Demo users: `demo.manager`, `demo.director`, `demo.director2` (second Director for dual-approval demos).
- UI already surfaces `decision-error`; pending list should show when a first approval is recorded.

## Testing

- xUnit against `ApprovalService` with `FakeInvoiceRepository`.
- Playwright: Manager signs in, creates/submits an over-limit invoice, Approve fails with a visible reason, status remains pending.