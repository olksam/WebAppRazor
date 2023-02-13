using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebAppRazor.Pages {
    public class AboutModel : PageModel
    {
        public string UserName { get; set; } = string.Empty;


        public ActionResult OnGet(string login, string password)
        {
            if (password != "admin123") {
                return RedirectToPage("Error");
            }

            UserName = login;

            return Page();
        }
    }
}
