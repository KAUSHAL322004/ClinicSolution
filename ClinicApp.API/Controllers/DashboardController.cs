using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicApp.API.Data;

namespace ClinicApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly ClinicContext _context;

        public DashboardController(ClinicContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetStats()
        {
            var stats = new
            {
                TotalPatients = await _context.Patients.CountAsync(),
                TotalDoctors = await _context.Doctors.CountAsync(),
                UpcomingAppointments = await _context.Appointments
                    .Where(a => a.AppointmentDate >= DateTime.Today)
                    .CountAsync(),
                RecentRecords = await _context.MedicalRecords
                    .Where(r => r.RecordDate >= DateTime.Today.AddDays(-30))
                    .CountAsync()
            };

            return Ok(stats);
        }
    }
}
