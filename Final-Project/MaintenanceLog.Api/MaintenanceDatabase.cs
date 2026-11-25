using Microsoft.Data.SqlClient;

namespace MaintenanceLog.Api;

internal static class MaintenanceDatabase
{
    public const string LocalConnectionString =
        @"Server=(localdb)\MSSQLLocalDB;Database=EquipmentMaintenanceLog;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";

    public static async Task InitializeLocalAsync()
    {
        var masterConnection = new SqlConnectionStringBuilder(LocalConnectionString)
        {
            InitialCatalog = "master"
        };

        await RunScriptAsync(masterConnection.ConnectionString, "01-create-local-database.sql");
        await RunScriptAsync(LocalConnectionString, "02-create-maintenance-records.sql");
    }

    private static async Task RunScriptAsync(string connectionString, string fileName)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "database", fileName);
        var sql = await File.ReadAllTextAsync(path);
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync();
    }
}
