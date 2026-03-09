using Api;
using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IStoryService, StoryService>();
builder.Services.AddTransient<IHackerNewsClient, HackerNewsClient>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/best_stories", async ([FromQuery] int count, IStoryService storyService) =>
    {
        var result = await storyService.GetBestStories(count);

        var response = result
            .Select(x =>
                {
                    var date = DateTimeOffset.FromUnixTimeSeconds(x.Time);

                    return new BestStoriesResponse(x.Title, x.Url, x.By, date.ToString("yyyy-MM-ddTHH:mm:sszzz"), x
                        .Score, x.Kids.Length);
                }
            )
            .ToList();

        return response;
    })
    .WithName("GetBestStories");

app.Run();