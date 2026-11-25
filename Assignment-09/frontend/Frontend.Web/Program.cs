using Frontend.Web.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

var backendUrl = builder.Configuration["BackendApi:BaseUrl"];

if (!Uri.TryCreate(backendUrl, UriKind.Absolute, out var backendUri))
{
    throw new OptionsValidationException(
        "BackendApi", typeof(ProductApiClient),
        new[] { "Set BackendApi:BaseUrl to an absolute HTTP(S) URL ending in '/'." });
}

builder.Services.AddHttpClient<ProductApiClient>(client =>
{
    client.BaseAddress = backendUri;
    client.Timeout = TimeSpan.FromSeconds(15);
});

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();

app.Run();
