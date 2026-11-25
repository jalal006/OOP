using System.Globalization;
using MaintenanceLog.Contracts;

namespace MaintenanceLog.Client;

internal sealed class MaintenanceMenu(MaintenanceApiClient api)
{
    public async Task RunAsync()
    {
        string? search = null;
        while (true)
        {
            try
            {
                var records = await api.ListAsync(search);
                ShowHome(records, search);

                var choice = Read("Choose an option or enter a record ID: ").ToUpperInvariant();
                switch (choice)
                {
                    case "Q":
                        return;
                    case "A":
                        search = null;
                        break;
                    case "S":
                        var term = Read("Equipment name (Enter to show all): ");
                        if (term.Length <= MaintenanceInput.EquipmentNameLimit)
                        {
                            search = string.IsNullOrEmpty(term) ? null : term;
                        }
                        else
                        {
                            Console.WriteLine("Search text must be 100 characters or fewer.");
                        }
                        break;
                    case "N":
                        if (await ShowDetailsAsync(null))
                        {
                            search = null;
                        }
                        break;
                    default:
                        if (int.TryParse(choice, out var id) && records.Any(record => record.Id == id))
                        {
                            var record = await api.GetAsync(id);
                            if (record is null)
                            {
                                Console.WriteLine("That record is no longer available.");
                            }
                            else if (await ShowDetailsAsync(record))
                            {
                                search = null;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Choose a menu option or an ID from the list.");
                        }
                        break;
                }
            }
            catch (HttpRequestException exception)
            {
                var message = exception.StatusCode is { } status
                    ? $"The API returned HTTP {(int)status} ({status})."
                    : "Could not reach the API. Check that MaintenanceLog.Api is running.";
                if (!Retry(message))
                {
                    return;
                }
            }
            catch (TaskCanceledException)
            {
                if (!Retry("The API request timed out."))
                {
                    return;
                }
            }
        }
    }

    private static void ShowHome(IReadOnlyList<MaintenanceRecord> records, string? search)
    {
        Console.WriteLine();
        Console.WriteLine("=== Equipment Maintenance Log | Home ===");
        Console.WriteLine(search is null ? "All records" : $"Search results for \"{search}\"");
        if (records.Count == 0)
        {
            Console.WriteLine("No records found.");
        }
        foreach (var record in records)
        {
            Console.WriteLine($"{record.Id,4}  {record.ReportedDate:yyyy-MM-dd}  "
                + $"[{StatusLabel(record.Status)}]  {record.EquipmentName}");
        }
        Console.WriteLine("[S] Search  [A] Show all  [N] New  [Q] Quit");
    }

    private async Task<bool> ShowDetailsAsync(MaintenanceRecord? record)
    {
        Console.WriteLine();
        Console.WriteLine("=== Equipment Maintenance Log | Details ===");
        Console.WriteLine(record is null ? "New record" : $"Record #{record.Id}");
        Console.WriteLine($"Equipment:   {record?.EquipmentName ?? ""}");
        Console.WriteLine($"Issue:       {record?.IssueDescription ?? ""}");
        Console.WriteLine($"Reported on: {record?.ReportedDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? ""}");
        Console.WriteLine($"Status:      {(record is null ? "" : StatusLabel(record.Status))}");
        Console.WriteLine(record is null ? "[N] New  [B] Back" : "[E] Edit  [D] Delete  [B] Back");

        while (true)
        {
            switch (Read("Choose an option: ").ToUpperInvariant())
            {
                case "N" when record is null:
                    await api.CreateAsync(ReadFields(null));
                    Console.WriteLine("Record created.");
                    return true;
                case "E" when record is not null:
                    await api.UpdateAsync(record.Id, ReadFields(record));
                    Console.WriteLine("Record updated.");
                    return true;
                case "D" when record is not null:
                    if (Read("Delete this record? [y/N]: ").Equals("y", StringComparison.OrdinalIgnoreCase))
                    {
                        await api.DeleteAsync(record.Id);
                        Console.WriteLine("Record deleted.");
                        return true;
                    }
                    Console.WriteLine("Deletion cancelled.");
                    break;
                case "B":
                    return false;
                default:
                    Console.WriteLine("Choose an option shown in Details.");
                    break;
            }
        }
    }

    private static MaintenanceInput ReadFields(MaintenanceRecord? existing)
    {
        if (existing is not null)
        {
            Console.WriteLine("Press Enter to keep the current value for a field.");
        }
        return new MaintenanceInput
        {
            EquipmentName = ReadText("Equipment name", MaintenanceInput.EquipmentNameLimit, existing?.EquipmentName),
            IssueDescription = ReadText("Issue description", MaintenanceInput.IssueDescriptionLimit, existing?.IssueDescription),
            ReportedDate = ReadDate(existing?.ReportedDate),
            Status = ReadStatus(existing?.Status)
        };
    }

    private static string ReadText(string label, int limit, string? existing)
    {
        while (true)
        {
            var text = Read($"{label}: ");
            if (text.Length == 0 && existing is not null)
            {
                return existing;
            }
            if (text.Length > 0 && text.Length <= limit)
            {
                return text;
            }
            Console.WriteLine($"Enter 1 to {limit} characters.");
        }
    }

    private static DateOnly ReadDate(DateOnly? existing)
    {
        while (true)
        {
            var text = Read("Reported date (YYYY-MM-DD): ");
            if (text.Length == 0 && existing.HasValue)
            {
                return existing.Value;
            }
            if (DateOnly.TryParseExact(text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                return date;
            }
            Console.WriteLine("Enter a valid date in YYYY-MM-DD format.");
        }
    }

    private static MaintenanceStatus ReadStatus(MaintenanceStatus? existing)
    {
        while (true)
        {
            var text = Read("Status ([1] Open, [2] In Progress, [3] Resolved): ");
            if (text.Length == 0 && existing.HasValue)
            {
                return existing.Value;
            }
            if (int.TryParse(text, out var value) && Enum.IsDefined((MaintenanceStatus)value))
            {
                return (MaintenanceStatus)value;
            }
            Console.WriteLine("Choose 1, 2, or 3.");
        }
    }

    private static string StatusLabel(MaintenanceStatus status) =>
        status == MaintenanceStatus.InProgress ? "In Progress" : status.ToString();

    private static bool Retry(string message)
    {
        Console.WriteLine(message);
        return !Read("Press Enter to reload Home or Q to quit: ").Equals("q", StringComparison.OrdinalIgnoreCase);
    }

    private static string Read(string prompt)
    {
        Console.Write(prompt);
        return (Console.ReadLine() ?? throw new EndOfStreamException()).Trim();
    }
}
