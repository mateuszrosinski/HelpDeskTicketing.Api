using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HelpDeskTicketing.Web.Models;
using System.Text.Json;

namespace HelpDeskTicketing.Web.Pages
{
    public class DetailsModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public DetailsModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public TicketViewModel Ticket { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var httpClient = _httpClientFactory.CreateClient();
            var baseUrl = _configuration["ApiService:BaseUrl"];
            
            try
            {
                var httpResponseMessage = await httpClient.GetAsync($"{baseUrl}/api/tickets/{id}");
                
                if(httpResponseMessage.IsSuccessStatusCode)
                {
                    var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                    Ticket = await JsonSerializer.DeserializeAsync<TicketViewModel>(contentStream, options);

                    if (Ticket == null)
                    {
                        return NotFound();
                    }

                    return Page();
                }
                else
                {
                     return NotFound(); 
                }
            }
            catch (Exception ex) 
                {
                return RedirectToPage("/Index");
                }
        }

        public void OnGet()
        {
        }
    }
}
