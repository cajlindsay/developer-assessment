import { test, expect } from '@playwright/test'

test('renders existing todo items on page load', async ({ page }) => {
  // arrange
  await mockExistingToDoItemsFromApi(page)

  // act
  await page.goto('./')

  // assert
  expect(await page.screenshot()).toMatchSnapshot()
})

test('user can type into and clear form fields', async ({ page }) => {
  // arrange
  await mockExistingToDoItemsFromApi(page)
  await page.goto('./')

  // act
  await page.getByLabel('Description').fill('To Do Item 1')

  // assert
  expect(await page.screenshot()).toMatchSnapshot()

  // act
  await page.getByRole('button', { name: 'Clear' }).click()

  // assert
  expect(await page.screenshot()).toMatchSnapshot()
})

test('user can add new todo items', async ({ page }) => {
  // arrange
  const toDoItems = await mockExistingToDoItemsFromApi(page)

  await page.route('https://localhost:5001/api/toDoItems', async (route) => {
    const request = route.request()
    const method = request.method()

    // mock the POST request
    if (method === 'POST') {
      toDoItems.push({ id: 'fdb7076e-5690-45f3-89ad-6dc18cf7ece0', isCompleted: false, ...request.postDataJSON() })
      return route.fulfill({ status: 200 })
    }
    // mock the GET request
    else if (method === 'GET') {
      const json = toDoItems
      await route.fulfill({ json })
    }
    // throw error if anything else
    else {
      throw new Error('unexpected request method')
    }
  })

  await page.goto('./')

  // act
  await page.getByLabel('Description').fill('A new To Do Item')
  await page.getByRole('button', { name: 'Add Item' }).click()

  // assert
  expect(await page.screenshot()).toMatchSnapshot()
})

test('user can mark todo items as completed', async ({ page }) => {
  // arrange
  const toDoItems = [
    {
      id: 'cf8f6309-a5be-48e7-8d42-cfda80ed42e0',
      description: 'To Do 1',
      isCompleted: false,
    },
    {
      id: 'ce71c28d-cc9c-4372-8586-0d5a95fbdebc',
      description: 'To Do 3',
      isCompleted: false,
    },
    {
      id: 'afe848be-a335-4ed5-8925-482195eaae03',
      description: 'To Do 2',
      isCompleted: false,
    },
  ]

  // mock GET
  await page.route('https://localhost:5001/api/toDoItems', async (route) => {
    const json = toDoItems
    await route.fulfill({ json })
  })

  // mock PUT for afe848be-a335-4ed5-8925-482195eaae03
  const toDoItemId = 'afe848be-a335-4ed5-8925-482195eaae03'
  await page.route(`https://localhost:5001/api/toDoItems/${toDoItemId}`, async (route) => {
    const request = route.request()
    const method = request.method()
    expect(method).toBe('PUT')
    const body = request.postDataJSON()
    expect(body).toStrictEqual({ id: toDoItemId, isCompleted: true })
    const itemIndex = toDoItems.findIndex((item) => item.id === body.id)
    toDoItems.splice(itemIndex, 1)
    return route.fulfill({ status: 200 })
  })

  await page.goto('./')

  // act
  await page.getByTestId(`todo-item-row-${toDoItemId}`).getByRole('button', { name: 'Mark as completed' }).click()

  // assert
  expect(await page.screenshot()).toMatchSnapshot()
})

async function mockExistingToDoItemsFromApi(page) {
  const toDoItems = [
    {
      id: 'cf8f6309-a5be-48e7-8d42-cfda80ed42e0',
      description: 'To Do 1',
      isCompleted: false,
    },
    {
      id: 'ce71c28d-cc9c-4372-8586-0d5a95fbdebc',
      description: 'To Do 3',
      isCompleted: false,
    },
    {
      id: 'afe848be-a335-4ed5-8925-482195eaae03',
      description: 'To Do 2',
      isCompleted: false,
    },
  ]

  await page.route('https://localhost:5001/api/toDoItems', async (route) => {
    const json = toDoItems
    await route.fulfill({ json })
  })

  return toDoItems
}
