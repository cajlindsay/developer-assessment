using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoList.Repository.Contract;

namespace TodoList.Repository.EF
{
    public class TodoItemRepository : ITodoItemRepository
    {
        private TodoContext _context;

        public TodoItemRepository(TodoContext context)
        {
            _context = context;
        }

        public async Task<List<TodoItem>> GetIncompleteTodoItems()
        {
            return await _context.TodoItems.Where(x => !x.IsCompleted).ToListAsync();
        }

        public async Task<TodoItem> GetTodoItem(Guid id)
        {
            return await _context.TodoItems.FindAsync(id);
        }

        public async Task<TodoItem> CreateTodoItem(TodoItem todoItem)
        {
            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();
            return todoItem;
        }

        public async Task UpdateTodoItem(TodoItem todoItem)
        {
            _context.Entry(todoItem).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public bool TodoItemIdExists(Guid id)
        {
            return _context.TodoItems.Any(x => x.Id == id);
        }

        public bool TodoItemDescriptionExists(string description)
        {
            return _context.TodoItems
                   .Any(x => x.Description.ToLowerInvariant() == description.ToLowerInvariant() && !x.IsCompleted);
        }
    }
}