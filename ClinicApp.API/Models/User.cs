namespace ClinicApp.API.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Passwordhash { get; set; } = string.Empty;
        public string Role { get; set; } = "Receptionist";


        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiry { get; set; }
        public string Password { get; internal set; }
    }
}
