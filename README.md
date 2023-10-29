Hi there! 

I have completed the assessment with the following changes:

## Backend
- Refactored the data access code into separate projects. There is a repository contract project containing the models and interfaces, and a repository Entity Framework project that contains the EF implmentation of the repository contract.
- Updated the api to use the new repository contract. I have used dependency injection to inject the EF repository implementation into the TodoItemsController.
- Added unit tests for the TodoItemsController using XUnit as the test runner and Moq as the mocking framework.

## React Frontend
- Implemented all of the functionality requested to make the user interface work.
- Some minor refactoring to add a little clarity to the App component.
- Added visual snapshot tests using Playwright. I find visual snapshot tests to be thorough, easy to write and an interesting discussion topic so take a look and see what you think.

## Angular Frontend
- Removed, as not needed.

## NOTES on running the Playwright tests
The Playwright tests take visual snapshots of the screen as the user would see them and compare them against stored snapshots to validate that they are the same. The naming convention of the stored snapshots includes both the browser and the operating system that the tests are running on. So that all developers can run the tests regardless of their OS and browser I have configured the tests to run inside the official playwright docker image.

To run the Playwright tests run the following commands in the Frontend-React project:

1. ```npm run start:playwright``` to open an interactive shell inside the running playwright docker container.

3. ```npm run test``` to run the playwright tests.

If any of the tests fail, you can view the HTML report and visual snapshot comparisons at http://localhost:9323