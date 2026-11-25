using System.ComponentModel.DataAnnotations;
using MaintenanceLog.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace MaintenanceLog.Api;

[ApiController]
[Route("api/maintenance-records")]
public class MaintenanceRecordsController : ControllerBase
{
    private readonly IMaintenanceRepository repository;

    public MaintenanceRecordsController(IMaintenanceRepository repository)
    {
        this.repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<MaintenanceRecord>>> List(
        [FromQuery, StringLength(MaintenanceInput.EquipmentNameLimit)] string? search,
        CancellationToken cancellationToken)
    {
        var records = await repository.ListAsync(search, cancellationToken);
        return Ok(records);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MaintenanceRecord>> Get(int id, CancellationToken cancellationToken)
    {
        MaintenanceRecord? record = await repository.FindAsync(id, cancellationToken);
        if (record == null)
        {
            return NotFound();
        }
        return Ok(record);
    }

    [HttpPost]
    public async Task<ActionResult<MaintenanceRecord>> Create(MaintenanceInput input, CancellationToken cancellationToken)
    {
        MaintenanceRecord record = ToRecord(0, input);
        MaintenanceRecord savedRecord = await repository.CreateAsync(record, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = savedRecord.Id }, savedRecord);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, MaintenanceInput input, CancellationToken cancellationToken)
    {
        MaintenanceRecord record = ToRecord(id, input);
        bool updated = await repository.UpdateAsync(record, cancellationToken);
        if (!updated)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        bool deleted = await repository.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }

    private static MaintenanceRecord ToRecord(int id, MaintenanceInput input)
    {
        // ASP.NET Core checks the required date and status before calling the controller.
        return new MaintenanceRecord
        {
            Id = id,
            EquipmentName = input.EquipmentName.Trim(),
            IssueDescription = input.IssueDescription.Trim(),
            ReportedDate = input.ReportedDate!.Value,
            Status = input.Status!.Value
        };
    }
}
