using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using TodoList.Repository.Contract;

namespace TodoList.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly ITodoItemRepository _repository;

        public TodoItemsController(ITodoItemRepository repository)
        {
            _repository = repository;
        }

        // GET: api/TodoItems
        [HttpGet]
        public async Task<IActionResult> GetTodoItems()
        {
            var results = await _repository.GetIncompleteTodoItems();
            return Ok(results);
        }

        // GET: api/TodoItems/...
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTodoItem(Guid id)
        {
            var result = await _repository.GetTodoItem(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        // PUT: api/TodoItems/... 
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(Guid id, TodoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return BadRequest();
            }

            try
            {
                await _repository.UpdateTodoItem(todoItem);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_repository.TodoItemIdExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
    
            return NoContent();
        }

        // POST: api/TodoItems 
        [HttpPost]
        public async Task<IActionResult> PostTodoItem(TodoItem todoItem)
        {
            if (string.IsNullOrWhiteSpace(todoItem?.Description))
            {
                return BadRequest("Description is required");
            }
            else if (_repository.TodoItemDescriptionExists(todoItem.Description))
            {
                return BadRequest("Description already exists");
            }

            var newToDoItem = await _repository.CreateTodoItem(todoItem);

            return CreatedAtAction(nameof(GetTodoItem), new { id = newToDoItem.Id }, newToDoItem);
        }
    }
}
