using NewsAggregationClient.Models.ResponseModels;
using NewsAggregationClient.Services.Interfaces;
using NewsAggregationClient.UI.Interfaces;
using NewsAggregationClient.Models.DTOs.ResponseDTOs;

namespace NewsAggregationClient.UI.DisplayServices;

public class NewsDisplayService : IDisplayService
{
    private readonly IConsoleService _console;

    public NewsDisplayService(IConsoleService console)
    {
        _console = console;
    }

    public void DisplayNewsArticles(List<NewsArticle> articles, string title)
    {
        _console.DisplayHeader($"{title} - {articles.Count} Articles");

        if (!articles.Any())
        {
            _console.WriteLine("No articles found.", ConsoleColor.Yellow);
            return;
        }

        foreach (var article in articles)
        {
            DisplayNewsArticle(article);
            _console.DisplaySeparator();
        }
    }

    public void DisplayNewsArticle(NewsArticle article)
    {
        _console.WriteLine($"Article Id: {article.Id}", ConsoleColor.Cyan);
        _console.WriteLine($"Title: {article.Title}", ConsoleColor.White);
        _console.WriteLine($"Description: {TruncateText(article.Description, 200)}", ConsoleColor.Gray);
        _console.WriteLine($"Source: {article.Source}", ConsoleColor.Green);
        _console.WriteLine($"URL: {article.Url}", ConsoleColor.Blue);
        _console.WriteLine($"Category: {article.Category?.Name ?? "N/A"}", ConsoleColor.Magenta);
        _console.WriteLine($"Published: {article.PublishedAt:yyyy-MM-dd HH:mm}", ConsoleColor.Yellow);

        if (article.Likes > 0 || article.Dislikes > 0)
        {
            _console.WriteLine($"Likes: {article.Likes} | Dislikes: {article.Dislikes}", ConsoleColor.Gray);
        }

        if(article.IsSaved)
        {
            _console.WriteLine("★ SAVED", ConsoleColor.Green);
        }

        _console.WriteLine("");
    }

    public void DisplaySavedArticles(List<SavedArticle> articles)
    {
        _console.DisplayHeader("S A V E D   A R T I C L E S");

        if (!articles.Any())
        {
            _console.WriteLine("No saved articles found.", ConsoleColor.Yellow);
            return;
        }

        foreach (var article in articles)
        {
            DisplaySavedArticle(article);
            _console.DisplaySeparator();
        }
    }

    public void DisplaySavedArticle(SavedArticle article)
    {
        _console.WriteLine($"ID: {article.Id}", ConsoleColor.Cyan);
        _console.WriteLine($"Title: {TruncateText(article.Title, 80)}", ConsoleColor.White);
        _console.WriteLine($"Description: {TruncateText(article.Description, 100)}", ConsoleColor.Gray);
        _console.WriteLine($"Source: {article.Source}", ConsoleColor.Green);
        _console.WriteLine($"URL: {article.Url}", ConsoleColor.Blue);
        _console.WriteLine($"Category: {article.CategoryName}", ConsoleColor.Magenta);
        _console.WriteLine($"Published: {article.PublishedAt:yyyy-MM-dd HH:mm}", ConsoleColor.Yellow);

        if (article.Likes > 0 || article.Dislikes > 0)
        {
            _console.WriteLine($"Likes: {article.Likes} | Dislikes: {article.Dislikes}", ConsoleColor.Gray);
        }

        _console.WriteLine("★ SAVED", ConsoleColor.Green);
        _console.WriteLine("");
    }

    public void DisplaySearchResults(List<NewsArticle> articles, string query)
    {
        _console.DisplayHeader($"S E A R C H   R E S U L T S");
        _console.WriteLine($"Results for \"{query}\"", ConsoleColor.Cyan);
        _console.WriteLine("");

        if (!articles.Any())
        {
            _console.WriteLine("No articles found matching your search.", ConsoleColor.Yellow);
            return;
        }

        foreach (var article in articles)
        {
            DisplayNewsArticle(article);
            _console.DisplaySeparator();
        }
    }

