using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HelpDeskTicketing.Web.Models;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace HelpDeskTicketing.Web.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public LoginModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [BindProperty]
        public LoginViewModel LoginInput { get; set; }

        public string ReturnUrl { get; set; }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var httpClient = _httpClientFactory.CreateClient();
            var baseUrl = _configuration["ApiService:BaseUrl"];

            using var jsonContent = new StringContent(
                JsonSerializer.Serialize(LoginInput),
                Encoding.UTF8,
                "application/json");

            using var httpResponseMessage = await httpClient.PostAsync($"{baseUrl}/api/accounts/login", jsonContent);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var responseBody = await httpResponseMessage.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);
                var token = tokenResponse.GetProperty("token").GetString();

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, LoginInput.Username),
                    new Claim("access_token", token) // Przechowujemy token w ciasteczku
                };

                var claimsIdentity = new ClaimsIdentity(claims, "Identity.Application");
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true, // Zapamiêtaj mnie
                    AllowRefresh = true,
                };

                await HttpContext.SignInAsync("Identity.Application", new ClaimsPrincipal(claimsIdentity), authProperties);

                return LocalRedirect(returnUrl);
            }

            ModelState.AddModelError(string.Empty, "Nieprawid³owy login lub has³o.");
            return Page();
        }
    }
}