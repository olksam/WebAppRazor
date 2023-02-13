using System.ComponentModel.DataAnnotations;

using WebAppRazor.TagHelpers;

namespace WebAppRazor.Models {
    public class Product {
        [FormIgnore]
        public int Id { get; set; }

        [Required, MinLength(4), MaxLength(128)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(256)]
        public string Description { get; set; } = string.Empty;

        public uint Count { get; set; }

        [Range(0, int.MaxValue)]
        public decimal Price { get; set; }
        public bool Available { get; set; }
    }
}
