import { defineConfig } from '@playwright/test';
import path from 'path';

const webPort = '3000';
const baseURL = `http://localhost:${webPort}`;

export default defineConfig({
  // look for tests in the tests folder
  testDir: path.join(process.cwd(), 'tests'),
  // maximum time one test can run for.
  timeout: 30 * 1000,
  expect: {
    /**
     * maximum time expect() should wait for the condition to be met.
     * For example in `await expect(locator).toHaveText();`
     */
    timeout: 5000
  },
  // run tests in files in parallel
  fullyParallel: true,
  // fail the build on CI if you accidentally left test.only in the source code.
  forbidOnly: !!process.env.CI,
  // retry on CI only
  retries: process.env.CI ? 2 : 0,
  // opt out of parallel tests on CI
  workers: process.env.CI ? 1 : undefined,
  // reporter to use
  reporter: [
    [
      'html',
      {
        // when developing locally open the report in a browser if at least one test fails
        open: process.env.CI ? 'never' : 'on-failure',
        // drop report output to .playwright-report subfolder in project being tested
        outputFolder: path.join(process.cwd(), '.playwright-report'),
        // when running in docker set host to 0.0.0.0 so that it can serve the report outside of 
        // the docker container, but make sure that it still serves on localhost when running locally
        host: process.env.HOME === '/home/pwuser' ? '0.0.0.0' : 'localhost',
        port: 9323
      }
    ]
  ],
  use: {
    // set base url for page navigation in tests so don't have to give full url every time
    baseURL,
    // collect trace when retrying the failed test
    trace: 'on-first-retry',
    // take screenshot if one doesn't exist
    screenshot: 'only-on-failure',
    // ensure screenshots are all the same size
    viewport: { width: 1920, height: 1080 }
  },

  // configure projects for browsers under test
  projects: [
    {
      name: 'Google Chrome',
      use: {
        browserName: 'chromium'
      }
    }
  ],
  // run local dev server before starting the tests
  webServer: {
    // start the web server
    command: 'cross-env BROWSER=chrome react-scripts start',
    url: baseURL,
    timeout: 120 * 1000,
    reuseExistingServer: !process.env.CI,
    // make sure that the command runs in the context of the web app under test
    cwd: process.cwd(),
    // env variables to control the build output when in visual testing mode
    env: {
      APP_ENV: 'visual_test',
      PORT: webPort
    }
  }
});
