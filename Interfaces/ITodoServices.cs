using TodoAPI.Contracts;
using TodoAPI.Models;

namespace TodoAPI.Interfaces
{
    public interface ITodoServices
    {
        Task<IEnumerable<Todo>> GetAllAsync();
        Task<Todo> GetByIdAsync(Guid Id);
        Task CreateTodoAsync(CreateTodoRequest payload);
        Task UpdateTodoAsync(Guid id, UpdateTodoRequest payload);
        Task DeleteTodoAsync(Guid id);
    }
}