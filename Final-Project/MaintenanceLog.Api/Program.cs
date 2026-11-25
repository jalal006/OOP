using System.Text.Json.Serialization;
using MaintenanceLog.Api;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);
string? connectionString = builder.Configuration.GetConnectionString("Maintenance");
bool useLocalDb = false;
if (string.IsNullOrWhiteSpace(connectionString))
{
    connectionString = MaintenanceDatabase.LocalConnectionString;
    useLocalDb = true;
}

builder.Services.AddSingleton<IMaintenanceRepository>(new SqlMaintenanceRepository(connectionString));
builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false)));
builder.Services.AddProblemDetails();

var app = builder.Build();
app.UseExceptionHandler();
app.MapControllers();

if (useLocalDb)
{
    if (!OperatingSystem.IsWindows())
    {
        throw new InvalidOperationException(
            "SQL Server LocalDB requires Windows. Set ConnectionStrings__Maintenance to your SQL Server connection string.");
    }

    try
    {
        await MaintenanceDatabase.InitializeLocalAsync();
    }
    catch (SqlException exception)
    {
        throw new InvalidOperationException(
            "Could not initialize SQL Server LocalDB. Check that SQL Server Express LocalDB is installed and accessible.",
            exception);
    }
}

await app.RunAsync();
