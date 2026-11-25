namespace MaintenanceLog.Contracts;

public class MaintenanceRecord
{
    public int Id { get; set; }
    public string EquipmentName { get; set; } = "";
    public string IssueDescription { get; set; } = "";
    public DateOnly ReportedDate { get; set; }
    public MaintenanceStatus Status { get; set; }
}
