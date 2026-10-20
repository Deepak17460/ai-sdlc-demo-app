---
name: ai-interaction-log
description: Append structured AI interaction entries to AI_INTERACTION_LOG.md for the AI-in-SDLC assignment.
---

# AI interaction log skill

After each meaningful AI step (spec, implementation prompt, test generation, review), append an entry to `AI_INTERACTION_LOG.md` using this template:

```markdown
## YYYY-MM-DD HH:MM — <step name>

- Agent: <agent or sub-agent name>
- Prompt summary: <one sentence>
- Tools/skills used: <list>
- Outcome: <what changed or was decided>
- Commit: <sha or n/a>
```

Hooks under `.cursor/hooks/` also append machine-readable lines; keep both in sync when possible.