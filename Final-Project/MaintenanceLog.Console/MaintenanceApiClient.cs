using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using MaintenanceLog.Contracts;

namespace MaintenanceLog.Client;

internal sealed class MaintenanceApiClient(HttpClient httpClient)
{
    private const string RecordsPath = "api/maintenance-records";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter(allowIntegerValues: false) }
    };

    public async Task<IReadOnlyList<MaintenanceRecord>> ListAsync(string? search)
    {
        var path = string.IsNullOrEmpty(search)
            ? RecordsPath
            : $"{RecordsPath}?search={Uri.EscapeDataString(search)}";
        return await httpClient.GetFromJsonAsync<List<MaintenanceRecord>>(path, JsonOptions) ?? [];
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
