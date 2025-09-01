namespace ClinicApp.API.Models
{
    public class MedicalRecord
    {
        public int RecordId { get; set; }
        public int PatientId { get; set; }
        public string Diagnosis { get; set; } = string.Empty;
        public string Treatment { get; set; } = string.Empty;
        public DateTime RecordDate { get; set; } = DateTime.Now;
        public int DoctorId { get; internal set; }
        public object Patient { get; internal set; }
        public object Doctor { get; internal set; }
    }
}
