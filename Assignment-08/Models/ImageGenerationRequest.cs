using System.Text.Json.Serialization;

namespace AiDemo.Models;

public class ImageGenerationRequest
{
    [JsonPropertyName("prompt")]
    public string Prompt { get; set; } = string.Empty;

    [JsonPropertyName("width")]
    public int Width { get; set; } = 1024;

    [JsonPropertyName("height")]
    public int Height { get; set; } = 768;

    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;
}
