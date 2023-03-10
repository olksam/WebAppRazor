using Microsoft.EntityFrameworkCore;

using WebApi.Data;
using WebApi.DTOs;
using WebApi.DTOs.Pagination;
using WebApi.Models;

namespace WebApi.Services
{
    public class TodoService : ITodoService {
        private readonly TodoDbContext _context;

        public TodoService(TodoDbContext context) {
            _context = context;
        }

        public async Task<TodoItemDto?> ChangeTodoItemStatus(int id, bool isCompleted) {
            var item = await _context.TodoItems.FindAsync(id);

            if (item is null) {
                return null;
            }

            item.IsCompleted = isCompleted;
            item.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            return new TodoItemDto {
                Id = item.Id,
                Text = item.Text,
                CreatedAt = item.CreatedAt,
                IsCompleted = item.IsCompleted
            };
        }

        public async Task<TodoItemDto> CreateTodoItem(CreateTodoItemRequest request) {
            var now = DateTimeOffset.UtcNow;

            var item = new TodoItem {
                Text = request.Text,
                CreatedAt = now,
                UpdatedAt = now,
                IsCompleted = false
            };

            item = _context.TodoItems.Add(item).Entity;

            await _context.SaveChangesAsync();

            return new TodoItemDto {
                Id = item.Id,
                Text = item.Text,
                CreatedAt = item.CreatedAt,
                IsCompleted = item.IsCompleted
            };
        }

        public async Task<TodoItemDto?> GetTodoItem(int id) {
            var entity = await _context.TodoItems.FindAsync(id);

            return entity != null
                ? new TodoItemDto {
                    Id = entity.Id,
                    Text = entity.Text,
                    CreatedAt = entity.CreatedAt,
                    IsCompleted = entity.IsCompleted
                } : null;
        }

        public async Task<PaginatedListDto<TodoItemDto>> GetTodoItems(int page, int pageSize, string? search, bool? isCompleted) {
            IQueryable<TodoItem> query = _context.TodoItems;

            if (!string.IsNullOrWhiteSpace(search)) {
                query = query.Where(e => e.Text.Contains(search));
            }

            if (isCompleted.HasValue) {
                query = query.Where(e => e.IsCompleted == isCompleted);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedListDto<TodoItemDto>(
                items.Select(e => new TodoItemDto {
                    Id = e.Id,
                    Text = e.Text,
                    CreatedAt = e.CreatedAt,
                    IsCompleted = e.IsCompleted
                }),
                new PaginationMeta(page, pageSize, totalCount));
        }
    }
}
