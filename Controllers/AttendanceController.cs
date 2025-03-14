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


        //Dolgozó elkezdi a munkát
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

            return Ok(new
            {
                Message = "Bejelentkezés rögzítve!",
                DolgozoId = munkaora.DolgozoId,
                BelepesIdo = munkaora.BelepesIdo
            });
        }



        [HttpPost("check-out")]
        public async Task<IActionResult> CheckOut(int dolgozoId)
        {
            // Lekérdezzük a legutóbbi bejelentkezést, ahol nincs KilepesIdo beállítva
            var munkaora = await _context.Munkaorak
                .Where(m => m.DolgozoId == dolgozoId && m.KilepesIdo == null)
                .OrderByDescending(m => m.BelepesIdo)
                .FirstOrDefaultAsync();

            if (munkaora == null)
            {
                return NotFound(new { Message = "Nincs aktív műszak ehhez a dolgozóhoz!" });
            }

            // Beállítjuk a kilépési időt
            munkaora.KilepesIdo = DateTime.Now;

            // Kiszámoljuk a ledolgozott időt percben és mentjük a HaviMunka táblába
            var ledolgozottIdoPerc = (int)(munkaora.KilepesIdo - munkaora.BelepesIdo).Value.TotalMinutes;




            // Töltsük be a dolgozót
            var dolgozo = await _context.Dolgozok.FindAsync(dolgozoId);
            if (dolgozo == null)
            {
                return NotFound(new { Message = "Dolgozó nem található!" });
            }

            var haviMunka = await _context.HaviMunka
                .Include(h => h.Dolgozo) // Betöltjük a dolgozó objektumot is
                .FirstOrDefaultAsync(h => h.DolgozoId == dolgozoId && h.Datum.Date == DateTime.Today);

            if (haviMunka != null)
            {
                haviMunka.LedolgozottIdoPerc += ledolgozottIdoPerc;
            }
            else
            {
                _context.HaviMunka.Add(new HaviMunka
                {
                    DolgozoId = dolgozoId,
                    Dolgozo = dolgozo, // Most már kötelezően beállítjuk
                    Datum = DateTime.Today,
                    LedolgozottIdoPerc = ledolgozottIdoPerc
                });
            }

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Kijelentkezés rögzítve!", LedolgozottIdoPerc = ledolgozottIdoPerc });
        }


        /*
        //Dolgozó befejezi a munkát
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
        */

        [HttpGet("{dolgozoId}")]
        public async Task<IActionResult> GetAttendance(int dolgozoId)
        {
            Console.WriteLine($"🔍 getAttendance hívás dolgozoId: {dolgozoId}");

            // Lekérjük a dolgozóhoz tartozó jelenlét adatokat, idő szerint csökkenő sorrendben
            var jelenletek = await _context.Munkaorak
                .Where(m => m.DolgozoId == dolgozoId)
                .OrderByDescending(m => m.BelepesIdo)
                .Select(m => new
                {
                    m.Id,
                    m.DolgozoId,
                    BelepesIdo = m.BelepesIdo.ToString("yyyy-MM-dd HH:mm:ss"),
                    KilepesIdo = m.KilepesIdo.HasValue ? m.KilepesIdo.Value.ToString("yyyy-MM-dd HH:mm:ss") : null
                })
                .ToListAsync();

            if (!jelenletek.Any())
            {
                Console.WriteLine($"⚠️ Nincs jelenlét adat ehhez a dolgozóhoz! ({dolgozoId})");
                return NotFound(new { Message = "Nincs jelenlét adat ehhez a dolgozóhoz!" });
            }

            Console.WriteLine($"📥 {jelenletek.Count} jelenlét adat betöltve dolgozóhoz: {dolgozoId}");

            return Ok(jelenletek);

        }


        //Havi munkaórák lekérdezése
        [HttpGet("monthly-work/{dolgozoId}")]
        public async Task<IActionResult> GetMonthlyWork(int dolgozoId)
        {
            // Először lekérdezzük az összes adatot az adatbázisból
            var munkaorak = await _context.Munkaorak
                .Where(m => m.DolgozoId == dolgozoId && m.KilepesIdo != null)  // Csak a lezárt műszakokat vesszük figyelembe
                .ToListAsync();

            // Csoportosítás dátum szerint és összes ledolgozott idő kiszámítása
            var haviMunka = munkaorak
                .AsEnumerable()  // Memóriában történő feldolgozás miatt szükséges!
                .GroupBy(m => m.BelepesIdo.Date)  // Csoportosítás nap szerint
                .Select(g => new
                {
                    Datum = g.Key,
                    LedolgozottIdoPerc = g.Sum(m => (int)(m.KilepesIdo.Value - m.BelepesIdo).TotalMinutes)  // Időkülönbség percben
                })
                .OrderBy(g => g.Datum)
                .ToList();

            return Ok(new
            {
                Days = haviMunka,
                TotalDaysWorked = haviMunka.Count  // Összes ledolgozott nap
            });
        }











        /*
        [HttpGet("monthly-work/{dolgozoId}")]
        public async Task<IActionResult> GetMonthlyWork(int dolgozoId)
        {
            var haviMunka = await _context.HaviMunka
                .Where(h => h.DolgozoId == dolgozoId && h.Datum.Month == DateTime.Now.Month)
                .Select(h => new
                {
                    h.Datum,
                    h.LedolgozottIdoPerc
                })
                .ToListAsync();

            var totalDaysWorked = haviMunka.Count;

            return Ok(new
            {
                Days = haviMunka,
                TotalDaysWorked = totalDaysWorked
            });
        }
        */


    }
}
