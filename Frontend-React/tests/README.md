I have taken the liberty of writing my tests using Playwright and it's screenshot comparison functionality. 
In order to get consistent test results it is important to run the tests against the same browser in the 
same operating system every time. Todo this we run the tests inside the official playwright docker image.

In order to run the tests do the following:

1. Run the command ```npm run test:run-docker```. This will open an interactive shell inside the running docker container.

2. In the container, type ```cd app``` to enter the directory that the source code is mounted into.

3. Run the command ```npm run test``` to run the playwright tests.

If any of the tests fail, you can view the HTML report for the tests at http://localhost:9323