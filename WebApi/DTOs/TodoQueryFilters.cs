using Microsoft.AspNetCore.Mvc;

namespace WebApi.DTOs {
    public class TodoQueryFilters {
        /// <summary>
        /// Search by text.
        /// </summary>
        [FromQuery(Name = "search")]
        public string? Search { get; set; }

        /// <summary>
        /// If todo item is completed or not.
        /// </summary>
        [FromQuery(Name = "completed")]
        public bool? Completed { get; set; }
    }
}
