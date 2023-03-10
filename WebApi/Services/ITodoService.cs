using WebApi.DTOs;
using WebApi.DTOs.Pagination;
using WebApi.Models;

namespace WebApi.Services
{
    public interface ITodoService {
        Task<PaginatedListDto<TodoItemDto>> GetTodoItems(int page, int pageSize, string? search, bool? isCompleted);
        Task<TodoItemDto?> GetTodoItem(int id);

        Task<TodoItemDto?> ChangeTodoItemStatus(int id, bool isCompleted);

        Task<TodoItemDto> CreateTodoItem(CreateTodoItemRequest request);
    }
}
