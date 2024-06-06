import { test, expect } from '@playwright/test';

test('should logout', async ({ page }) => {
    await page.goto('/');
    await page.click('text=Logout');
    await expect(page.locator('[data-testid="login-button"]')).toBeVisible();
});