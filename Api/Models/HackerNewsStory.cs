namespace Api.Models;

public record HackerNewsStory(string By, int Descendants, long Id, long[] Kids, int Score, long Time, string Title, string Type, string Url);
