namespace NewsAggregation.Client.Models.ResponseModels;

public class UserResponse
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public DateTime LastLogin { get; set; }
}

public class ExternalServerResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime LastAccessed { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class NotificationSettings
{
    public bool BusinessEnabled { get; set; }
    public bool EntertainmentEnabled { get; set; }
    public bool SportsEnabled { get; set; }
    public bool TechnologyEnabled { get; set; }
    public bool KeywordsEnabled { get; set; }
    public List<string> Keywords { get; set; } = new();
}

public class NotificationResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
}