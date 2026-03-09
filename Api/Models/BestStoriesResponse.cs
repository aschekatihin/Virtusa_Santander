namespace Api.Models;

public record BestStoriesResponse(string Title, string Uri, string PostedBy, string Time, int Score, int CommentCount);