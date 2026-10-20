# AI interaction log

Machine and agent entries for the AI-in-SDLC assignment.

## 2026-10-20 — Assignment kickoff (spec-agent)

- Agent: spec-agent
- Prompt summary: Read assignment PDF; set up OpenSpec change, AGENTS.md, skills, hooks, and agent briefs.
- Tools/skills used: pdf extraction, OpenSpec layout, approval-limits skill draft, ai-interaction-log skill
- Outcome: Context files created under `openspec/`, `.cursor/skills/`, `.cursor/hooks/`, `.cursor/agents/`.
- Commit: n/a (git deferred by request)

## 2026-10-20 — Single-approver limits (implementer-agent + test-agent)

- Agent: implementer-agent; sub-agent: test-agent
- Prompt summary: Land Manager 50,000 and Director 500,000 solo limits with unit tests; do not implement dual approval yet.
- Tools/skills used: approval-limits skill, xUnit tests
- Outcome: `ApprovalService.Approve` takes role; over-limit attempts throw without mutating state.
- Commit: n/a

## 2026-10-20 — Dual-approver rule (implementer-agent + test-agent)

- Agent: implementer-agent; sub-agent: test-agent
- Prompt summary: Add two-approver path above 500,000 with distinct-user and Director-as-second constraints.
- Tools/skills used: approval-limits skill
- Outcome: First/second approval fields and negative-rule tests added; 15 unit tests green.
- Commit: n/a

## 2026-10-20 — Playwright over-limit (e2e-agent)

- Agent: e2e-agent
- Prompt summary: Cover rejected Manager over-limit approval through the UI.
- Tools/skills used: Playwright page objects
- Outcome: `e2e/tests/approval-limits.spec.ts` passed (1/1).
- Commit: n/a

## 2026-10-20 — Mutation scores

- Agent: test-agent
- Prompt summary: Capture Stryker before/after scores on ApprovalService.
- Tools/skills used: dotnet-stryker 4.13.0
- Outcome: Before 24.49% (3 tests); After 71.43% (15 tests).
- Commit: n/a

## 2026-10-20 — Review findings (review-agent)

- Agent: review-agent
- Prompt summary: Review implementation using pr-review skill; judge each finding.
- Tools/skills used: pr-review skill
- Outcome: Findings with verdicts recorded in ASSIGNMENT.md.
- Commit: n/a