    public void DisplayExternalServers(List<ExternalServerResponseDto> servers)
    {
        _console.DisplayHeader("External Servers");

        if (!servers.Any())
        {
            _console.WriteLine("No external servers configured.", ConsoleColor.Yellow);
            return;
        }

        _console.WriteLine("List of external servers:", ConsoleColor.Cyan);
        _console.WriteLine("");

        for (int i = 0; i < servers.Count; i++)
        {
            var server = servers[i];
            var status = server.IsActive ? "Active" : "Not Active";
            var statusColor = server.IsActive ? ConsoleColor.Green : ConsoleColor.Red;

            _console.WriteLine($"{i + 1}. {server.Name} - ", ConsoleColor.White);
            _console.Write($"{status}", statusColor);
            _console.WriteLine($" - last accessed: {server.LastAccessed:dd MMM yyyy}", ConsoleColor.Gray);
        }
    }

    public void DisplayExternalServerDetails(ExternalServerResponseDto server)
    {
        _console.DisplayHeader("External Server Details");

        _console.WriteLine($"Name: {server.Name}", ConsoleColor.Cyan);
        _console.WriteLine($"API Key: {MaskApiKey(server.ApiKey)}", ConsoleColor.Yellow);
        _console.WriteLine($"Base URL: {server.BaseUrl}", ConsoleColor.Blue);
        _console.WriteLine($"Status: {(server.IsActive ? "Active" : "Inactive")}",
            server.IsActive ? ConsoleColor.Green : ConsoleColor.Red);
        _console.WriteLine($"Last Accessed: {server.LastAccessed:dd MMM yyyy HH:mm}", ConsoleColor.Gray);
    }

    public void DisplayNotifications(List<NotificationResponse> notifications)
    {
        _console.DisplayHeader("N O T I F I C A T I O N S");

        if (!notifications.Any())
        {
            _console.WriteLine("No notifications found.", ConsoleColor.Yellow);
            return;
        }

        foreach (var notification in notifications)
        {
            var readStatus = notification.IsRead ? "READ" : "UNREAD";
            var readColor = notification.IsRead ? ConsoleColor.Gray : ConsoleColor.Green;

            _console.WriteLine($"[{readStatus}] {notification.Title}", readColor);
            _console.WriteLine($"   {notification.Message}", ConsoleColor.White);
            _console.WriteLine($"   {notification.CreatedAt:dd MMM yyyy HH:mm} - {notification.Type}", ConsoleColor.Gray);
            _console.WriteLine("");
        }
    }

    public void DisplayNotificationSettings(NotificationSettings settings)
    {
        _console.DisplayHeader("C O N F I G U R E - N O T I F I C A T I O N S");

        _console.WriteLine($"1. Business - {GetEnabledStatus(settings.BusinessEnabled)}", GetEnabledColor(settings.BusinessEnabled));
        _console.WriteLine($"2. Entertainment - {GetEnabledStatus(settings.EntertainmentEnabled)}", GetEnabledColor(settings.EntertainmentEnabled));
        _console.WriteLine($"3. Sports - {GetEnabledStatus(settings.SportsEnabled)}", GetEnabledColor(settings.SportsEnabled));
        _console.WriteLine($"4. Technology - {GetEnabledStatus(settings.TechnologyEnabled)}", GetEnabledColor(settings.TechnologyEnabled));
        _console.WriteLine($"5. Keywords - {GetEnabledStatus(settings.KeywordsEnabled)}", GetEnabledColor(settings.KeywordsEnabled));

        if (settings.KeywordsEnabled && settings.Keywords.Any())
        {
            _console.WriteLine($"   Keywords: {string.Join(", ", settings.Keywords)}", ConsoleColor.Cyan);
        }

        _console.WriteLine("6. Back", ConsoleColor.White);
        _console.WriteLine("7. Logout", ConsoleColor.White);
    }

    public void DisplayUserWelcome(string username, DateTime currentTime)
    {
        _console.DisplayHeader($"Welcome to the News Application, {username}!");
        _console.WriteLine($"Date: {currentTime:dd-MMM-yyyy}", ConsoleColor.Cyan);
        _console.WriteLine($"Time: {currentTime:HH:mm}", ConsoleColor.Cyan);
        _console.WriteLine("");
    }

    public void DisplayMenu(List<string> menuItems, string title)
    {
        _console.WriteLine($"Please choose the options below for {title}", ConsoleColor.Yellow);
        _console.WriteLine("");

        for (int i = 0; i < menuItems.Count; i++)
        {
            _console.WriteLine($"{i + 1}. {menuItems[i]}", ConsoleColor.White);
        }
        _console.WriteLine("");
    }

