using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using WebApi.Entities;

namespace WebApi.Data
{
    public class TodoDbContext : IdentityDbContext<AppUser> {
        public TodoDbContext(DbContextOptions options) : base(options) {

        }

        public DbSet<TodoItem> TodoItems => Set<TodoItem>();

        public DbSet<Transaction> Transactions => Set<Transaction>();
    }
}
