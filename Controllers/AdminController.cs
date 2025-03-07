using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Employee_Attendance_Api.Data;
using Employee_Attendance_Api.Models;
using Employee_Attendance_Api.DTOs;

namespace Employee_Attendance_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("add-employee")]
        public async Task<IActionResult> AddEmployee([FromBody] RegisterRequest request)
        {
            var newEmployee = new Dolgozo
            {
                Nev = request.Nev,
                FelhasznaloNev = request.FelhasznaloNev,
                JelszoHash = BCrypt.Net.BCrypt.HashPassword(request.Jelszo),
                IsAdmin = false
            };

            _context.Dolgozok.Add(newEmployee);
            await _context.SaveChangesAsync();

            return Ok("Dolgozó sikeresen hozzáadva!");
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete-employee/{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Dolgozok.FindAsync(id);
            if (employee == null)
            {
                return NotFound("Dolgozó nem található!");
            }

            _context.Dolgozok.Remove(employee);
            await _context.SaveChangesAsync();

            return Ok("Dolgozó sikeresen törölve!");
        }
    }
}
