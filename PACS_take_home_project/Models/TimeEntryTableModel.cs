namespace PACS_take_home_project.Models
{
    public class TimeEntryTableModel
    {
        public string Name { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly InTime { get; set; }
        public TimeOnly OutTime { get; set; }
        public double TotalHours { get; set; }
    }
}
