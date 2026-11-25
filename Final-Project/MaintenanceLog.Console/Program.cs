using MaintenanceLog.Client;

const string defaultApiUrl = "http://localhost:5086/";
string apiUrl = defaultApiUrl;
string? configuredApiUrl = Environment.GetEnvironmentVariable("MAINTENANCE_API_URL");
if (!string.IsNullOrWhiteSpace(configuredApiUrl))
{
    apiUrl = configuredApiUrl.Trim();
}

if (!Uri.TryCreate(apiUrl.TrimEnd('/') + "/", UriKind.Absolute, out var baseAddress)
    || (baseAddress.Scheme != "http" && baseAddress.Scheme != "https"))
{
    System.Console.Error.WriteLine("MAINTENANCE_API_URL must be a valid HTTP or HTTPS address.");
    return 1;
}

using var httpClient = new HttpClient
{
    BaseAddress = baseAddress,
    Timeout = TimeSpan.FromSeconds(20)
};

var menu = new MaintenanceMenu(new MaintenanceApiClient(httpClient));
try
{
    await menu.RunAsync();
}
catch (EndOfStreamException)
{
    // Input was closed, for example by Ctrl+Z or the end of redirected input.
}

return 0;
