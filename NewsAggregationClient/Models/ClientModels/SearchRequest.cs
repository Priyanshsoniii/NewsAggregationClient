namespace NewsAggregation.Client.Models.ClientModels;

public class SearchRequest
{
    public string Query { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Category { get; set; }
    public string? SortBy { get; set; } = "date";
}