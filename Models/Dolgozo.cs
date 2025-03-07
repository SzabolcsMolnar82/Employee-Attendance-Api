namespace Employee_Attendance_Api.Models
{
    public class Dolgozo
    {
        public int Id { get; set; }
        public required string Nev { get; set; }
        public required string FelhasznaloNev { get; set; }
        public required string JelszoHash { get; set; }
        public bool IsAdmin { get; set; }
    }
}
