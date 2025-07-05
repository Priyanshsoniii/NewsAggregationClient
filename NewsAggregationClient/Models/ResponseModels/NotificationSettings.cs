namespace NewsAggregationClient.Models.ResponseModels;

public class NotificationSettings
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public bool EmailNotifications { get; set; } = true;
    public bool BusinessNotifications { get; set; } = true;
    public bool EntertainmentNotifications { get; set; } = true;
    public bool SportsNotifications { get; set; } = true;
    public bool TechnologyNotifications { get; set; } = true;
    public bool GeneralNotifications { get; set; } = true;
    public bool PoliticsNotifications { get; set; } = true;
    public bool GamesNotifications { get; set; } = true;
    public bool SongsNotifications { get; set; } = true;
    public bool FestivalNotifications { get; set; } = true;
    public bool MiscellaneousNotifications { get; set; } = true;
    public string Keywords { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

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