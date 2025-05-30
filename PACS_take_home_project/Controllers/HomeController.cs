using System.Data;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using PACS_take_home_project.Domain;
using PACS_take_home_project.Models;
using PACS_take_home_project.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            var timeEntryTableList = _csvDataService.GetTimeEntryTableList();
            var table = _csvDataService.CreateDataTable(timeEntryTableList, "Time Entries");

            var employeeList = _csvDataService.GetEmployees().OrderBy(x => x.FirstName).ToList();
            var model = new HomeViewModel()
            {
                TimeEntriesTable = table,
                Employees = employeeList,
            };

            return View(model);
        }

        public IActionResult FilterTableAjax(int employeeId, DateOnly? date)
        {
            var timeEntryTableList = _csvDataService.GetTimeEntryTableList(employeeId, date);
            var table = _csvDataService.CreateDataTable(timeEntryTableList, "Time Entries");

            return View("~/Views/Partials/_Report.cshtml", table);
        }

        public IActionResult InsertTimeEntryAjax(TimeEntry timeEntry, int filteredEmployeeId, DateOnly? filteredDate)
        {
            if (timeEntry.EmployeeID == 0 || timeEntry.Date == default || timeEntry.InTime == default || timeEntry.OutTime == default)
            {
                return Json(new { errorMessage = "All fields are required" });
            }
            else if (timeEntry.InTime > timeEntry.OutTime)
            {
                return Json(new { errorMessage = "In Time must be before Out Time" });
            }

            var currentTimeEntries = _csvDataService.GetTimeEntries();
            if (currentTimeEntries.Any(x => x.EmployeeID == timeEntry.EmployeeID && x.Date == timeEntry.Date))
            {
                return Json(new { errorMessage = "Only one time entry allowed per employee per date" });
            }

            _csvDataService.InsertTimeEntry(timeEntry);

            var timeEntryTableList = _csvDataService.GetTimeEntryTableList(filteredEmployeeId, filteredDate);
            var table = _csvDataService.CreateDataTable(timeEntryTableList, "Time Entries");

            return View("~/Views/Partials/_Report.cshtml", table);
        }
    }
}
