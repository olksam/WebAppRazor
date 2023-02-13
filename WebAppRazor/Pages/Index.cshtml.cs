using Microsoft.AspNetCore.Mvc.RazorPages;

using WebAppRazor.Models;

namespace WebAppRazor.Pages {
    public class IndexModel : PageModel {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger) {
            _logger = logger;
        }

        public void OnPost(Product product) {
            var data = product;
        }
    }
}