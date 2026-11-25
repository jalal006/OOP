# Equipment Maintenance Log

A C# console app for recording equipment faults and tracking their status. The app uses an ASP.NET Core Web API and SQL Server.

## Run on Windows

Install the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) and [SQL Server Express LocalDB](https://learn.microsoft.com/sql/database-engine/configure-windows/sql-server-express-localdb).

In `Final-Project`, start the API:

```powershell
dotnet run --project MaintenanceLog.Api
```

The API listens on `http://localhost:5086` and creates the local database with three sample records on first run.

In a second terminal in `Final-Project`, start the console app:

```powershell
dotnet run --project MaintenanceLog.Console
```

## Controls

- **Home** — Enter a record ID to open Details. Press **S** to search by equipment name, **A** to show all records, **N** to add a record, or **Q** to quit.
- **Details** — Press **E** to edit an existing record, **D** to delete it, **N** to enter a new record, or **B** to go back. The list refreshes after each change.

## Project structure

- `MaintenanceLog.Console` — Home and Details menus and HTTP client
- `MaintenanceLog.Api` — API controller, repository interface, and SQL Server implementation
- `MaintenanceLog.Contracts` — Shared record and request types
- `database` — SQL Server scripts
