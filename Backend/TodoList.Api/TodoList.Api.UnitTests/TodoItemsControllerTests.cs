using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using TodoList.Api.Controllers;
using TodoList.Repository.Contract;
using Xunit;

namespace TodoList.Api.UnitTests
{
    public class TodoItemsControllerTests
    {
        [Fact]
        public async void Get_todoitems_should_return_all_todo_items_returned_from_the_repository()
        {
            // arrange
            var mockRepository = new Mock<ITodoItemRepository>();
            var mockToDoItems = CreateTestToDoItems();
            mockRepository
                .Setup(m => m.GetIncompleteTodoItems())
                .ReturnsAsync(mockToDoItems)
                .Verifiable(Times.Once);;
            var controller = new TodoItemsController(mockRepository.Object);

            // act
            var actionResult = await controller.GetTodoItems();

            // assert
            Assert.IsType<OkObjectResult>(actionResult);
            var okResult = actionResult as OkObjectResult;
            var expectedToDoItems = CreateTestToDoItems();
            Assert.Equivalent(okResult.Value, expectedToDoItems, strict: true);
            mockRepository.Verify();
        }

        [Fact]
        public async void Get_todoitem_should_return_todo_item_from_the_repository_that_matches_the_given_id()
        {
            // arrange
            var mockRepository = new Mock<ITodoItemRepository>();
            var mockToDoItem = CreateTestToDoItems()[1];
            mockRepository
                .Setup(m => m.GetTodoItem(mockToDoItem.Id))
                .ReturnsAsync(mockToDoItem)
                .Verifiable(Times.Once); ;
            var controller = new TodoItemsController(mockRepository.Object);

            // act
            var actionResult = await controller.GetTodoItem(mockToDoItem.Id);

            // assert
            Assert.IsType<OkObjectResult>(actionResult);
            var okResult = actionResult as OkObjectResult;
            var expectedToDoItem = CreateTestToDoItems()[1];
            Assert.Equivalent(okResult.Value, expectedToDoItem, strict: true);
            mockRepository.Verify();
        }

        [Fact]
        public async void Get_todoitem_should_return_not_found_result_when_there_is_no_todo_item_in_the_repository_that_matches_the_given_id()
        {
            // arrange
            var mockRepository = new Mock<ITodoItemRepository>();
            var toDoItemId = new Guid("c01ced37-cc45-420f-9765-6a6a6b0d535b");
            mockRepository
                .Setup(m => m.GetTodoItem(toDoItemId))
                .ReturnsAsync(null as TodoItem)
                .Verifiable(Times.Once); ;
            var controller = new TodoItemsController(mockRepository.Object);

            // act
            var actionResult = await controller.GetTodoItem(toDoItemId);

            // assert
            Assert.IsType<NotFoundResult>(actionResult);
            mockRepository.Verify();
        }

        [Fact]
        public async void Put_todoitem_should_update_the_given_todo_item_in_the_repository()
        {
            // arrange
            var mockRepository = new Mock<ITodoItemRepository>();
            var mockToDoItem = CreateTestToDoItems()[1];
            mockRepository
                .Setup(m => m.UpdateTodoItem(mockToDoItem))
                .Verifiable(Times.Once);
            var controller = new TodoItemsController(mockRepository.Object);

            // act
            var actionResult = await controller.PutTodoItem(mockToDoItem.Id, mockToDoItem);

            // assert
            Assert.IsType<NoContentResult>(actionResult);
            mockRepository.Verify();
        }

        [Fact]
        public async void Put_todoitem_should_return_bad_request_result_when_the_id_does_not_match_the_todo_item_id()
        {
            // arrange
            var mockRepository = new Mock<ITodoItemRepository>();
            var mockToDoItem = CreateTestToDoItems()[1];
            mockRepository
                .Setup(m => m.UpdateTodoItem(It.IsAny<TodoItem>()))
                .Verifiable(Times.Never);
            var controller = new TodoItemsController(mockRepository.Object);
            var nonMatchingId = new Guid("6869159c-3dd5-411a-8f07-6c68f15f453a");

            // act
            var actionResult = await controller.PutTodoItem(nonMatchingId, mockToDoItem);

            // assert
            Assert.IsType<BadRequestResult>(actionResult);
            mockRepository.Verify();
        }

