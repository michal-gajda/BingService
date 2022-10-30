namespace BingService.Infrastructure.Bing.Models;

internal sealed record HPImageArchive
{
    [JsonPropertyName("images")] public HPImage[] Images { get; init; } = default!;
}
