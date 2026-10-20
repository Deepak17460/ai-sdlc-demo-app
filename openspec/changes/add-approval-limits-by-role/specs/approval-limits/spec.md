# Delta spec: approval-limits

## ADDED Requirements

### Requirement: Manager solo limit

A Manager SHALL be allowed to fully approve a pending invoice when its amount is less than or equal to 50,000.

#### Scenario: Manager approves within limit

- GIVEN a pending invoice for 50,000
- WHEN a Manager approves it
- THEN the invoice status is Approved and DecidedBy is that Manager

#### Scenario: Manager rejected above limit

- GIVEN a pending invoice for 50,000.01
- WHEN a Manager approves it
- THEN the attempt fails with a clear reason
- AND the invoice status and decision fields are unchanged

### Requirement: Director solo limit

A Director SHALL be allowed to fully approve a pending invoice when its amount is less than or equal to 500,000.

#### Scenario: Director approves within limit

- GIVEN a pending invoice for 500,000
- WHEN a Director approves it
- THEN the invoice status is Approved

#### Scenario: Director cannot solo-approve above limit

- GIVEN a pending invoice for 500,000.01 with no prior approval
- WHEN a Director approves it
- THEN the invoice remains PendingApproval
- AND FirstApprovedBy is recorded as that Director

### Requirement: Dual approval above 500,000

Invoices above 500,000 SHALL require two distinct approvers, and the second approver MUST be a Director (or Admin).

#### Scenario: Second Director completes approval

- GIVEN a pending invoice for 600,000 first-approved by Manager A
- WHEN Director B approves it
- THEN the invoice status is Approved and DecidedBy is Director B

#### Scenario: Same user cannot be both approvers

- GIVEN a pending invoice for 600,000 first-approved by Director A
- WHEN Director A approves it again
- THEN the attempt fails with a clear reason
- AND the invoice remains PendingApproval with the original first approver

#### Scenario: Second approver must be Director

- GIVEN a pending invoice for 600,000 first-approved by Director A
- WHEN Manager B approves it
- THEN the attempt fails with a clear reason
- AND the invoice is unchanged aside from the existing first approval
