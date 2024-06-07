import { test, expect, Page } from '@playwright/test';

// Helpers
const scrollUntilVisible = async (page: Page, locator: string, timeout: number = 2000) => {
    const startTime = Date.now();

    while (Date.now() - startTime < timeout) {
        try {
            if (await page.locator(locator).isVisible()) {
                return;
            }
        } catch (error) {
            console.error(error);
        }
        await page.evaluate(() => window.scrollBy(0, window.innerHeight));
        await page.waitForTimeout(500);
    }
    throw new Error(`Element ${locator} not found within timeout ${timeout}ms`);
};

const waitForElementWithRetries = async (page: Page, locator: string, retries: number = 5) => {
    for (let i = 0; i < retries; i++) {
        try {
            await page.waitForSelector(locator, { timeout: 2000 });
            return;
        } catch (error) {
            await page.evaluate(() => window.scrollBy(0, window.innerHeight));
            await page.waitForTimeout(500);
        }
    }
    throw new Error(`Element ${locator} not found after ${retries} retries`);
};

const createUniquePost = async (page: Page, message: string) => {
    await page.fill('input', message);
    await page.click('text=Add Post');
    await scrollUntilVisible(page, `text=${message}`);
    await expect(page.locator(`text=${message}`)).toBeVisible();
    const deleteButtons = page.locator('[data-testid^="delete-button-"]');
    const count = await deleteButtons.count();
    const lastDeleteButton = deleteButtons.nth(count - 1);
    const postIdAttr = await lastDeleteButton.getAttribute('data-testid');
    const postId = postIdAttr ? postIdAttr.split('-')[2] : null;
    return postId;
};

// Tests
test('should navigate to the latest page', async ({ page }) => {
    await page.goto('/');
    await page.click('text=Latest messages');
    await expect(page).toHaveURL('/latest');
    await expect(page.locator('h1')).toContainText('Latest posts');
});

test('should create tweet', async ({ page }) => {
    await page.goto('/latest');
    await expect(page.locator('h1')).toContainText('Latest posts');
    const message = `Hello, World! ${Date.now()}`;
    await page.fill('input', message);
    await page.click('text=Add Post');
    await scrollUntilVisible(page, `text=${message}`);
    await expect(page.locator(`text=${message}`)).toBeVisible();
});

test('should like a tweet', async ({ page }) => {
    await page.goto('/latest');
    await expect(page.locator('h1')).toContainText('Latest posts');
    const message = `Hello, World! ${Date.now()}`;
    const postId = await createUniquePost(page, message);
    const likeButton = page.locator(`[data-testid=like-button-${postId}]`);
    await scrollUntilVisible(page, `[data-testid=like-button-${postId}]`);
    await likeButton.click();
    await page.waitForTimeout(5000);
    await page.reload();
    await expect(page.locator(`[data-testid=post-${postId}] span.text-gray-600.text-sm.mr-1`)).toContainText('1');
});

test('should delete a tweet', async ({ page }) => {
    await page.goto('/latest');
    await expect(page.locator('h1')).toContainText('Latest posts');
    const message = `Hello, World! ${Date.now()}`;
    const postId = await createUniquePost(page, message);
    const deleteButton = page.locator(`[data-testid=delete-button-${postId}]`).first();
    await waitForElementWithRetries(page, `[data-testid=delete-button-${postId}]`);
    await deleteButton.click();
    await page.waitForSelector(`[data-testid=post-${postId}]`, { state: 'detached', timeout: 10000 });
    await expect(page.locator(`[data-testid=post-${postId}]`)).not.toBeVisible();
});
