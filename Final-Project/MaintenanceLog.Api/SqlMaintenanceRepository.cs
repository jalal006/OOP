using System.Data;
using MaintenanceLog.Contracts;
using Microsoft.Data.SqlClient;

namespace MaintenanceLog.Api;

public sealed class SqlMaintenanceRepository(string connectionString) : IMaintenanceRepository
{
    public async Task<IReadOnlyList<MaintenanceRecord>> ListAsync(string? search, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT Id, EquipmentName, IssueDescription, ReportedDate, Status
            FROM dbo.MaintenanceRecords
            WHERE @search IS NULL OR CHARINDEX(@search, EquipmentName COLLATE Latin1_General_100_CI_AS_SC) > 0
            ORDER BY ReportedDate DESC, Id DESC;
            """;

        await using var connection = new SqlConnection(connectionString);
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@search", SqlDbType.NVarChar, MaintenanceInput.EquipmentNameLimit).Value =
            string.IsNullOrWhiteSpace(search) ? DBNull.Value : search.Trim();
        await connection.OpenAsync(cancellationToken);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var records = new List<MaintenanceRecord>();
        while (await reader.ReadAsync(cancellationToken))
        {
            records.Add(ReadRecord(reader));
        }
        return records;
    }

    public async Task<MaintenanceRecord?> FindAsync(int id, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT Id, EquipmentName, IssueDescription, ReportedDate, Status
            FROM dbo.MaintenanceRecords
            WHERE Id = @id;
            """;

        await using var connection = new SqlConnection(connectionString);
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@id", SqlDbType.Int).Value = id;
        await connection.OpenAsync(cancellationToken);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? ReadRecord(reader) : null;
    }

    public async Task<MaintenanceRecord> CreateAsync(MaintenanceRecord record, CancellationToken cancellationToken)
    {
        const string sql = """
            INSERT dbo.MaintenanceRecords (EquipmentName, IssueDescription, ReportedDate, Status)
            OUTPUT INSERTED.Id
            VALUES (@equipmentName, @issueDescription, @reportedDate, @status);
            """;

        await using var connection = new SqlConnection(connectionString);
        await using var command = new SqlCommand(sql, connection);
        AddRecordParameters(command, record);
        await connection.OpenAsync(cancellationToken);
        var id = (int)(await command.ExecuteScalarAsync(cancellationToken))!;
        return record with { Id = id };
    }

    public async Task<bool> UpdateAsync(MaintenanceRecord record, CancellationToken cancellationToken)
    {
        const string sql = """
            UPDATE dbo.MaintenanceRecords
            SET EquipmentName = @equipmentName,
                IssueDescription = @issueDescription,
                ReportedDate = @reportedDate,
                Status = @status
            WHERE Id = @id;
            """;

        await using var connection = new SqlConnection(connectionString);
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@id", SqlDbType.Int).Value = record.Id;
        AddRecordParameters(command, record);
        await connection.OpenAsync(cancellationToken);
        return await command.ExecuteNonQueryAsync(cancellationToken) == 1;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        const string sql = "DELETE FROM dbo.MaintenanceRecords WHERE Id = @id;";

        await using var connection = new SqlConnection(connectionString);
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@id", SqlDbType.Int).Value = id;
        await connection.OpenAsync(cancellationToken);
        return await command.ExecuteNonQueryAsync(cancellationToken) == 1;
    }

    private static void AddRecordParameters(SqlCommand command, MaintenanceRecord record)
    {
        command.Parameters.Add("@equipmentName", SqlDbType.NVarChar, MaintenanceInput.EquipmentNameLimit).Value =
            record.EquipmentName;
        command.Parameters.Add("@issueDescription", SqlDbType.NVarChar, MaintenanceInput.IssueDescriptionLimit).Value =
            record.IssueDescription;
        command.Parameters.Add("@reportedDate", SqlDbType.Date).Value = record.ReportedDate;
        command.Parameters.Add("@status", SqlDbType.TinyInt).Value = (byte)record.Status;
    }

    private static MaintenanceRecord ReadRecord(SqlDataReader reader) =>
        new(
            reader.GetInt32(0),
            reader.GetString(1),
            reader.GetString(2),
            reader.GetFieldValue<DateOnly>(3),
            (MaintenanceStatus)reader.GetByte(4));
}
