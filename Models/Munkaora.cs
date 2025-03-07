namespace Employee_Attendance_Api.Models
{
    public class Munkaora
    {
        public int Id { get; set; }
        public int DolgozoId { get; set; }
        public Dolgozo? Dolgozo { get; set; }
        public DateTime BelepesIdo { get; set; }
        public DateTime? KilepesIdo { get; set; }
    }
}
