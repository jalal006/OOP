using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using MaintenanceLog.Contracts;

namespace MaintenanceLog.Client;

internal class MaintenanceApiClient
{
    private const string RecordsPath = "api/maintenance-records";
    private readonly HttpClient httpClient;

    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter(allowIntegerValues: false) }
    };

    public MaintenanceApiClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<IReadOnlyList<MaintenanceRecord>> ListAsync(string? search)
    {
        string path = RecordsPath;
        if (!string.IsNullOrEmpty(search))
        {
            path = $"{RecordsPath}?search={Uri.EscapeDataString(search)}";
        }

        List<MaintenanceRecord>? records = await httpClient.GetFromJsonAsync<List<MaintenanceRecord>>(path, JsonOptions);
        return records ?? new List<MaintenanceRecord>();
    }

    public async Task<MaintenanceRecord?> GetAsync(int id)
    {
        using var response = await httpClient.GetAsync($"{RecordsPath}/{id}");
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<MaintenanceRecord>(JsonOptions);
    }

    public async Task CreateAsync(MaintenanceInput input)
    {
        using var response = await httpClient.PostAsJsonAsync(RecordsPath, input, JsonOptions);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateAsync(int id, MaintenanceInput input)
    {
        using var response = await httpClient.PutAsJsonAsync($"{RecordsPath}/{id}", input, JsonOptions);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(int id)
    {
        using var response = await httpClient.DeleteAsync($"{RecordsPath}/{id}");
        response.EnsureSuccessStatusCode();
    }
}
