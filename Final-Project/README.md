# Equipment Maintenance Log

A workshop technician can record equipment faults, find previous reports by equipment name, and update a report as work progresses.

## Run on Windows

1. Install the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) and [SQL Server Express LocalDB](https://learn.microsoft.com/sql/database-engine/configure-windows/sql-server-express-localdb). In Visual Studio Installer, you can select **SQL Server Express LocalDB** as an individual component. Use `sqllocaldb versions` to check that LocalDB is installed.
2. Open PowerShell in the `Final-Project` folder and start the backend.

   ```powershell
   dotnet run --project MaintenanceLog.Api
   ```

   The API starts at `http://localhost:5086`. On first run, it creates the `EquipmentMaintenanceLog` database and loads three demo records. Later starts keep your changes. The default setup uses Windows authentication.
3. Keep that terminal open. Open another PowerShell window in `Final-Project` and start the frontend.

   ```powershell
   dotnet run --project MaintenanceLog.Console
   ```

   For Home, press **S** to search by equipment name, **A** to show all records, **N** to open empty Details, or **Q** to quit. Enter a displayed record ID to open its Details.

## Use a teacher-hosted SQL Server

Send [02-create-maintenance-records.sql](database/02-create-maintenance-records.sql) to the teacher and ask them to run it inside the database assigned to the project. The script creates the table and adds demo records on the first run. The API account needs permission to read, insert, update, and delete rows in that table.

Set the connection string in the PowerShell window that will run the API. This example assumes your teacher provides a SQL username and password.

```powershell
$env:ConnectionStrings__Maintenance = 'Server=tcp:<host>,1433;Database=<database>;User Id=<user>;Password=<password>;Encrypt=True;TrustServerCertificate=False'
dotnet run --project MaintenanceLog.Api
```

The API uses the configured database with its existing schema. Store actual credentials outside GitHub. Remove the variable for the default LocalDB setup with `Remove-Item Env:ConnectionStrings__Maintenance`.

## Demonstrate the project

1. Open Home and see the demo records. Press **S** and type `conveyor` to show matching equipment names.
2. Open the example conveyor record by its ID and use **B** to return Home.
3. Press **N** on Home and **N** in Details. Create a record for **Robot Cell B**, with the issue **Gripper pressure drops**, a date in **YYYY-MM-DD** format, and status **1** (Open). Home reloads with the new record.
4. Select the new record and press **E**. Press Enter for the first three fields to keep their values, then choose **3** to mark it Resolved.
5. Select it again, press **D**, and confirm with **Y**. Home reloads without that record.

## How the code is organized

- **MaintenanceLog.Console** contains the Home and Details menus and an HTTP client.
- **MaintenanceLog.Api** contains the ASP.NET Core controller and SQL repository. It validates input and uses parameterized queries.
- **MaintenanceLog.Contracts** contains the data types shared by the two applications.
- **database** contains the SQL Server database and table scripts.

The console sends HTTP requests to `MaintenanceRecordsController`, which inherits ASP.NET Core's `ControllerBase`. The controller depends on the `IMaintenanceRepository` interface. The `SqlMaintenanceRepository` implementation handles SQL Server operations. `MaintenanceRecord` represents a saved item, and `MaintenanceInput` describes the fields required to create or edit one.

| Operation | Request |
| --- | --- |
| List all records | `GET /api/maintenance-records` |
| Search by equipment name | `GET /api/maintenance-records?search=conveyor` |
| Get one record | `GET /api/maintenance-records/{id}` |
| Create a record | `POST /api/maintenance-records` |
| Edit a record | `PUT /api/maintenance-records/{id}` |
| Delete a record | `DELETE /api/maintenance-records/{id}` |

A request body for creating or editing a record has this shape. The valid status values are `Open`, `InProgress`, and `Resolved`.

```json
{
  "equipmentName": "Robot Cell B",
  "issueDescription": "Gripper pressure drops.",
  "reportedDate": "2026-11-24",
  "status": "Open"
}
```
