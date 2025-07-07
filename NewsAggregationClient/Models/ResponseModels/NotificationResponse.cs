namespace NewsAggregationClient.Models.ResponseModels;

public class NotificationResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; 
    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; }
    public DateTime? ReadAt { get; set; }
}

public class FilteredKeyword
{
    public int Id { get; set; }
    public string Keyword { get; set; }
    public DateTime CreatedAt { get; set; }
} 