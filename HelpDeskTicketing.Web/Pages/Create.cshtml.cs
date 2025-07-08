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
        // Prywatne pola do przechowywania wstrzykni�tych serwis�w
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        // --- TO JEST JEDYNY KONSTRUKTOR, JAKI POWINIEN ISTNIE� W TEJ KLASIE ---
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

            // KROK 1: Musimy do��czy� token do tego ��dania!
            // Bez tego API zwr�ci b��d 401 Unauthorized, a my tego nie obs�u�yli�my.
            var accessToken = HttpContext.User.FindFirst("access_token")?.Value;
            if (string.IsNullOrEmpty(accessToken))
            {
                // Je�li u�ytkownik nie jest zalogowany, przekieruj go na stron� logowania
                return RedirectToPage("/Login");
            }
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // U�ywamy tego samego DTO, co w API
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
                    // B��d komunikacji z API
                    ModelState.AddModelError(string.Empty, "Nie uda�o si� po��czy� z serwisem API.");
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
                ModelState.AddModelError(string.Empty, "Wyst�pi� b��d podczas tworzenia zg�oszenia. API odpowiedzia�o b��dem.");
                return Page();
            }
        }
    }
}
