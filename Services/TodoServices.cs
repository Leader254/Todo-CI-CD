using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TodoAPI.AppDataContext;
using TodoAPI.Contracts;
using TodoAPI.Interfaces;
using TodoAPI.Models;

namespace TodoAPI.Services
{
    public class TodoServices : ITodoServices
    {
        private readonly TodoDbContext _context;
        private readonly ILogger<TodoServices> _logger;
        private readonly IMapper _mapper;

        public TodoServices(TodoDbContext context, ILogger<TodoServices> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task CreateTodoAsync(CreateTodoRequest payload)
        {
            try
            {
                var todo = _mapper.Map<Todo>(payload);
                todo.CreatedAt = DateTime.UtcNow;

                // var todo = new Todo 
                // {
                //     Id = Guid.NewGuid(),
                //     Title = payload.Title,
                //     Description = payload.Description,
                //     Priority = payload.Priority,
                //     DueDate = payload.DueDate,
                //     CreatedAt = DateTime.UtcNow,
                //     UpdatedAt = DateTime.UtcNow
                // };
                _context.Todos.Add(todo);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the Todo item.");
                throw new Exception("An error occurred while creating the Todo item.");
            }
        }

        public async Task DeleteTodoAsync(Guid id)
        {
            var todo = await _context.Todos.FindAsync(id);
            if (todo != null)
            {
                _context.Todos.Remove(todo);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception($"No  item found with the id {id}");
            }
        }

        public async Task<IEnumerable<Todo>> GetAllAsync()
        {
            var todos = await _context.Todos.ToListAsync() ?? throw new Exception("No todos found");
            return todos;
        }

        public async Task<Todo> GetByIdAsync(Guid Id)
        {
            var todo = await _context.Todos.FindAsync(Id) ?? throw new KeyNotFoundException($"No Todo item with Id {Id} found.");
            return todo;
        }

        public async Task UpdateTodoAsync(Guid id, UpdateTodoRequest payload)
        {
            try
            {
                var todo = await _context.Todos.FindAsync(id);
                if (todo == null)
                {
                    _logger.LogWarning("Todo item with id {TodoId} not found", id);
                }
                if (todo != null)
                {
                    todo.Title = payload.Title ?? todo.Title;
                    todo.Description = payload.Description ?? todo.Description;
                    todo.IsComplete = payload.IsComplete ?? todo.IsComplete;
                    todo.DueDate = payload.DueDate ?? todo.DueDate;
                    todo.Priority = payload.Priority ?? todo.Priority;
                    todo.UpdatedAt = DateTime.UtcNow;

                    await _context.SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the todo item with id {TodoId}.", id);
                throw new Exception("An unexpected error occurred while updating the todo item.");
            }
        }
    }
}