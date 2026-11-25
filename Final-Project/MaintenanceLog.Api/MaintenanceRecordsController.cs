using System.ComponentModel.DataAnnotations;
using MaintenanceLog.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace MaintenanceLog.Api;

[ApiController]
[Route("api/maintenance-records")]
public sealed class MaintenanceRecordsController(IMaintenanceRepository repository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<MaintenanceRecord>>> List(
        [FromQuery, StringLength(MaintenanceInput.EquipmentNameLimit)] string? search,
        CancellationToken cancellationToken)
    {
        return Ok(await repository.ListAsync(search, cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MaintenanceRecord>> Get(int id, CancellationToken cancellationToken)
    {
        var record = await repository.FindAsync(id, cancellationToken);
        return record is null ? NotFound() : Ok(record);
    }

    [HttpPost]
    public async Task<ActionResult<MaintenanceRecord>> Create(MaintenanceInput input, CancellationToken cancellationToken)
    {
        var record = await repository.CreateAsync(ToRecord(0, input), cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = record.Id }, record);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, MaintenanceInput input, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(ToRecord(id, input), cancellationToken)
            ? NoContent()
            : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(id, cancellationToken) ? NoContent() : NotFound();
    }

    private static MaintenanceRecord ToRecord(int id, MaintenanceInput input) =>
        new(id, input.EquipmentName.Trim(), input.IssueDescription.Trim(), input.ReportedDate!.Value, input.Status!.Value);
}
