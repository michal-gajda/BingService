namespace BingService.Infrastructure.Bing.Interfaces;

using BingService.Infrastructure.Bing.Models;
using Refit;

internal interface IBingService
{
    [Get("/HPImageArchive.aspx?format=js&idx={index}&n={count}&mkt={cultureInfo}")]
    public Task<HPImageArchive> DownloadAsync(int index, int count, string cultureInfo, CancellationToken cancellationToken = default);
}
