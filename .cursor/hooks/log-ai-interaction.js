#!/usr/bin/env node
/**
 * Appends a structured AI interaction line to AI_INTERACTION_LOG.md.
 * Reads hook JSON from stdin; always allows the event to continue.
 */
const fs = require('fs');
const path = require('path');

const eventName = process.argv[2] || 'unknown';
const logPath = path.join(process.cwd(), 'AI_INTERACTION_LOG.md');

function readStdin() {
  return new Promise((resolve) => {
    let data = '';
    process.stdin.setEncoding('utf8');
    process.stdin.on('data', (chunk) => { data += chunk; });
    process.stdin.on('end', () => resolve(data));
    process.stdin.on('error', () => resolve(''));
  });
}

(async () => {
  const raw = await readStdin();
  let payload = {};
  try {
    payload = raw ? JSON.parse(raw) : {};
  } catch {
    payload = { raw };
  }

  const stamp = new Date().toISOString();
  const summary = JSON.stringify({
    event: eventName,
    tool: payload.tool_name || payload.toolName || null,
    subagent: payload.subagent_type || payload.subagentType || null,
    keys: Object.keys(payload),
  });

  const entry = `\n### Hook ${stamp} — \`${eventName}\`\n\n\`\`\`json\n${summary}\n\`\`\`\n`;

  try {
    if (!fs.existsSync(logPath)) {
      fs.writeFileSync(logPath, '# AI interaction log\n\nMachine and agent entries for the AI-in-SDLC assignment.\n', 'utf8');
    }
    fs.appendFileSync(logPath, entry, 'utf8');
  } catch (error) {
    // Fail open: never block the agent for logging problems.
    process.stderr.write(`log-ai-interaction: ${error.message}\n`);
  }

  if (eventName === 'sessionStart') {
    process.stdout.write(JSON.stringify({
      additional_context: 'Project hooks active. Log AI steps to AI_INTERACTION_LOG.md. Follow AGENTS.md approval limits and two-step commits.',
    }));
  } else {
    process.stdout.write('{}');
  }
})();
