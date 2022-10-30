namespace BingService.Infrastructure.Bing.Models;

internal sealed record HPImage
{
    [JsonPropertyName("urlbase")] public string Url { get; init; } = string.Empty;
    [JsonPropertyName("hsh")] public string Hash { get; init; } = string.Empty;
}
