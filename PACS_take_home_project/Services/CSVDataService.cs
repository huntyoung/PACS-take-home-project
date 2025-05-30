using System.Formats.Asn1;
using System.Globalization;
using CsvHelper;
using System.IO;
using System.Linq;
using CsvHelper.TypeConversion;
using CsvHelper.Configuration;
using PACS_take_home_project.Domain;
using PACS_take_home_project.Models;
using System.Data;

namespace PACS_take_home_project.Services
{
    public class CSVDataService
    {
        private const string EmployeesFilePath = "Data/Employees.csv";
        private const string TimeEntriesFilePath = "Data/TimeEntries.csv";

        public CSVDataService() { }

        public List<Employee> GetEmployees()
        {
            using var streamReader = File.OpenText(EmployeesFilePath);
            using var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture);

            // Get records from Employees.csv converted to EmployeeModels
            var employees = csvReader.GetRecords<Employee>().ToList();

            return employees;
        }

        public List<TimeEntry> GetTimeEntries()
        {
            using var streamReader = File.OpenText(TimeEntriesFilePath);
            using var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture);

            // Get records from TimeEntries.csv converted to TimeEntryModels
            var timeEntries = csvReader.GetRecords<TimeEntry>().ToList();

            return timeEntries;
        }

        public void InsertTimeEntry(TimeEntry timeEntry)
        {
            // Grab current time entries and append new time entry
            var timeEntriesList = GetTimeEntries();
            if (timeEntriesList.Any())
            {
                timeEntry.EntryID = timeEntriesList.Last().EntryID + 1;
            }
            timeEntriesList.Add(timeEntry);

            using var streamWriter = new StreamWriter(TimeEntriesFilePath);
            using var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);

            // Add DateOnly and TimeOnly format for writing to csv
            var dateOnlyOptions = new TypeConverterOptions { Formats = ["MM/dd/yyyy"] };
            csvWriter.Context.TypeConverterOptionsCache.AddOptions<DateOnly>(dateOnlyOptions);

            var timeOnlyOptions = new TypeConverterOptions { Formats = ["HH:mm"] };
            csvWriter.Context.TypeConverterOptionsCache.AddOptions<TimeOnly>(timeOnlyOptions);

            // Write over csv with all records
            csvWriter.WriteRecords(timeEntriesList);
        }

        public List<TimeEntryTableModel> GetTimeEntryTableList(int employeeId = 0, DateOnly? date = null)
        {
            var employeeList = GetEmployees();
            var timeEntries = GetTimeEntries();

            var query = employeeList.Join(timeEntries, e => e.EmployeeID, t => t.EmployeeID, (e, t) => new { e, t });

            if (employeeId > 0) query = query.Where(x => x.e.EmployeeID == employeeId);
            if (date != null) query = query.Where(x => x.t.Date == date);

            var timeEntryTableList = query.Select(x => 
                new TimeEntryTableModel()
                {
                    Name = $"{x.e.FirstName} {x.e.LastName}",
                    Date = x.t.Date,
                    InTime = x.t.InTime,
                    OutTime = x.t.OutTime,
                    TotalHours = Math.Round((x.t.OutTime - x.t.InTime).TotalMinutes / 60, 2)
                })
                .OrderByDescending(x => x.Date).ThenByDescending(x => x.OutTime)
                .ToList();

            return timeEntryTableList;
        }

        public DataTable CreateDataTable<T>(List<T> tableModelList, string tableName)
        {
            DataTable table = new DataTable(tableName);

            var columnsAdded = false;
            foreach (var tableModel in tableModelList)
            {
                var row = table.NewRow();
                foreach (var property in typeof(T).GetProperties())
                {
                    if (!columnsAdded)
                    {
                        table.Columns.Add(property.Name, property.PropertyType);
                    }

                    row[property.Name] = property.GetValue(tableModel);
                }
                table.Rows.Add(row);
                columnsAdded = true;
            }

            return table;
        }
    }
}
