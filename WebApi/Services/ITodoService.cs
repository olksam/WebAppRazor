using WebApi.DTOs;
using WebApi.DTOs.Pagination;

namespace WebApi.Services
{
    public interface ITodoService {
        Task<PaginatedListDto<TodoItemDto>> GetTodoItems(string userId, int page, int pageSize, string? search, bool? isCompleted);
        Task<TodoItemDto?> GetTodoItem(string userId, int id);

        Task<TodoItemDto?> ChangeTodoItemStatus(string userId, int id, bool isCompleted);

        Task<TodoItemDto> CreateTodoItem(string userId, CreateTodoItemRequest request);
    }
}
