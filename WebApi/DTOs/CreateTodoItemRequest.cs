using System.ComponentModel.DataAnnotations;

namespace WebApi.DTOs {
    public class CreateTodoItemRequest {
        /// <summary>
        /// New todo item's text.
        /// </summary>
        [Required]
        public string Text { get; set; } = string.Empty;
    }
}
