The project support timetable for small school implement by Blazor Server, SQLite, [Radzen](https://blazor.radzen.com/)

![Application Demo](./asserts/demo-app.jpeg)

Note feature:

- Students
  - View (Support filter - Done)
  - Create (Done)
  - Update (Done)
  - Delete (Done)
- Teacher
  - View (Support filter - Done)
  - Create (Done)
  - Update (Done)
  - Delete (Done)
- Subject
  - View (Support filter - Done)
  - Create (Done)
  - Update (Done)
  - Delete (Done)
- Room
  - View
  - Create (Done)
  - Update (Done)
  - Delete (Done)
- Time Table
  - View
  - Create
  - Edit
- Import (Done Import using CSV file - file refer as [`TimeTable.Blazor\Template`]) - TBC Validation input.
  - Student
  - Teacher
  - Room
  - Subject
  - Timetable

- Export timetable to CSV (TBC)
- Application Unit Test

How to run the project.

1. Install [.NET 7](https://dotnet.microsoft.com/en-us/download) (for Build apps - SDK or ASP.NET Core Runtime 7.0.10)

2. Run command create database.

Use Visual Studio or Visual Code

```Powershell
update-database ## Create database
## OR
dotnet tool install --global dotnet-ef # If you misspelled a built-in dotnet command.
dotnet ef database update

## Start application
## Visual Code
cd TimeTable.Blazor
dotnet run
```

Noted: **The project support dev container and can run in workspace**.