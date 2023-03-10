namespace WebApi.DTOs {
    public class TodoItemDto {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// Defines if task completed or not.
        /// </summary>
        public bool IsCompleted { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
