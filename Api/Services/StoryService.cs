using Api.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Api.Services;

public interface IStoryService
{
    Task<IList<HackerNewsStory>> GetBestStories(int count);
}

public class StoryService : IStoryService
{
    private static readonly SemaphoreSlim Semaphore = new(1, 1);
    private const int FetchStoryParallelism = 5;
    private const string BestStoriesKey = "best_stories";

    private readonly IMemoryCache memoryCache;
    private readonly IHackerNewsClient hackerNewsClient;
    private readonly ILogger<StoryService> logger;

    public StoryService(IMemoryCache memoryCache, IHackerNewsClient hackerNewsClient, ILogger<StoryService> logger)
    {
        this.memoryCache = memoryCache;
        this.hackerNewsClient = hackerNewsClient;
        this.logger = logger;
    }

    public async Task<IList<HackerNewsStory>> GetBestStories(int count)
    {
        await RefreshCaches();

        if (!memoryCache.TryGetValue<long[]?>(BestStoriesKey, out var bestStoryIds))
        {
            return new List<HackerNewsStory>();
        }

        if (bestStoryIds == null)
            return new List<HackerNewsStory>();

        var cachedItems = bestStoryIds
            .Select(x => memoryCache.Get<HackerNewsStory?>(x))
            .Where(x => x != null)
            .Select(x => x!)
            .ToArray();

        if (cachedItems.Length == 0)
            return new List<HackerNewsStory>();

        return cachedItems
            .OrderByDescending(x => x.Score)
            .Take(count)
            .ToList();
    }

    private async Task RefreshCaches()
    {
        long[]? bestStoryIds;

        await Semaphore.WaitAsync();
        try
        {
            if (!memoryCache.TryGetValue(BestStoriesKey, out bestStoryIds))
            {
                bestStoryIds = await hackerNewsClient.GetBestStoryIds();

                memoryCache.Set(BestStoriesKey, bestStoryIds, new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(10) });
            }
        }
        finally
        {
            Semaphore.Release();
        }

        if (bestStoryIds == null || bestStoryIds.Length == 0)
        {
            return;
        }

        await Parallel.ForEachAsync(bestStoryIds, new ParallelOptions { MaxDegreeOfParallelism = FetchStoryParallelism }, async (id, _) =>
        {
            if (memoryCache.TryGetValue(id, out HackerNewsStory? cachedStory) && cachedStory != null)
            {
                logger.LogInformation("Story {id} is in cache", id.ToString());
                return;
            }
            
            logger.LogInformation("Fetching {id}", id.ToString());
            
            var story = await hackerNewsClient.GetStory(id);
            memoryCache.Set(id, story, new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(10) });
        });
    }
}