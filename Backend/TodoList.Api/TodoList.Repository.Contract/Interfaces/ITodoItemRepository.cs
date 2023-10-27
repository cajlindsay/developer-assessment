using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TodoList.Repository.Contract
{
    public interface ITodoItemRepository
    {
        Task<List<TodoItem>> GetIncompleteTodoItems();
        Task<TodoItem> GetTodoItem(Guid id);
        Task<TodoItem> CreateTodoItem(TodoItem todoItem);
        Task UpdateTodoItem(TodoItem todoItem);
        bool TodoItemIdExists(Guid id);
        bool TodoItemDescriptionExists(string description);
    }
}