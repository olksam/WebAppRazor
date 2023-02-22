using Microsoft.EntityFrameworkCore;

using WebAppMvc.Models;

namespace WebAppMvc.Data {
    public class WebAppMvcContext : DbContext
    {
        public WebAppMvcContext (DbContextOptions<WebAppMvcContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products => Set<Product>();

        public DbSet<WebAppMvc.Models.Category> Category { get; set; } = default!;
    }
}
