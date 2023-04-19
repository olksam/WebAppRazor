using Microsoft.EntityFrameworkCore;

using WebApi.Data;
using WebApi.DTOs;
using WebApi.DTOs.Pagination;
using WebApi.Entities;

namespace WebApi.Services
{
    public class TodoService : ITodoService {
        private readonly TodoDbContext _context;
        private readonly IEmailSender _emailSender;

        public TodoService(TodoDbContext context, IEmailSender emailSender) {

            _context = context;
            _emailSender = emailSender;
        }

        public async Task<TodoItemDto?> ChangeTodoItemStatus(string userId, int id, bool isCompleted) {
            var item = await _context.TodoItems.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

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

        public async Task<TodoItemDto> CreateTodoItem(string userId, CreateTodoItemRequest request) {
            var user = await _context.Users.FindAsync(userId);
            if (user is null) {
                throw new KeyNotFoundException();
            }


            var now = DateTimeOffset.UtcNow;

            var item = new TodoItem {
                Text = request.Text,
                CreatedAt = now,
                UpdatedAt = now,
                IsCompleted = false,
                UserId = userId
            };

            item = _context.TodoItems.Add(item).Entity;

            await _context.SaveChangesAsync();

            await _emailSender.SendEmail(user.Email!, item.Text, "New todo item!");

            return new TodoItemDto {
                Id = item.Id,
                Text = item.Text,
                CreatedAt = item.CreatedAt,
                IsCompleted = item.IsCompleted
            };
        }

        public async Task<TodoItemDto?> GetTodoItem(string userId, int id) {
            var entity = await _context.TodoItems.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

            return entity != null
                ? new TodoItemDto {
                    Id = entity.Id,
                    Text = entity.Text,
                    CreatedAt = entity.CreatedAt,
                    IsCompleted = entity.IsCompleted
                } : null;
        }

        public async Task<PaginatedListDto<TodoItemDto>> GetTodoItems(string userId, int page, int pageSize, string? search, bool? isCompleted) {
            IQueryable<TodoItem> query = _context.TodoItems.Where(e => e.UserId == userId);

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
