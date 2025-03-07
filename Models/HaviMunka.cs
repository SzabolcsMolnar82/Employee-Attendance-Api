namespace Employee_Attendance_Api.Models
{
    public class HaviMunka
    {
        public int Id { get; set; }
        public int DolgozoId { get; set; }
        public required Dolgozo Dolgozo { get; set; }
        public DateTime Datum { get; set; }
        public int LedolgozottIdoPerc { get; set; }
    }
}
