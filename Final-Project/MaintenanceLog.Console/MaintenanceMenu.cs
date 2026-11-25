using System.Globalization;
using MaintenanceLog.Contracts;

namespace MaintenanceLog.Client;

internal class MaintenanceMenu
{
    private readonly MaintenanceApiClient api;

    public MaintenanceMenu(MaintenanceApiClient api)
    {
        this.api = api;
    }

    public async Task RunAsync()
    {
        string? search = null;

        while (true)
        {
            try
            {
                var records = await api.ListAsync(search);
                ShowHome(records, search);

                string choice = Read("Choose an option or enter a record ID: ").ToUpperInvariant();
                bool recordsChanged = false;

                switch (choice)
                {
                    case "Q":
                        return;
                    case "A":
                        search = null;
                        break;
                    case "S":
                        string name = Read("Equipment name (Enter to show all): ");
                        if (name.Length <= MaintenanceInput.EquipmentNameLimit)
                        {
                            search = name;
                        }
                        else
                        {
                            Console.WriteLine($"Search text must be {MaintenanceInput.EquipmentNameLimit} characters or fewer.");
                        }
                        break;
                    case "N":
                        recordsChanged = await ShowNewDetailsAsync();
                        break;
                    case "E":
                        string editId = Read("Enter the ID of the record to edit: ");
                        recordsChanged = await OpenRecordAsync(editId, records, editImmediately: true);
                        break;
                    default:
                        recordsChanged = await OpenRecordAsync(choice, records, editImmediately: false);
                        break;
                }

                if (recordsChanged)
                {
                    search = null;
                }
            }
            catch (HttpRequestException exception)
            {
                string message = "Could not reach the API. Check that MaintenanceLog.Api is running.";
                if (exception.StatusCode.HasValue)
                {
                    message = $"The API returned HTTP {(int)exception.StatusCode.Value} ({exception.StatusCode.Value}).";
                }

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
        if (string.IsNullOrEmpty(search))
        {
            Console.WriteLine("All records");
        }
        else
        {
            Console.WriteLine($"Search results for \"{search}\"");
        }

        if (records.Count == 0)
        {
            Console.WriteLine("No records found.");
        }

        foreach (MaintenanceRecord record in records)
        {
            Console.WriteLine($"{record.Id,4}  {record.ReportedDate:yyyy-MM-dd}  "
                + $"[{StatusLabel(record.Status)}]  {record.EquipmentName}");
        }

        Console.WriteLine("[S] Search  [A] Show all  [N] New  [E] Edit  [Q] Quit");
    }

    private async Task<bool> OpenRecordAsync(string choice, IReadOnlyList<MaintenanceRecord> records, bool editImmediately)
    {
        if (int.TryParse(choice, out int id))
        {
            foreach (MaintenanceRecord item in records)
            {
                if (item.Id == id)
                {
                    MaintenanceRecord? record = await api.GetAsync(id);
                    if (record == null)
                    {
                        Console.WriteLine("That record is no longer available.");
                        return false;
                    }

                    return await ShowDetailsAsync(record, editImmediately);
                }
            }
        }

        Console.WriteLine("Choose a menu option or an ID from the list.");
        return false;
    }

    private async Task<bool> ShowNewDetailsAsync()
    {
        Console.WriteLine();
        Console.WriteLine("=== Equipment Maintenance Log | Details ===");
        Console.WriteLine("New record");
        Console.WriteLine("Equipment:   ");
        Console.WriteLine("Issue:       ");
        Console.WriteLine("Reported on: ");
        Console.WriteLine("Status:      ");
        Console.WriteLine("[N] New  [B] Back");

        while (true)
        {
            string choice = Read("Choose an option: ").ToUpperInvariant();
            if (choice == "N")
            {
                MaintenanceInput input = ReadFields(null);
                await api.CreateAsync(input);
                Console.WriteLine("Record created.");
                return true;
            }
            if (choice == "B")
            {
                return false;
            }

            Console.WriteLine("Choose N or B.");
        }
    }

    private async Task<bool> ShowDetailsAsync(MaintenanceRecord record, bool editImmediately)
    {
        Console.WriteLine();
        Console.WriteLine("=== Equipment Maintenance Log | Details ===");
        Console.WriteLine($"Record #{record.Id}");
        Console.WriteLine($"Equipment:   {record.EquipmentName}");
        Console.WriteLine($"Issue:       {record.IssueDescription}");
        Console.WriteLine($"Reported on: {record.ReportedDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}");
        Console.WriteLine($"Status:      {StatusLabel(record.Status)}");

        if (editImmediately)
        {
            await EditRecordAsync(record);
            return true;
        }

        Console.WriteLine("[E] Edit  [D] Delete  [B] Back");
        while (true)
        {
            switch (Read("Choose an option: ").ToUpperInvariant())
            {
                case "E":
                    await EditRecordAsync(record);
                    return true;
                case "D":
                    string confirm = Read("Delete this record? [y/N]: ");
                    if (confirm.Equals("y", StringComparison.OrdinalIgnoreCase))
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
                    Console.WriteLine("Choose E, D, or B.");
                    break;
            }
        }
    }

    private async Task EditRecordAsync(MaintenanceRecord record)
    {
        MaintenanceInput input = ReadFields(record);
        await api.UpdateAsync(record.Id, input);
        Console.WriteLine("Record updated.");
    }

    private static MaintenanceInput ReadFields(MaintenanceRecord? existing)
    {
        if (existing != null)
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
            string text = Read($"{label}: ");
            if (text.Length == 0 && existing != null)
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
            string text = Read("Reported date (YYYY-MM-DD): ");
            if (text.Length == 0 && existing.HasValue)
            {
                return existing.Value;
            }
            if (DateOnly.TryParseExact(text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly date))
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
            string text = Read("Status ([1] Open, [2] In Progress, [3] Resolved): ");
            if (text.Length == 0 && existing.HasValue)
            {
                return existing.Value;
            }
            if (int.TryParse(text, out int value) && Enum.IsDefined((MaintenanceStatus)value))
            {
                return (MaintenanceStatus)value;
            }
            Console.WriteLine("Choose 1, 2, or 3.");
        }
    }

    private static string StatusLabel(MaintenanceStatus status)
    {
        if (status == MaintenanceStatus.InProgress)
        {
            return "In Progress";
        }
        return status.ToString();
    }

    private static bool Retry(string message)
    {
        Console.WriteLine(message);
        return !Read("Press Enter to reload Home or Q to quit: ").Equals("q", StringComparison.OrdinalIgnoreCase);
    }

    private static string Read(string prompt)
    {
        Console.Write(prompt);
        string? input = Console.ReadLine();
        if (input == null)
        {
            throw new EndOfStreamException();
        }
        return input.Trim();
    }
}
