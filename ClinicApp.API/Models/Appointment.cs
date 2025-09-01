namespace ClinicApp.API.Models
{
    public class Appointment
    {
        public int AppoinmentId { get; set; }
        public int PatientId { get; set; }
        public int Doctor {  get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = "Scheduled";
        public int AppointmentId { get; internal set; }
        public object Patient { get; internal set; }
        public int DoctorId { get; internal set; }
    }
}
