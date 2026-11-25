using System.ComponentModel.DataAnnotations;

namespace MaintenanceLog.Contracts;

public sealed class MaintenanceInput
{
    public const int EquipmentNameLimit = 100;
    public const int IssueDescriptionLimit = 500;

    [Required, StringLength(EquipmentNameLimit)]
    public string EquipmentName { get; init; } = "";

    [Required, StringLength(IssueDescriptionLimit)]
    public string IssueDescription { get; init; } = "";

    [Required]
    public DateOnly? ReportedDate { get; init; }

    [Required, EnumDataType(typeof(MaintenanceStatus))]
    public MaintenanceStatus? Status { get; init; }
}
