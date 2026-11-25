# Equipment Maintenance Log

**OOP individual final project**

**Student:** Djallal Ahaddad

I will develop an Equipment Maintenance Log for a workshop technician. It will record equipment problems and track them from the initial report to resolution. A record will contain the equipment name, issue description, reported date, and status (Open, In Progress, or Resolved).

The **Home** menu will display existing records and allow the user to search by equipment name. Selecting a record will open **Details**, where the user can view, edit, or delete the record. Choosing **New** on Home will open empty Details so the user can enter and save a new record. After creating, editing, or deleting a record, the application will return to Home and refresh the list.

The project will use a C# console frontend that communicates with an ASP.NET Core Web API. The API will store records in a SQL Server database and support listing, searching, retrieving by ID, creating, updating, and deleting records. The C# implementation will organize responsibilities around a record model, an API controller, and a repository interface with a SQL Server implementation.
