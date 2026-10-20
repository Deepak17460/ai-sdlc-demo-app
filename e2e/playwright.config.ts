import { defineConfig, devices } from '@playwright/test';

export default defineConfig({
  testDir: './tests',
  workers: 1,
  retries: process.env.CI ? 1 : 0,
  reporter: [['list'], ['html', { open: 'never' }]],
  use: {
    baseURL: 'http://localhost:5000',
    trace: 'on-first-retry',
  },
  projects: [{ name: 'chromium', use: { ...devices['Desktop Chrome'] } }],
  webServer: {
    // From WSL: run API directly. From Windows: start API via WSL.
    command: process.platform === 'win32'
      ? 'wsl --cd "/mnt/c/Users/dekuma/OneDrive - ASSA ABLOY Group/Desktop/AlgoMaster/NAGPOps/Assignements/AI_SDLC/ai-sdlc-demo-app" -- dotnet run --project backend/src/InvoiceApp.Api --urls http://0.0.0.0:5000'
      : 'dotnet run --project ../backend/src/InvoiceApp.Api --urls http://0.0.0.0:5000',
    url: 'http://localhost:5000',
    reuseExistingServer: !process.env.CI,
    timeout: 180_000,
  },
});

