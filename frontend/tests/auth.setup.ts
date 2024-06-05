import { test as setup } from '@playwright/test';
import { AUTH_EMAIL, AUTH_PASSWORD } from '../config';

const authFile = 'playwright/.auth/user.json';

setup('authenticate', async ({ page }) => {
    await page.goto('http://localhost:3000/');
    await page.getByTestId('login-button').click();
    await page.waitForLoadState('domcontentloaded');
    await page.getByLabel('Email address*').fill(`${AUTH_EMAIL}`);
    await page.getByLabel('Password*').fill(`${AUTH_PASSWORD}`);
    await page.click('button[type="submit"]');

    await page.context().storageState({ path: authFile });
});
