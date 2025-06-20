using NewsAggregation.Client.Services.Interfaces;
using NewsAggregation.Client.UI.Interfaces;

namespace NewsAggregation.Client.UI.DisplayServices;

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

    public void DisplayCategoryMenu()
    {
        var categories = new List<string>
        {
            "All",
            "Business",
            "Entertainment",
            "Sports",
            "Technology"
        };

        _console.WriteLine("Please choose the options below for Headlines", ConsoleColor.Yellow);
        _console.WriteLine("");

        for (int i = 0; i < categories.Count; i++)
        {
            _console.WriteLine($"{i + 1}. {categories[i]}", ConsoleColor.White);
        }
        _console.WriteLine("");
    }

    public void DisplayArticleActionMenu()
    {
        var actions = new List<string>
        {
            "Back",
            "Logout",
            "Save Article"
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
}