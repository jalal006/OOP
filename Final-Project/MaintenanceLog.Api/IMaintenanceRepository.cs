using MaintenanceLog.Contracts;

namespace MaintenanceLog.Api;

public interface IMaintenanceRepository
{
    Task<IReadOnlyList<MaintenanceRecord>> ListAsync(string? search, CancellationToken cancellationToken);
    Task<MaintenanceRecord?> FindAsync(int id, CancellationToken cancellationToken);
    Task<MaintenanceRecord> CreateAsync(MaintenanceRecord record, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(MaintenanceRecord record, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
