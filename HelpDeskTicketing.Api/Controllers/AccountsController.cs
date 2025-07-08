using HelpDeskTicketing.Api.Controllers;
using HelpDeskTicketing.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HelpDeskTicketing.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration; // ZMIANA: Dodajemy IConfiguration

        // ZMIANA: Modyfikujemy konstruktor (SignInManager nie jest już potrzebny do samego logowania z hasłem)
        public AccountsController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        // POST: api/accounts/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser
            {
                UserName = registerDto.Username,
                Email = registerDto.Email
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (result.Succeeded)
            {
                return StatusCode(201);
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }

            return BadRequest(ModelState);
        }


        // POST: api/accounts/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Krok 1: Znajdź użytkownika po nazwie
            var user = await _userManager.FindByNameAsync(loginDto.Username);

            // Krok 2: Sprawdź, czy użytkownik istnieje ORAZ czy hasło jest poprawne
            if (user != null && await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                // Krok 3: Jeśli dane są poprawne, wygeneruj token
                var tokenString = GenerateJwtToken(user);
                return Ok(new { token = tokenString });
            }

            // Jeśli dane są niepoprawne, zwróć błąd
            return Unauthorized();
        }

        // Prywatna metoda do generowania tokenu
        private string GenerateJwtToken(ApplicationUser user)
        {
            var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.Now.AddHours(3), // Czas wygaśnięcia tokenu
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
