using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Employee_Attendance_Api.Data;
using Employee_Attendance_Api.DTOs;

namespace Employee_Attendance_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var dolgozo = await _context.Dolgozok.FirstOrDefaultAsync(d => d.FelhasznaloNev == request.FelhasznaloNev);
            if (dolgozo == null || !BCrypt.Net.BCrypt.Verify(request.Jelszo, dolgozo.JelszoHash))
            {
                return Unauthorized("Érvénytelen felhasználónév vagy jelszó!");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("8[=5WQ#vDCA[g@p$YyFVYXEqm7*x)mS6");
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, dolgozo.FelhasznaloNev) };
            if (dolgozo.IsAdmin)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new { Token = tokenHandler.WriteToken(token) });
        }
    }
}
