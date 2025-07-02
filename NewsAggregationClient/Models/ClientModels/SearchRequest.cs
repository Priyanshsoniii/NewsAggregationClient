namespace NewsAggregationClient.Models.ClientModels;

public class SearchRequest
{
    public string Query { get; set; } = string.Empty;
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string SortBy { get; set; } = "publishedAt"; // publishedAt, likes, dislikes
    public string SortOrder { get; set; } = "desc"; // asc, desc
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}