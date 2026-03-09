using Api;
using Api.Services;

namespace TestProject1;

[TestClass]
public sealed class HackerNewsClientTests
{
    [TestMethod]
    public async Task GetBestStoryIds_does_not_smoke_test()
    {
        var service = new HackerNewsClient();
        
        var result = await service.GetBestStoryIds();
        Assert.IsNotNull(result);
        
        Assert.IsGreaterThan(0, result.Length);
    }

    [TestMethod]
    public async Task GetStory_does_not_smoke()
    {
        var service = new HackerNewsClient();
        
        var bestIds = await service.GetBestStoryIds();
        
        var result = await service.GetStory(bestIds[0]);
        Assert.IsNotNull(result);
        Assert.AreEqual(bestIds[0], result.Id);
    }
}