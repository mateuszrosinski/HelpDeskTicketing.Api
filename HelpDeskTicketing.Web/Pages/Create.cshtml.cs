using HelpDeskTicketing.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace HelpDeskTicketing.Web.Pages
{
    public class CreateModel : PageModel
    {
        // Prywatne pola do przechowywania wstrzykniêtych serwisów
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        // --- TO JEST JEDYNY KONSTRUKTOR, JAKI POWINIEN ISTNIEÆ W TEJ KLASIE ---
        public CreateModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [BindProperty]
        public CreateTicketViewModel NewTicket { get; set; }

        public void OnGet()
        {
        }

        // W pliku Pages/Create.cshtml.cs

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var httpClient = _httpClientFactory.CreateClient();
            var baseUrl = _configuration["ApiService:BaseUrl"];

            // KROK 1: Musimy do³¹czyæ token do tego ¿¹dania!
            // Bez tego API zwróci b³¹d 401 Unauthorized, a my tego nie obs³u¿yliœmy.
            var accessToken = HttpContext.User.FindFirst("access_token")?.Value;
            if (string.IsNullOrEmpty(accessToken))
            {
                // Jeœli u¿ytkownik nie jest zalogowany, przekieruj go na stronê logowania
                return RedirectToPage("/Login");
            }
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // U¿ywamy tego samego DTO, co w API
            var ticketToCreate = new
            {
                Title = NewTicket.Title,
                Description = NewTicket.Description
            };

            bool isSuccess = false; // Zmienna flagowa

            using (var jsonContent = new StringContent(JsonSerializer.Serialize(ticketToCreate), Encoding.UTF8, "application/json"))
            {
                try
                {
                    using (var httpResponseMessage = await httpClient.PostAsync($"{baseUrl}/api/tickets", jsonContent))
                    {
                        isSuccess = httpResponseMessage.IsSuccessStatusCode;
                    }
                }
                catch (Exception ex)
                {
                    // B³¹d komunikacji z API
                    ModelState.AddModelError(string.Empty, "Nie uda³o siê po³¹czyæ z serwisem API.");
                    return Page();
                }
            }

            // KROK 2: Wykonujemy przekierowanie POZA blokiem operacji sieciowych
            if (isSuccess)
            {
                return RedirectToPage("/Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Wyst¹pi³ b³¹d podczas tworzenia zg³oszenia. API odpowiedzia³o b³êdem.");
                return Page();
            }
        }
    }
}
