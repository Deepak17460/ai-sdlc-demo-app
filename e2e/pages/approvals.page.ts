import { Page, expect } from '@playwright/test';

export class ApprovalsPage {
  constructor(private readonly page: Page) {}

  async openApprovalsTab() {
    await this.page.getByRole('tab', { name: 'Approvals' }).click();
  }

  async approveInvoice(invoiceLabelPart: string) {
    await this.openApprovalsTab();
    const item = this.page.getByRole('listitem').filter({ hasText: invoiceLabelPart });
    await item.getByRole('button', { name: /Approve invoice/ }).click();
  }

  async expectDecisionErrorContaining(text: string) {
    const alert = this.page.getByRole('alert').filter({ hasText: text });
    await expect(alert).toBeVisible();
  }

  async expectStillPending(invoiceLabelPart: string) {
    await this.openApprovalsTab();
    await expect(this.page.getByRole('listitem').filter({ hasText: invoiceLabelPart })).toBeVisible();
  }
}
