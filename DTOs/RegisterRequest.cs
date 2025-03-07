namespace Employee_Attendance_Api.DTOs
{
    public class RegisterRequest
    {
        public required string Nev { get; set; }
        public required string FelhasznaloNev { get; set; }
        public required string Jelszo { get; set; }
    }

}
