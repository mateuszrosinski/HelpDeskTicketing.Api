using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HelpDeskTicketing.Web.Models;
using System.Text;
using System.Text.Json;

namespace HelpDeskTicketing.Web.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public RegisterModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [BindProperty]
        public RegisterViewModel RegisterInput { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var httpClient = _httpClientFactory.CreateClient();
            var baseUrl = _configuration["ApiService:BaseUrl"];

            var registerDto = new
            {
                Email = RegisterInput.Email,
                Username = RegisterInput.Username,
                Password = RegisterInput.Password
            };

            using var jsonContent = new StringContent(
                JsonSerializer.Serialize(registerDto),
                Encoding.UTF8,
                "application/json");

            using var httpResponseMessage = await httpClient.PostAsync($"{baseUrl}/api/accounts/register", jsonContent);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                // Po udanej rejestracji, przekieruj na stron� logowania
                return RedirectToPage("/Login");
            }

            // Je�li API zwr�ci�o b��d (np. login zaj�ty)
            ModelState.AddModelError(string.Empty, "Wyst�pi� b��d podczas rejestracji. Spr�buj ponownie.");
            return Page();
        }
    }
}