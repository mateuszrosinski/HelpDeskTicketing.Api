using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HelpDeskTicketing.Web.Models;
using System.Text.Json;
using System.Net.Http.Headers; // Potrzebne dla nag��wka Authorization

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
            // KROK 1: Sprawdzamy, czy u�ytkownik jest w og�le zalogowany.
            if (_httpContextAccessor.HttpContext.User?.Identity?.IsAuthenticated != true)
            {
                // Je�li nie jest, nie mamy czego wy�wietla�.
                // Mo�na go te� przekierowa� na stron� logowania: return RedirectToPage("/Login");
                return Page();
            }

            // KROK 2: Wyci�gamy token JWT z claim�w zalogowanego u�ytkownika.
            var accessToken = _httpContextAccessor.HttpContext.User.FindFirst("access_token")?.Value;

            if (string.IsNullOrEmpty(accessToken))
            {
                // To nie powinno si� zdarzy�, je�li u�ytkownik jest zalogowany, ale to dobre zabezpieczenie.
                // Oznacza to b��d logowania lub uszkodzone ciasteczko.
                return Page();
            }

            var httpClient = _httpClientFactory.CreateClient();
            var baseUrl = _configuration["ApiService:BaseUrl"];

            try
            {
                // KROK 3: Dodajemy token do nag��wka ��dania.
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
                    // Obs�uga b��du, je�li API zwr�ci np. 401 (token wygas�)
                    // Na razie po prostu nic nie wy�wietlamy.
                }
            }
            catch (Exception ex)
            {
                // Obs�uga b��du, je�li API nie odpowiada
            }

            return Page();
        }
    }
}