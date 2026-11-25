using System.ComponentModel.DataAnnotations;

namespace MaintenanceLog.Contracts;

public class MaintenanceInput
{
    public const int EquipmentNameLimit = 100;
    public const int IssueDescriptionLimit = 500;

    [Required, StringLength(EquipmentNameLimit)]
    public string EquipmentName { get; set; } = "";

    [Required, StringLength(IssueDescriptionLimit)]
    public string IssueDescription { get; set; } = "";

    [Required]
    public DateOnly? ReportedDate { get; set; }

    [Required, EnumDataType(typeof(MaintenanceStatus))]
    public MaintenanceStatus? Status { get; set; }
}
