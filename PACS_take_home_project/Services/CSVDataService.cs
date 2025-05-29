using System.Formats.Asn1;
using System.Globalization;
using CsvHelper;
using System.IO;
using System.Linq;
using CsvHelper.TypeConversion;
using CsvHelper.Configuration;
using PACS_take_home_project.Domain;

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

        public void InsertTimeEntry(TimeEntry timeEntryModel)
        {
            // Grab current time entries and append new time entry
            var timeEntriesList = GetTimeEntries();
            if (timeEntriesList.Any())
            {
                timeEntryModel.EntryID = timeEntriesList.Last().EntryID + 1;
            }
            timeEntriesList.Add(timeEntryModel);

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
    }
}
