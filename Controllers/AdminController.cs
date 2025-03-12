using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Employee_Attendance_Api.Data;
using Employee_Attendance_Api.Models;
using Employee_Attendance_Api.DTOs;
using Microsoft.EntityFrameworkCore;

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


        //Dolgozók hozzáadása admin oldalon.
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


        //Dolgozók törlése admin oldalon
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




        //Ez új, ez ahhoz kell hogy az admin oldalon megjelenítse az adatbázisban szereplő Dolgozókat (és Adminokat!) is.
        //Az isAdmin alapján kell majd eldönteni hogy bejelentkezéskor az admin oldalra irányítson át vagy a dolgozi Dashboard oldalra.
        [Authorize(Roles = "Admin")]
        [HttpGet("get-employees")]
        public async Task<IActionResult> GetEmployees([FromQuery] int? id)
        {
            if (id.HasValue)
            {
                var employee = await _context.Dolgozok
                    .Where(d => d.Id == id)
                    .Select(d => new
                    {
                        d.Id,
                        d.Nev,
                        d.FelhasznaloNev,
                        d.IsAdmin
                    })
                    .FirstOrDefaultAsync();

                if (employee == null)
                {
                    return NotFound(new { Message = "Dolgozó nem található!" });
                }

                return Ok(employee);
            }

            var employees = await _context.Dolgozok
                .Select(d => new
                {
                    d.Id,
                    d.Nev,
                    d.FelhasznaloNev,
                    d.IsAdmin
                })
                .ToListAsync();

            return Ok(employees);
        }



















        /*
        [Authorize(Roles = "Admin")]
        [HttpGet("get-employees")]
        public async Task<IActionResult> GetEmployees()
        {
            var employees = await _context.Dolgozok
                .Select(d => new
                {
                    d.Id,
                    d.Nev,
                    d.FelhasznaloNev,
                    d.IsAdmin
                })
                .ToListAsync();

            return Ok(employees);
        }
        */


    }
}
