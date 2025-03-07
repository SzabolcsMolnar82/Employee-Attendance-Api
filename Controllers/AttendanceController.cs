using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Employee_Attendance_Api.Data;
using Employee_Attendance_Api.Models;

namespace Employee_Attendance_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AttendanceController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("check-in")]
        public async Task<IActionResult> CheckIn(int dolgozoId)
        {
            var munkaora = new Munkaora
            {
                DolgozoId = dolgozoId,
                BelepesIdo = DateTime.Now
            };
            _context.Munkaorak.Add(munkaora);
            await _context.SaveChangesAsync();

            return Ok("Bejelentkezés rögzítve!");
        }

        [HttpPost("check-out")]
        public async Task<IActionResult> CheckOut(int dolgozoId)
        {
            var munkaora = await _context.Munkaorak.FirstOrDefaultAsync(m => m.DolgozoId == dolgozoId && m.KilepesIdo == null);
            if (munkaora == null)
            {
                return NotFound("Nincs aktív műszak!");
            }

            munkaora.KilepesIdo = DateTime.Now;
            await _context.SaveChangesAsync();

            return Ok("Kijelentkezés rögzítve!");
        }
    }
}
