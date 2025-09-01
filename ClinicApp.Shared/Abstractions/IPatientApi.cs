using ClinicApp.Shared.Models;


namespace ClinicApp.Shared.Abstractions;


public interface IPatientApi
{
    Task<List<PatientDto>> GetAllAsync(CancellationToken ct = default);
    Task<PatientDto> CreateAsync(PatientDto dto, CancellationToken ct = default);
}