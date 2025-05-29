using System.Data;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using PACS_take_home_project.Models;
using PACS_take_home_project.Services;

namespace PACS_take_home_project.Controllers
{
    public class HomeController : Controller
    {
        private readonly CSVDataService _csvDataService;

        public HomeController(CSVDataService csvDataService)
        {
            _csvDataService = csvDataService;
        }

        public IActionResult Index()
        {
            var employeeList = _csvDataService.GetEmployees();
            var timeEntries = _csvDataService.GetTimeEntries();

            var timeEntryTableList = employeeList
                .Join(timeEntries, e => e.EmployeeID, t => t.EmployeeID, (e, t) => new { Name = $"{e.FirstName} {e.LastName}", t })
                .Select(x => new TimeEntriesTableModel()
                {
                    Name = x.Name,
                    Date = x.t.Date,
                    InTime = x.t.InTime,
                    OutTime = x.t.OutTime,
                    TotalHours = Math.Round((x.t.OutTime - x.t.InTime).TotalMinutes / 60, 2)
                })
                .OrderByDescending(x => x.Date).ThenByDescending(x => x.OutTime);

            DataTable table = new DataTable("Time Entries");

            var columnsAdded = false;
            foreach (var timeEntry in timeEntryTableList)
            {
                var row = table.NewRow();
                foreach (var property in typeof(TimeEntriesTableModel).GetProperties())
                {
                    if (!columnsAdded)
                    {
                        table.Columns.Add(property.Name, property.PropertyType);
                    }

                    row[property.Name] = property.GetValue(timeEntry);
                }
                table.Rows.Add(row);
                columnsAdded = true;
            }

            var model = new HomeViewModel()
            {
                TimeEntriesTable = table,
                EmployeeModelList = employeeList,
            };

            return View(model);
        }
    }
}
