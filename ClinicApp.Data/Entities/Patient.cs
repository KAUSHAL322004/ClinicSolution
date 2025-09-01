namespace ClinicApp.Data.Entities;

public class Patient
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Phone {  get; set; }
    public string? Email { get; set; }
    public DateTime? Dob { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}