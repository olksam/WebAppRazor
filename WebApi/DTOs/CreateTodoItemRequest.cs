using System.ComponentModel.DataAnnotations;

namespace WebApi.DTOs {
    public class CreateTodoItemRequest {
        /// <summary>
        /// New todo item's text.
        /// </summary>
        [Required]
        [MinLength(5)]
        public string Text { get; set; } = string.Empty;
    }
}
