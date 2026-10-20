# Proposal: Approval limits by role

## Why

Finance needs enforceable approval authority. Today any Approver or Admin can approve any pending invoice regardless of amount. That invites mistakes on large invoices and makes dual control impossible to prove.

## What changes

- Managers may solo-approve invoices up to **50,000**.
- Directors may solo-approve invoices up to **500,000**.
- Invoices **above 500,000** require **two distinct approvers**; the **second must be a Director**.
- The same user cannot count as both approvers.
- Failed approval attempts return a clear reason and leave the invoice unchanged.

## Out of scope

- Partial amount approval (UC-2).
- Bulk approval (UC-1).
- Email notifications or external workflow engines.

## Success criteria

- Unit tests cover every limit rule and both negative rules (same-user dual approval; over-limit rejection with no state change).
- One Playwright test shows a rejected over-limit approval in the UI.
- Implementation lands in at least two commits (single-approver, then dual-approver).