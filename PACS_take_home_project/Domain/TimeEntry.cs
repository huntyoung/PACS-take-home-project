namespace PACS_take_home_project.Domain
{
    public record TimeEntry
    {
        public int EntryID { get; set; }
        public int EmployeeID { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly InTime { get; set; }
        public TimeOnly OutTime { get; set; }
    }
}
