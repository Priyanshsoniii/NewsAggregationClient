namespace NewsAggregation.Client.Models.ResponseModels;

public class NewsResponse
{
    public List<NewsArticle> Articles { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class NewsArticle
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; }
    public string? Author { get; set; }
    public string? ImageUrl { get; set; }
    public int Likes { get; set; }
    public int Dislikes { get; set; }
    public bool IsSaved { get; set; }
}
