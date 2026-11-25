namespace MaintenanceLog.Contracts;

public sealed record MaintenanceRecord(
    int Id,
    string EquipmentName,
    string IssueDescription,
    DateOnly ReportedDate,
    MaintenanceStatus Status);
