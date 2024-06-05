import { test as setup } from '@playwright/test';
import { config } from 'dotenv';

const authFile = 'playwright/.auth/user.json';

config({ path: '.env.local' });

setup('authenticate', async ({ page }) => {
    await page.goto('/');
    await page.getByTestId('login-button').click();
    await page.waitForLoadState('domcontentloaded');
    await page.getByLabel('Email address*').fill(process.env.AUTH_EMAIL ?? '');
    await page.getByLabel('Password*').fill(process.env.AUTH_PASSWORD ?? '');
    await page.click('button[type="submit"]');

    await page.context().storageState({ path: authFile });
});
