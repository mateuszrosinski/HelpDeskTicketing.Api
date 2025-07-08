using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HelpDeskTicketing.Web.Models;
using System.Text.Json;
using System.Net.Http.Headers; // Potrzebne dla nag³ówka Authorization

namespace HelpDeskTicketing.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor; // <-- NOWY SERWIS

        public IndexModel(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor) // <-- NOWY SERWIS
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor; // <-- NOWY SERWIS
        }

        public List<TicketViewModel> Tickets { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            // KROK 1: Sprawdzamy, czy u¿ytkownik jest w ogóle zalogowany.
            if (_httpContextAccessor.HttpContext.User?.Identity?.IsAuthenticated != true)
            {
                // Jeœli nie jest, nie mamy czego wyœwietlaæ.
                // Mo¿na go te¿ przekierowaæ na stronê logowania: return RedirectToPage("/Login");
                return Page();
            }

            // KROK 2: Wyci¹gamy token JWT z claimów zalogowanego u¿ytkownika.
            var accessToken = _httpContextAccessor.HttpContext.User.FindFirst("access_token")?.Value;

            if (string.IsNullOrEmpty(accessToken))
            {
                // To nie powinno siê zdarzyæ, jeœli u¿ytkownik jest zalogowany, ale to dobre zabezpieczenie.
                // Oznacza to b³¹d logowania lub uszkodzone ciasteczko.
                return Page();
            }

            var httpClient = _httpClientFactory.CreateClient();
            var baseUrl = _configuration["ApiService:BaseUrl"];

            try
            {
                // KROK 3: Dodajemy token do nag³ówka ¿¹dania.
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var httpResponseMessage = await httpClient.GetAsync($"{baseUrl}/api/tickets");

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var jsonResponse = await httpResponseMessage.Content.ReadAsStringAsync();

                    var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    Tickets = await JsonSerializer.DeserializeAsync<List<TicketViewModel>>(contentStream, options);
                }
                else
                {
                    // Obs³uga b³êdu, jeœli API zwróci np. 401 (token wygas³)
                    // Na razie po prostu nic nie wyœwietlamy.
                }
            }
            catch (Exception ex)
            {
                // Obs³uga b³êdu, jeœli API nie odpowiada
            }

            return Page();
        }
    }
}