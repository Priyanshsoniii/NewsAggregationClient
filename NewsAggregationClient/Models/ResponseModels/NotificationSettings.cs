namespace NewsAggregationClient.Models.ResponseModels;

// Model for individual category notification setting from API
public class CategoryNotificationSetting
{
    public int? CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public bool EmailNotifications { get; set; }
}

// Model for the API response wrapper
public class NotificationSettingsResponse
{
    public bool Success { get; set; }
    public List<CategoryNotificationSetting> Settings { get; set; } = new();
}

public class NotificationSettings
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public bool EmailNotifications { get; set; } = true;
    public string Keywords { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Dynamic storage for category notifications
    public Dictionary<string, bool> CategoryNotifications { get; set; } = new();

    // Legacy properties for backward compatibility
    public bool BusinessNotifications 
    { 
        get => CategoryNotifications.GetValueOrDefault("business", false);
        set => CategoryNotifications["business"] = value;
    }
    public bool EntertainmentNotifications 
    { 
        get => CategoryNotifications.GetValueOrDefault("entertainment", false);
        set => CategoryNotifications["entertainment"] = value;
    }
    public bool SportsNotifications 
    { 
        get => CategoryNotifications.GetValueOrDefault("sports", false);
        set => CategoryNotifications["sports"] = value;
    }
    public bool TechnologyNotifications 
    { 
        get => CategoryNotifications.GetValueOrDefault("technology", false);
        set => CategoryNotifications["technology"] = value;
    }
    public bool GeneralNotifications 
    { 
        get => CategoryNotifications.GetValueOrDefault("general", false);
        set => CategoryNotifications["general"] = value;
    }
    public bool PoliticsNotifications 
    { 
        get => CategoryNotifications.GetValueOrDefault("politics", false);
        set => CategoryNotifications["politics"] = value;
    }
    public bool GamesNotifications 
    { 
        get => CategoryNotifications.GetValueOrDefault("games", false);
        set => CategoryNotifications["games"] = value;
    }
    public bool SongsNotifications 
    { 
        get => CategoryNotifications.GetValueOrDefault("songs", false);
        set => CategoryNotifications["songs"] = value;
    }
    public bool FestivalNotifications 
    { 
        get => CategoryNotifications.GetValueOrDefault("festival", false);
        set => CategoryNotifications["festival"] = value;
    }
    public bool MiscellaneousNotifications 
    { 
        get => CategoryNotifications.GetValueOrDefault("miscellaneous", false);
        set => CategoryNotifications["miscellaneous"] = value;
    }

    // Dynamic method to get/set any category notification
    public bool GetCategoryNotification(string categoryName)
    {
        return CategoryNotifications.GetValueOrDefault(categoryName.ToLower(), false);
    }

    public void SetCategoryNotification(string categoryName, bool enabled)
    {
        CategoryNotifications[categoryName.ToLower()] = enabled;
    }

    // Add these aliases for compatibility
    public bool EmailEnabled => EmailNotifications;
    public bool BusinessEnabled => BusinessNotifications;
    public bool EntertainmentEnabled => EntertainmentNotifications;
    public bool SportsEnabled => SportsNotifications;
    public bool TechnologyEnabled => TechnologyNotifications;
    public bool GeneralEnabled => GeneralNotifications;
    public bool PoliticsEnabled => PoliticsNotifications;
    public bool GamesEnabled => GamesNotifications;
    public bool SongsEnabled => SongsNotifications;
    public bool FestivalEnabled => FestivalNotifications;
    public bool MiscellaneousEnabled => MiscellaneousNotifications;
    public bool KeywordsEnabled => !string.IsNullOrEmpty(Keywords);
}