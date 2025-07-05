using NewsAggregationClient.Services.Interfaces;
using NewsAggregationClient.UI.Interfaces;
using NewsAggregationClient.Models.ResponseModels;
using NewsAggregationClient.Models.ClientModels;

namespace NewsAggregationClient.UI.DisplayServices;

public class ConsoleDisplayService
{
    private readonly IConsoleService _console;

    public ConsoleDisplayService(IConsoleService console)
    {
        _console = console;
    }

    public void DisplayMainMenu()
    {
        _console.DisplayHeader("News Aggregator Application");
        _console.WriteLine("Welcome to the News Aggregator application. Please choose the options below.", ConsoleColor.Yellow);
        _console.WriteLine("");
        _console.WriteLine("1. Login", ConsoleColor.White);
        _console.WriteLine("2. Sign up", ConsoleColor.White);
        _console.WriteLine("3. Exit", ConsoleColor.White);
        _console.WriteLine("");
    }

    public void DisplayAdminMenu()
    {
        var menuItems = new List<string>
        {
            "View the list of external servers and status",
            "View the external server's details",
            "Update/Edit the external server's details",
            "Add new News Category",
            "View Reported Articles",
            "Hide Article",
            "Hide Category",
            "Manage Filtered Keywords",
            "Trigger News Aggregation",
            "Logout"
        };

        _console.WriteLine("Please choose the options below.", ConsoleColor.Yellow);
        _console.WriteLine("");

        for (int i = 0; i < menuItems.Count; i++)
        {
            _console.WriteLine($"{i + 1}. {menuItems[i]}", ConsoleColor.White);
        }
        _console.WriteLine("");
    }

    public void DisplayUserMenu()
    {
        var menuItems = new List<string>
        {
            "Headlines",
            "Personalized Headlines",
            "Saved Articles",
            "Search",
            "Notifications",
            "Logout"
        };

        _console.WriteLine("Please choose the options below", ConsoleColor.Yellow);
        _console.WriteLine("");

        for (int i = 0; i < menuItems.Count; i++)
        {
            _console.WriteLine($"{i + 1}. {menuItems[i]}", ConsoleColor.White);
        }
        _console.WriteLine("");
    }

    public void DisplayHeadlinesMenu()
    {
        var menuItems = new List<string>
        {
            "Today",
            "Date range",
            "Logout"
        };

        _console.WriteLine("Please choose the options below", ConsoleColor.Yellow);
        _console.WriteLine("");

        for (int i = 0; i < menuItems.Count; i++)
        {
            _console.WriteLine($"{i + 1}. {menuItems[i]}", ConsoleColor.White);
        }
        _console.WriteLine("");
    }

    public void DisplayCategoryMenu(List<Category> categories)
    {
        _console.WriteLine("Please choose the options below for Headlines", ConsoleColor.Yellow);
        _console.WriteLine("");

        for (int i = 0; i < categories.Count; i++)
        {
            _console.WriteLine($"{i + 1}. {categories[i].Name}", ConsoleColor.White);
        }
        _console.WriteLine("");
    }

    public void DisplayArticleActionMenu()
    {
        var actions = new List<string>
        {
            "Back",
            "Logout",
            "Save Article",
            "Report Article",
            "Like Article",
            "Dislike Article"
        };

        _console.WriteLine("");
        for (int i = 0; i < actions.Count; i++)
        {
            _console.WriteLine($"{i + 1}. {actions[i]}", ConsoleColor.White);
        }
        _console.WriteLine("");
    }

    public void DisplaySavedArticleActionMenu()
    {
        var actions = new List<string>
        {
            "Back",
            "Logout",
            "Delete Article"
        };

        _console.WriteLine("");
        for (int i = 0; i < actions.Count; i++)
        {
            _console.WriteLine($"{i + 1}. {actions[i]}", ConsoleColor.White);
        }
        _console.WriteLine("");
    }

    public void DisplayNotificationsMenu()
    {
        var menuItems = new List<string>
        {
            "View Notifications",
            "Configure Notifications",
            "Back",
            "Logout"
        };

        _console.WriteLine("Please choose the options below", ConsoleColor.Yellow);
        _console.WriteLine("");

        for (int i = 0; i < menuItems.Count; i++)
        {
            _console.WriteLine($"{i + 1}. {menuItems[i]}", ConsoleColor.White);
        }
        _console.WriteLine("");
    }

    public void DisplaySearchMenu()
    {
        _console.WriteLine("Please enter your search criteria:", ConsoleColor.Yellow);
        _console.WriteLine("");
    }

    public void DisplaySearchSortOptions()
    {
        var sortOptions = new List<string>
        {
            "Published Date (Newest First)",
            "Published Date (Oldest First)",
            "Most Liked",
            "Most Disliked"
        };

        _console.WriteLine("Sort by:", ConsoleColor.Yellow);
        _console.WriteLine("");

        for (int i = 0; i < sortOptions.Count; i++)
        {
            _console.WriteLine($"{i + 1}. {sortOptions[i]}", ConsoleColor.White);
        }
        _console.WriteLine("");
    }

    public void DisplayNotificationSettingsMenu(NotificationSettings settings)
    {
        var menuItems = new List<string>
        {
            "Toggle Email Notifications",
            "Toggle Business Notifications",
            "Toggle Entertainment Notifications",
            "Toggle Sports Notifications",
            "Toggle Technology Notifications",
            "Toggle General Notifications",
            "Toggle Politics Notifications",
            "Toggle Games Notifications",
            "Toggle Songs Notifications",
            "Toggle Festival Notifications",
            "Toggle Miscellaneous Notifications",
            "Configure Keywords",
            "Back to Main Menu",
            "Logout"
        };

        _console.WriteLine("Notification Settings:", ConsoleColor.Yellow);
        _console.WriteLine("");

        for (int i = 0; i < menuItems.Count; i++)
        {
            _console.WriteLine($"{i + 1}. {menuItems[i]}", ConsoleColor.White);
        }
        _console.WriteLine("");
    }

    public void DisplayNotifications(List<NewsAggregationClient.Models.ResponseModels.NotificationResponse> notifications)
    {
        _console.DisplayHeader("N O T I F I C A T I O N S");
        
        if (!notifications.Any())
        {
            _console.WriteLine("No notifications found.", ConsoleColor.Yellow);
            return;
        }

        foreach (var notification in notifications)
        {
            var status = notification.IsRead ? "[READ]" : "[NEW]";
            var color = notification.IsRead ? ConsoleColor.Gray : ConsoleColor.White;
            
            _console.WriteLine($"{status} {notification.Title}", color);
            _console.WriteLine($"   {notification.Message}", ConsoleColor.Cyan);
            _console.WriteLine($"   Type: {notification.Type} | Date: {notification.CreatedAt:yyyy-MM-dd HH:mm}", ConsoleColor.DarkGray);
            _console.DisplaySeparator();
        }
    }
}