using Microsoft.AspNetCore.Identity;

namespace WebApi.Entities {
    public class AppUser : IdentityUser {
        public string? RefreshToken { get; set; }

        public virtual ICollection<TodoItem> TodoItems { get; set; } = new List<TodoItem>();
    }
}
