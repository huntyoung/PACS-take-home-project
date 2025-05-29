using PACS_take_home_project.Domain;
using System.Data;

namespace PACS_take_home_project.Models
{
    public class HomeViewModel
    {
        public DataTable TimeEntriesTable { get; set; }
        public List<Employee> Employees { get; set; }
    }
}
