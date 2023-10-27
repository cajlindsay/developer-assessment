import './App.css'
import { Image, Alert, Button, Container, Row, Col, Form, Table, Stack } from 'react-bootstrap'
import React, { useState, useEffect } from 'react'
import api from './api'

export default function App() {
  const [items, setItems] = useState([])
  const [error, setError] = useState(null)

  useEffect(() => {
    getItems()
  }, [])

  async function getItems() {
    try {
      const { data } = await api.get('/toDoItems')
      data.sort((a, b) => (a.description > b.description ? -1 : 1))
      setItems(data)
    } catch (error) {
      console.error(error)
      setError('An unexpected error has occurred')
    }
  }

  return (
    <div className="App">
      <Container>
        <Row>
          <Col>
            <Image src="clearPointLogo.png" fluid rounded />
          </Col>
        </Row>
        <Row>
          <Col>
            <Alert variant="success">
              <Alert.Heading>Todo List App</Alert.Heading>
              Welcome to the ClearPoint frontend technical test. We like to keep things simple, yet clean so your
              task(s) are as follows:
              <br />
              <br />
              <ol className="list-left">
                <li>Add the ability to add (POST) a Todo Item by calling the backend API</li>
                <li>
                  Display (GET) all the current Todo Items in the below grid and display them in any order you wish
                </li>
                <li>
                  Bonus points for completing the 'Mark as completed' button code for allowing users to update and mark
                  a specific Todo Item as completed and for displaying any relevant validation errors/ messages from the
                  API in the UI
                </li>
                <li>Feel free to add unit tests and refactor the component(s) as best you see fit</li>
              </ol>
            </Alert>
            {error && <Alert variant="danger">{error}</Alert>}
          </Col>
        </Row>
        <Row>
          <Col>
            <ToDoItemForm handleToDoItemAdded={getItems} handleSetError={setError} />
          </Col>
        </Row>
        <br />
        <Row>
          <Col>
            <ToDoItemsContent items={items} handleRefreshItems={getItems} handleSetError={setError} />
          </Col>
        </Row>
      </Container>
      <footer className="page-footer font-small teal pt-4">
        <div className="footer-copyright text-center py-3">
          © 2021 Copyright:
          <a href="https://clearpoint.digital" target="_blank" rel="noreferrer">
            clearpoint.digital
          </a>
        </div>
      </footer>
    </div>
  )
}

function ToDoItemForm({ handleToDoItemAdded, handleSetError }) {
  const [description, setDescription] = useState('')

  const handleDescriptionChange = (event) => {
    setDescription(event.target.value)
  }

  async function handleAdd() {
    try {
      await api.post('/toDoItems', { description })
      handleClear()
      await handleToDoItemAdded()
    } catch (error) {
      const { response } = error
      const errorMessage = response.status === 400 ? response.data : 'An unexpected error has occurred'
      handleSetError(errorMessage)
    }
  }

  function handleClear() {
    setDescription('')
    handleSetError(null)
  }

  return (
    <Container>
      <h1>Add Item</h1>
      <Form.Group as={Row} className="mb-3" controlId="formAddTodoItem">
        <Form.Label column sm="2">
          Description
        </Form.Label>
        <Col md="6">
          <Form.Control
            type="text"
            placeholder="Enter description..."
            value={description}
            onChange={handleDescriptionChange}
          />
        </Col>
      </Form.Group>
      <Form.Group as={Row} className="mb-3 offset-md-2" controlId="formAddTodoItem">
        <Stack direction="horizontal" gap={2}>
          <Button variant="primary" onClick={() => handleAdd()}>
            Add Item
          </Button>
          <Button variant="secondary" onClick={() => handleClear()}>
            Clear
          </Button>
        </Stack>
      </Form.Group>
    </Container>
  )
}

function ToDoItemsContent({ items, handleRefreshItems, handleSetError }) {
  async function handleMarkAsComplete(item) {
    try {
      await api.put(`/toDoItems/${item.id}`, { id: item.id, isCompleted: true })
      await handleRefreshItems()
    } catch (error) {
      console.error(error)
      handleSetError('An unexpected error has occurred')
    }
  }

  return (
    <>
      <h1>
        Showing {items.length} Item(s){' '}
        <Button variant="primary" className="pull-right" onClick={() => handleRefreshItems()}>
          Refresh
        </Button>
      </h1>

      <Table striped bordered hover>
        <thead>
          <tr>
            <th>Id</th>
            <th>Description</th>
            <th>Action</th>
          </tr>
        </thead>
        <tbody>
          {items.map((item) => (
            <tr key={item.id} data-testid={`todo-item-row-${item.id}`}>
              <td>{item.id}</td>
              <td>{item.description}</td>
              <td>
                <Button variant="warning" size="sm" onClick={() => handleMarkAsComplete(item)}>
                  Mark as completed
                </Button>
              </td>
            </tr>
          ))}
        </tbody>
      </Table>
    </>
  )
}
