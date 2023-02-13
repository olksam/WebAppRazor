using Microsoft.AspNetCore.Mvc.RazorPages;

using WebAppRazor.Models;
using WebAppRazor.Services;

namespace WebAppRazor.Pages {
    public class ProductModel : PageModel
    {
        private readonly ProductService _productService;

        public ProductModel(ProductService productService) {
            _productService = productService;
        }

        public Product? Product { get; private set; }

        public async Task OnGetAsync(int id)
        {
            Product = await _productService.GetProductByIdAsync(id);
        }
    }
}