    public void DisplayPaginatedArticles(List<NewsArticle> articles, int currentPage, int totalPages, string title)
    {
        _console.DisplayHeader($"{title} - Page {currentPage} of {totalPages}");

        if (!articles.Any())
        {
            _console.WriteLine("No articles found.", ConsoleColor.Yellow);
            return;
        }

        foreach (var article in articles)
        {
            DisplayNewsArticle(article);
            _console.DisplaySeparator();
        }

        if (totalPages > 1)
        {
            _console.WriteLine($"Page {currentPage} of {totalPages}", ConsoleColor.Cyan);
        }
    }

    public void DisplayCategoryMenu(List<Category> categories)
    {
        Console.WriteLine("Categories:");
        for (int i = 0; i < categories.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {categories[i].Name}");
        }
    }

    public void DisplayNotificationSettingsMenu(NotificationSettings settings)
    {
        _console.DisplayHeader("Notification Settings Menu");
        _console.WriteLine($"Email Notifications: {(settings.EmailEnabled ? "Enabled" : "Disabled")}", settings.EmailEnabled ? ConsoleColor.Green : ConsoleColor.Red);
        _console.WriteLine($"Business: {(settings.BusinessEnabled ? "Enabled" : "Disabled")}", settings.BusinessEnabled ? ConsoleColor.Green : ConsoleColor.Red);
        _console.WriteLine($"Entertainment: {(settings.EntertainmentEnabled ? "Enabled" : "Disabled")}", settings.EntertainmentEnabled ? ConsoleColor.Green : ConsoleColor.Red);
        _console.WriteLine($"Sports: {(settings.SportsEnabled ? "Enabled" : "Disabled")}", settings.SportsEnabled ? ConsoleColor.Green : ConsoleColor.Red);
        _console.WriteLine($"Technology: {(settings.TechnologyEnabled ? "Enabled" : "Disabled")}", settings.TechnologyEnabled ? ConsoleColor.Green : ConsoleColor.Red);
        _console.WriteLine($"General: {(settings.GeneralEnabled ? "Enabled" : "Disabled")}", settings.GeneralEnabled ? ConsoleColor.Green : ConsoleColor.Red);
        _console.WriteLine($"Politics: {(settings.PoliticsEnabled ? "Enabled" : "Disabled")}", settings.PoliticsEnabled ? ConsoleColor.Green : ConsoleColor.Red);
        _console.WriteLine($"Games: {(settings.GamesEnabled ? "Enabled" : "Disabled")}", settings.GamesEnabled ? ConsoleColor.Green : ConsoleColor.Red);
        _console.WriteLine($"Songs: {(settings.SongsEnabled ? "Enabled" : "Disabled")}", settings.SongsEnabled ? ConsoleColor.Green : ConsoleColor.Red);
        _console.WriteLine($"Festival: {(settings.FestivalEnabled ? "Enabled" : "Disabled")}", settings.FestivalEnabled ? ConsoleColor.Green : ConsoleColor.Red);
        _console.WriteLine($"Miscellaneous: {(settings.MiscellaneousEnabled ? "Enabled" : "Disabled")}", settings.MiscellaneousEnabled ? ConsoleColor.Green : ConsoleColor.Red);
        _console.WriteLine("");
        if (settings.Keywords != null && settings.Keywords.Any())
        {
            _console.WriteLine($"Keywords: {string.Join(", ", settings.Keywords)}", ConsoleColor.Cyan);
        }
    }

    private string TruncateText(string text, int maxLength)
    {
        if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
            return text;

        return text[..maxLength] + "...";
    }

    private string MaskApiKey(string apiKey)
    {
        if (string.IsNullOrEmpty(apiKey) || apiKey.Length <= 8)
            return apiKey;

        return apiKey[..4] + new string('*', apiKey.Length - 8) + apiKey[^4..];
    }

    private string GetEnabledStatus(bool enabled)
    {
        return enabled ? "Enabled" : "Disabled";
    }

    private ConsoleColor GetEnabledColor(bool enabled)
    {
        return enabled ? ConsoleColor.Green : ConsoleColor.Red;
    }
}