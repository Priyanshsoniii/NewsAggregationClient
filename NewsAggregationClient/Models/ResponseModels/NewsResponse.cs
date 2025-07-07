namespace NewsAggregationClient.Models.ResponseModels;

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
    public string Title { get; set; }
    public string Description { get; set; }
    public string Url { get; set; }
    public string Source { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }
    public DateTime PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public int Likes { get; set; }
    public int Dislikes { get; set; }
    public Category Category { get; set; } 
    public List<object> SavedByUsers { get; set; }
    public bool IsSaved { get; internal set; }
}

public class HeadlineDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Url { get; set; }
    public string Source { get; set; }
    public DateTime PublishedAt { get; set; }
    public int Likes { get; set; }
    public int Dislikes { get; set; }
    public string CategoryName { get; set; }
    public int CategoryId { get; set; }
}

public class SavedArticle
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Url { get; set; }
    public string Source { get; set; }
    public DateTime PublishedAt { get; set; }
    public int Likes { get; set; }
    public int Dislikes { get; set; }
    public string CategoryName { get; set; }
    public int CategoryId { get; set; }
}

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsHidden { get; set; } 
    public List<object> NewsArticles { get; set; }
    public List<object> UserNotificationSettings { get; set; }
}

public class HeadlinesApiResponse
{
    public bool Success { get; set; }
    public int Count { get; set; }
    public List<HeadlineDto> Headlines { get; set; } = new();
}

