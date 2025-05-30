# PACS Take Home Project
Using ASP.NET Core MVC with .NET 8.0

## Setup Instructions
- Project Requirements
  1. .NET 8 SDK
  2. Visual Studio 2022 v17.8 or later for .NET 8 support

- To run the project:
  1. Open the .sln file in Visual Studio 2022.
  2. Ensure IIS Express is selected in the run dropdown.
  3. Press F5 or click the play button.
 
## Approach
Visual Studio gave me a Models, Controller, and Views folder as default for this project.<br /><br />
I started by creating the service CSVDataService.cs and writing the methods for getting data from Employees and TimeEntries and inserting them into my Domain models. I then created the method for inserting into TimeEntries.csv. I haven't worked too much with CSV data so I had to do some reasearch on the best tools to use. I decided to use C#'s CsvHelper as it made it simple to read from and write to a csv.<br /><br />
I then worked on displaying the data to the view. I decided to pass in a DataTable object to the view to then loop through and display the data. Though not needed for this project, I decided to make my method for getting the DataTable object and the _Report.cstml view agnostic so that they could theoretically be used again to get and display more data. Using bootstrap classes I stylized the table.<br /><br />
For filtering I wanted to use the same method (GetTimeEntryTableList) as when displaying the full set of data originally. I used LINQ dot notation to splice the steps between joining and selecting with WHERE statements to filter by EmployeeID and Date if needed.<br /><br />
For insertion I decided to mix traditional form submission with and AJAX call. This way I could use the browsers built in form validation for required fields as well as having the freedom to re-render the table or display error messages without reloading the page.

## Possible Improvements
If I were to continue working on this, I would like to add a way to only show a set amount of rows in the table at a time. This could be another filter option to select how many rows to show. It would also be useful to show the total amount of rows filtered in the bottom corner of the table. I would create a way to edit or delete indivual rows. You could use bootstrap or other tools like css media queries to make the page easier to use on smaller screen sizes, like phones. I would maybe organize my CSS and Javascript into individual files, I mainly used bootstrap and inline CSS for styling.
