---
name: pr-review
description: Review a pull request diff for the invoice approval demo and produce findings with fixed, dismissed, or deferred verdicts.
---

# PR review skill

## Focus areas

1. Approval limit correctness (amounts, roles, dual approval)
2. Rejected attempts must not mutate invoice state
3. Business logic leaking into controllers
4. Missing negative tests
5. Playwright covering UI rejection path

## Output format (required)

List every finding as:

```
### Finding N: <short title>
- Severity: high | medium | low
- Verdict: fixed | dismissed | deferred
- Reason: <one or two sentences>
```

Do not leave a finding without a verdict.