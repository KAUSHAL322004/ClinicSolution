using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicApp.API.Data;
using ClinicApp.API.Models;

namespace ClinicApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicalRecordsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MedicalRecordsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/medicalrecords
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MedicalRecord>>> GetMedicalRecords()
        {
            return await _context.MedicalRecords
                .Include(r => r.Patient)
                .Include(r => r.Doctor)
                .ToListAsync();
        }

        // GET: api/medicalrecords/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MedicalRecord>> GetMedicalRecord(int id)
        {
            var record = await _context.MedicalRecords
                .Include(r => r.Patient)
                .Include(r => r.Doctor)
                .FirstOrDefaultAsync(r => r.RecordId == id);

            if (record == null)
            {
                return NotFound();
            }

            return record;
        }

        // POST: api/medicalrecords
        [HttpPost]
        public async Task<ActionResult<MedicalRecord>> PostMedicalRecord(MedicalRecord record)
        {
            // check patient & doctor validity
            if (!_context.Patients.Any(p => p.PatientId == record.PatientId) ||
                !_context.Doctors.Any(d => d.DoctorId == record.DoctorId))
            {
                return BadRequest("Invalid PatientId or DoctorId");
            }

            _context.MedicalRecords.Add(record);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMedicalRecord), new { id = record.RecordId }, record);
        }

        // PUT: api/medicalrecords/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMedicalRecord(int id, MedicalRecord record)
        {
            if (id != record.RecordId)
            {
                return BadRequest();
            }

            _context.Entry(record).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.MedicalRecords.Any(e => e.RecordId == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/medicalrecords/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedicalRecord(int id)
        {
            var record = await _context.MedicalRecords.FindAsync(id);
            if (record == null)
            {
                return NotFound();
            }

            _context.MedicalRecords.Remove(record);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