        [Fact]
        public async void Put_todoitem_should_return_not_found_result_when_the_todo_item_does_not_exist_in_the_repository()
        {
            // arrange
            var mockRepository = new Mock<ITodoItemRepository>();
            var mockToDoItem = CreateTestToDoItems()[1];
            mockRepository
                .Setup(m => m.UpdateTodoItem(mockToDoItem))
                .Throws<DbUpdateConcurrencyException>()
                .Verifiable(Times.Once);
            mockRepository
                .Setup(m => m.TodoItemIdExists(mockToDoItem.Id))
                .Returns(false)
                .Verifiable(Times.Once);
            var controller = new TodoItemsController(mockRepository.Object);

            // act
            var actionResult = await controller.PutTodoItem(mockToDoItem.Id, mockToDoItem);

            // assert
            Assert.IsType<NotFoundResult>(actionResult);
            mockRepository.Verify();
        }

        [Fact]
        public async void Put_todoitem_should_throw_an_exception_when_the_repository_throws_an_error_and_the_todo_item_exists_in_the_repository()
        {
            // arrange
            var mockRepository = new Mock<ITodoItemRepository>();
            var mockToDoItem = CreateTestToDoItems()[1];
            mockRepository
                .Setup(m => m.UpdateTodoItem(mockToDoItem))
                .Throws<DbUpdateConcurrencyException>()
                .Verifiable(Times.Once);
            mockRepository
                .Setup(m => m.TodoItemIdExists(mockToDoItem.Id))
                .Returns(true)
                .Verifiable(Times.Once);
            var controller = new TodoItemsController(mockRepository.Object);

            try 
            {
                // act
                var actionResult = await controller.PutTodoItem(mockToDoItem.Id, mockToDoItem);

                // assert
                Assert.Fail("This test should have thrown an exception in the controller.");
            }
            catch (Exception) 
            {
                // assert
                mockRepository.Verify();
            }
        }

        [Fact]
        public async void Post_todoitem_should_create_a_new_todo_item_in_the_repository()
        {
            // arrange
            var mockRepository = new Mock<ITodoItemRepository>();
            var mockToDoItem = new TodoItem 
            {
                Description = "To Do Item 2"
            };
            mockRepository
                .Setup(m => m.TodoItemDescriptionExists(mockToDoItem.Description))
                .Returns(false)
                .Verifiable(Times.Once);
            mockRepository
                .Setup(m => m.CreateTodoItem(mockToDoItem))
                .ReturnsAsync(CreateTestToDoItems()[1])
                .Verifiable(Times.Once);
            var controller = new TodoItemsController(mockRepository.Object);

            // act
            var actionResult = await controller.PostTodoItem(mockToDoItem);

            // assert
            Assert.IsType<CreatedAtActionResult>(actionResult);
            var createdAtActionResult = actionResult as CreatedAtActionResult;
            var expectedToDoItem = CreateTestToDoItems()[1];
            Assert.Equivalent(createdAtActionResult.Value, expectedToDoItem, strict: true);
            mockRepository.Verify();
        }

        [Fact]
        public async void Post_todoitem_should_return_bad_request_result_if_description_is_null()
        {
            // arrange
            var mockRepository = new Mock<ITodoItemRepository>();
            var mockToDoItem = new TodoItem 
            {
                Description = null
            };
            mockRepository
                .Setup(m => m.TodoItemDescriptionExists(It.IsAny<string>()))
                .Verifiable(Times.Never);
            mockRepository
                .Setup(m => m.CreateTodoItem(It.IsAny<TodoItem>()))
                .Verifiable(Times.Never);
            var controller = new TodoItemsController(mockRepository.Object);

            // act
            var actionResult = await controller.PostTodoItem(mockToDoItem);

            // assert
            Assert.IsType<BadRequestObjectResult>(actionResult);
            var badRequestObjectResult = actionResult as BadRequestObjectResult;
            Assert.Equal(badRequestObjectResult.Value, "Description is required");
            mockRepository.Verify();
        }

