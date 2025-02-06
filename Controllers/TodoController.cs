using Microsoft.AspNetCore.Mvc;
using TodoAPI.Contracts;
using TodoAPI.Interfaces;


namespace TodoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController(ITodoServices todoServices) : ControllerBase
    {
        private readonly ITodoServices _todoServices = todoServices;

        [HttpPost]
        public async Task<IActionResult> CreateTodoAsync(CreateTodoRequest payload)
        {
            try
            {
                await _todoServices.CreateTodoAsync(payload);
                return Ok(new { message = "Todo created successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the  crating Todo Item", error = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves all todos
        /// </summary>
        [HttpGet("getAllTodos")]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var todo = await _todoServices.GetAllAsync();
                if (todo == null || !todo.Any())
                {
                    return Ok(new { message = "No todo items found" });
                }
                return Ok(new { message = "Successfully retrieved all blog posts", data = todo });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving all Tood it posts", error = ex.Message });

            }
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            try
            {
                var todo = await _todoServices.GetByIdAsync(id);
                if (todo == null)
                {
                    return NotFound(new { message = $"No Todo item with Id {id} found." });
                }

                return Ok(new { message = $"Successfully retrieved Todo item with Id {id}.", data = todo });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An internal server error occurred. Please try again later." });
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateTodoAsync(Guid id, UpdateTodoRequest payload)
        {
            try
            {

                var todo = await _todoServices.GetByIdAsync(id);
                if (todo == null)
                {
                    return NotFound(new { message = $"Todo Item  with id {id} not found" });
                }

                await _todoServices.UpdateTodoAsync(id, payload);
                return Ok(new { message = $" Todo Item  with id {id} successfully updated" });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred while updating blog post with id {id}", error = ex.Message });
            }
        }
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteTodoAsync(Guid id)
        {
            try
            {
                await _todoServices.DeleteTodoAsync(id);
                return Ok(new { message = $"Todo  with id {id} successfully deleted" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred while deleting Todo Item  with id {id}", error = ex.Message });
            }
        }
    }
}