#!/usr/bin/env node
/**
 * On agent stop, remind about structured PR verdicts and the interaction log.
 */
const fs = require('fs');
const path = require('path');

const logPath = path.join(process.cwd(), 'AI_INTERACTION_LOG.md');
const stamp = new Date().toISOString();

try {
  if (!fs.existsSync(logPath)) {
    fs.writeFileSync(logPath, '# AI interaction log\n\nMachine and agent entries for the AI-in-SDLC assignment.\n', 'utf8');
  }
  fs.appendFileSync(
    logPath,
    `\n### Hook ${stamp} — \`stop\`\n\n- Reminder: every review finding needs verdict fixed | dismissed | deferred.\n- Reminder: single-approver and dual-approver work must be separate commits.\n`,
    'utf8'
  );
} catch {
  // fail open
}

process.stdout.write(JSON.stringify({
  followup_message: 'Before finishing: update AI_INTERACTION_LOG.md, ensure OpenSpec tasks reflect reality, and list PR review findings with verdicts.',
}));
