using Microsoft.EntityFrameworkCore;

using WebApi.Models;

namespace WebApi.Data {
    public class TodoDbContext : DbContext {
        public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options) {

        }

        public DbSet<TodoItem> TodoItems => Set<TodoItem>();
    }
}
