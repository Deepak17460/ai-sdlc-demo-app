import { test, expect } from '@playwright/test';
import { LoginPage } from '../pages/login.page';
import { InvoicesPage } from '../pages/invoices.page';
import { ApprovalsPage } from '../pages/approvals.page';

test('manager over-limit approval is rejected in the UI and invoice stays pending', async ({ page }) => {
  const login = new LoginPage(page);
  const invoices = new InvoicesPage(page);
  const approvals = new ApprovalsPage(page);
  const customer = `Over Limit School ${Date.now()}`;

  await login.goto();
  await login.signIn('demo.manager', 'ChangeMe123!');
  await invoices.createInvoice(customer, 50000.01);
  await invoices.submitLatestDraftForApproval(customer);

  await approvals.approveInvoice(customer);
  await approvals.expectDecisionErrorContaining('50,000');
  await approvals.expectStillPending(customer);

  await page.getByRole('tab', { name: 'Invoices' }).click();
  const row = page.getByRole('row').filter({ hasText: customer });
  await expect(row.getByRole('cell', { name: 'Pending Approval' })).toBeVisible();
});
