using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HelpDeskTicketing.Web.Pages
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnPostAsync()
        {
            // Wylogowuje użytkownika, usuwając ciasteczko
            await HttpContext.SignOutAsync("Identity.Application");
            return RedirectToPage("/Index");
        }
    }
}