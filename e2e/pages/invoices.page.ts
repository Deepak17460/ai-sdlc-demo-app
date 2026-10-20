import { Page, expect } from '@playwright/test';

export class InvoicesPage {
  constructor(private readonly page: Page) {}

  async openInvoicesTab() {
    await this.page.getByRole('tab', { name: 'Invoices' }).click();
  }

  async createInvoice(customerName: string, amount: number) {
    await this.openInvoicesTab();
    await this.page.getByLabel('Customer name').fill(customerName);
    await this.page.getByLabel('Amount').fill(String(amount));
    await this.page.getByRole('button', { name: 'Create invoice' }).click();
    await expect(this.page.getByRole('cell', { name: customerName })).toBeVisible();
  }

  async submitLatestDraftForApproval(customerName: string) {
    const row = this.page.getByRole('row').filter({ hasText: customerName });
    await row.getByRole('button', { name: /Submit invoice .* for approval/ }).click();
    await expect(row.getByRole('cell', { name: 'Pending Approval' })).toBeVisible();
  }
}
