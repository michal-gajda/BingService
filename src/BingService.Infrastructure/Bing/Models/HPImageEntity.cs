namespace BingService.Infrastructure.Bing.Models;

internal sealed class HPImageEntity
{
    public Guid Id { get; set; }
    public string Hash { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