        [Fact]
        public async void Post_todoitem_should_return_bad_request_result_if_description_is_an_empty_string()
        {
            // arrange
            var mockRepository = new Mock<ITodoItemRepository>();
            var mockToDoItem = new TodoItem 
            {
                Description = string.Empty
            };
            mockRepository
                .Setup(m => m.TodoItemDescriptionExists(It.IsAny<string>()))
                .Verifiable(Times.Never);
            mockRepository
                .Setup(m => m.CreateTodoItem(It.IsAny<TodoItem>()))
                .Verifiable(Times.Never);
            var controller = new TodoItemsController(mockRepository.Object);

            // act
            var actionResult = await controller.PostTodoItem(mockToDoItem);

            // assert
            Assert.IsType<BadRequestObjectResult>(actionResult);
            var badRequestObjectResult = actionResult as BadRequestObjectResult;
            Assert.Equal(badRequestObjectResult.Value, "Description is required");
            mockRepository.Verify();
        }

        [Fact]
        public async void Post_todoitem_should_return_bad_request_result_if_description_is_whitespace()
        {
            // arrange
            var mockRepository = new Mock<ITodoItemRepository>();
            var mockToDoItem = new TodoItem 
            {
                Description = "       "
            };
            mockRepository
                .Setup(m => m.TodoItemDescriptionExists(It.IsAny<string>()))
                .Verifiable(Times.Never);
            mockRepository
                .Setup(m => m.CreateTodoItem(It.IsAny<TodoItem>()))
                .Verifiable(Times.Never);
            var controller = new TodoItemsController(mockRepository.Object);

            // act
            var actionResult = await controller.PostTodoItem(mockToDoItem);

            // assert
            Assert.IsType<BadRequestObjectResult>(actionResult);
            var badRequestObjectResult = actionResult as BadRequestObjectResult;
            Assert.Equal(badRequestObjectResult.Value, "Description is required");
            mockRepository.Verify();
        }

        [Fact]
        public async void Post_todoitem_should_return_bad_request_result_if_description_already_exists_in_repository()
        {
            // arrange
            var mockRepository = new Mock<ITodoItemRepository>();
            var mockToDoItem = new TodoItem 
            {
                Description = "Here is the description"
            };
            mockRepository
                .Setup(m => m.TodoItemDescriptionExists("Here is the description"))
                .Returns(true)
                .Verifiable(Times.Once);
            mockRepository
                .Setup(m => m.CreateTodoItem(It.IsAny<TodoItem>()))
                .Verifiable(Times.Never);
            var controller = new TodoItemsController(mockRepository.Object);

            // act
            var actionResult = await controller.PostTodoItem(mockToDoItem);

            // assert
            Assert.IsType<BadRequestObjectResult>(actionResult);
            var badRequestObjectResult = actionResult as BadRequestObjectResult;
            Assert.Equal(badRequestObjectResult.Value, "Description already exists");
            mockRepository.Verify();
        }

        private List<TodoItem> CreateTestToDoItems()
        {
            return new List<TodoItem> {
                new TodoItem {
                    Id = new Guid("96974a3a-84d2-40eb-81ad-5d5471d02813"),
                    Description = "To Do Item 1",
                    IsCompleted = false
                },
                new TodoItem {
                    Id = new Guid("2ffb1402-796d-4812-bcdd-162be500b1fc"),
                    Description = "To Do Item 2",
                    IsCompleted = false
                },
                new TodoItem {
                    Id = new Guid("cc244a42-496c-4c01-82c4-511406b6dccc"),
                    Description = "To Do Item 3",
                    IsCompleted = false
                }
            };
        }
    }
}