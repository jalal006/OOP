using System.Text;
using System.Text.Json;
using AiDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AiDemo.Pages;

public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    [BindProperty]
    public string Prompt { get; set; } = string.Empty;

    [BindProperty]
    public string TextPrompt { get; set; } = string.Empty;

    public string GeneratedImageUrl { get; set; } = "https://placehold.co/800x500?text=Image+Preview";
    public string TextAnswer { get; set; } = string.Empty;

    public IndexModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task OnPostGenerateImage()
    {
        var endpoint = _configuration["Foundry:Endpoint"];
        var apiKey = _configuration["Foundry:ApiKey"];
        var model = _configuration["Foundry:ImageModel"];

        if (string.IsNullOrWhiteSpace(Prompt))
        {
            GeneratedImageUrl = "https://placehold.co/800x500?text=Enter+a+prompt";
            return;
        }

        if (string.IsNullOrWhiteSpace(endpoint) ||
            string.IsNullOrWhiteSpace(apiKey) ||
            string.IsNullOrWhiteSpace(model))
        {
            GeneratedImageUrl = "https://placehold.co/800x500?text=Missing+configuration";
            return;
        }

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("api-key", apiKey);

        var request = new ImageGenerationRequest
        {
            Prompt = Prompt,
            Width = 1024,
            Height = 768,
            Model = model
        };

        var requestJson = JsonSerializer.Serialize(request);
        using var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

        try
        {
            var response = await client.PostAsync(
                $"{endpoint.TrimEnd('/')}/mai/v1/images/generations",
                content);

            if (response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                using JsonDocument result = JsonDocument.Parse(body);

                string? image = result.RootElement
                    .GetProperty("data")[0]
                    .GetProperty("b64_json")
                    .GetString();

                if (!string.IsNullOrWhiteSpace(image))
                {
                    GeneratedImageUrl = $"data:image/png;base64,{image}";
                }
            }
            else
            {
                GeneratedImageUrl = "https://placehold.co/800x500?text=Image+Request+Failed";
            }
        }
        catch (Exception)
        {
            GeneratedImageUrl = "https://placehold.co/800x500?text=Image+Request+Failed";
        }
    }

    public async Task OnPostGenerateText()
    {
        var endpoint = _configuration["Foundry:Endpoint"];
        var apiKey = _configuration["Foundry:ApiKey"];
        var model = _configuration["Foundry:TextModel"];

        if (string.IsNullOrWhiteSpace(TextPrompt))
        {
            TextAnswer = "Enter a text prompt.";
            return;
        }

        if (string.IsNullOrWhiteSpace(endpoint) ||
            string.IsNullOrWhiteSpace(apiKey) ||
            string.IsNullOrWhiteSpace(model))
        {
            TextAnswer = "Missing configuration.";
            return;
        }

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("api-key", apiKey);

        var request = new TextGenerationRequest
        {
            Model = model,
            Input = TextPrompt,
            MaxOutputTokens = 250
        };

        var requestJson = JsonSerializer.Serialize(request);
        using var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

        try
        {
            var response = await client.PostAsync(
                $"{endpoint.TrimEnd('/')}/openai/v1/responses?api-version=v1",
                content);

            if (response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                using JsonDocument result = JsonDocument.Parse(body);

                if (result.RootElement.TryGetProperty("output", out var output))
                {
                    foreach (var item in output.EnumerateArray())
                    {
                        if (!item.TryGetProperty("content", out var parts))
                        {
                            continue;
                        }

                        foreach (var part in parts.EnumerateArray())
                        {
                            if (part.TryGetProperty("type", out var type) &&
                                type.GetString() == "output_text" &&
                                part.TryGetProperty("text", out var text))
                            {
                                TextAnswer = text.GetString() ?? string.Empty;
                                return;
                            }
                        }
                    }
                }

                TextAnswer = "No answer was returned.";
            }
            else
            {
                TextAnswer = "The text request failed.";
            }
        }
        catch (Exception)
        {
            TextAnswer = "The text request failed.";
        }
    }
}
