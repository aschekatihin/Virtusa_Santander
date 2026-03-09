using Api.Models;
using Flurl.Http;

namespace Api.Services;

public interface IHackerNewsClient
{
    Task<long[]?> GetBestStoryIds();
    Task<HackerNewsStory?> GetStory(long id);
}

public class HackerNewsClient : IHackerNewsClient
{
    private const string baseUrl = "https://hacker-news.firebaseio.com/v0";

    public Task<long[]?> GetBestStoryIds()
    {
        var url = $"{baseUrl}/beststories.json";
        return url.GetJsonAsync<long[]?>();
    }

    public Task<HackerNewsStory?> GetStory(long id)
    {
        var url = $"{baseUrl}/item/{id}.json";
        return url.GetJsonAsync<HackerNewsStory?>();
    }
}