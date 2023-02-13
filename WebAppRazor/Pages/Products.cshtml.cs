using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using WebAppRazor.Models;
using WebAppRazor.Services;

namespace WebAppRazor.Pages
{
    public class ProductsModel : PageModel
    {
        private readonly ProductService _productService;

        public IEnumerable<Product> Products { get; private set; } = Enumerable.Empty<Product>();

        public ProductsModel(ProductService productService) {
            _productService = productService;
        }

        public async Task OnGetAsync()
        {
            Products = await _productService.GetProductsAsync();
        }
    }
}
