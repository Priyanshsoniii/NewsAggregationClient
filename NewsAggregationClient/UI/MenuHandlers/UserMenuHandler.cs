using NewsAggregation.Client.Models.ResponseModels;
using NewsAggregation.Client.Services.Interfaces;
using NewsAggregation.Client.UI.DisplayServices;
using NewsAggregation.Client.UI.Interfaces;

namespace NewsAggregation.Client.UI.MenuHandlers;

public class UserMenuHandler : IMenuHandler
{
    private readonly IConsoleService _console;
    private readonly IApiService _apiService;
    private readonly IDisplayService _displayService;
    private readonly ConsoleDisplayService _consoleDisplay;
    private readonly NewsDisplayService _newsDisplay;

    public UserMenuHandler(
        IConsoleService console,
        IApiService apiService,
        IDisplayService displayService,
        ConsoleDisplayService consoleDisplay,
        NewsDisplayService newsDisplay)
    {
        _console = console;
        _apiService = apiService;
        _displayService = displayService;
        _consoleDisplay = consoleDisplay;
        _newsDisplay = newsDisplay;
    }

    public async Task HandleMenuAsync(UserDto user)
    {
        while (true)
        {
            if (user == null || _newsDisplay == null)
            {
                throw new InvalidOperationException("Required objects are not initialized.");
            }
            _newsDisplay.DisplayUserWelcome(user.Username, DateTime.Now);
            _consoleDisplay.DisplayUserMenu();

            _console.Write("Enter your choice: ");
            var choice = _console.ReadLine();

            switch (choice)
            {
                case "1":
                    await HandleHeadlinesAsync(user);
                    break;
                case "2":
                    await HandleSavedArticlesAsync(user);
                    break;
                case "3":
               //     await HandleSearchAsync(user);
                    break;
                case "4":
                //    await HandleNotificationsAsync(user);
                    break;
                case "5":
                    _console.WriteLine("Logging out...", ConsoleColor.Yellow);
                    return;
                default:
                    _console.DisplayError("Invalid choice. Please select 1-5.");
                    _console.PressAnyKeyToContinue();
                    break;
            }
        }
    }

    private async Task HandleHeadlinesAsync(UserDto user)
    {
        while (true)
        {
            _displayService.DisplayUserWelcome(user.Username, DateTime.Now);
            _consoleDisplay.DisplayHeadlinesMenu();

            _console.Write("Enter your choice: ");
            var choice = _console.ReadLine();

            switch (choice)
            {
                case "1":
                    
                    await ShowHeadlinesByDateAsync(user, "today");
                    break;
                case "2":
                    await ShowHeadlinesByDateRangeAsync(user);
                    break;
                case "3":
                    return; // Back to main menu
                default:
                    _console.DisplayError("Invalid choice. Please select 1-3.");
                    _console.PressAnyKeyToContinue();
                    break;
            }
        }
    }

    private async Task ShowHeadlinesByDateAsync(UserDto user, string dateOption)
    {
        DateTime? fromDate = dateOption == "today" ? DateTime.Today : null;
        DateTime? toDate = dateOption == "today" ? DateTime.Today.AddDays(1).AddSeconds(-1) : null;

        while (true)
        {
            _displayService.DisplayUserWelcome(user.Username, DateTime.Now);
            _consoleDisplay.DisplayCategoryMenu();

            _console.Write("Enter your choice: ");
            var choice = _console.ReadLine();

            string category = choice switch
            {
                "1" => "all",
                "2" => "business",
                "3" => "entertainment",
                "4" => "sports",
                "5" => "technology",
                _ => null
            };

            if (category == null)
            {
                _console.DisplayError("Invalid choice. Please select 1-5.");
                _console.PressAnyKeyToContinue();
                continue;
            }

            await DisplayNewsArticlesAsync(user, category, fromDate, toDate);
            return;
        }
    }

    private async Task ShowHeadlinesByDateRangeAsync(UserDto user)
    {
        try
        {
            _console.Write("Enter start date (yyyy-mm-dd): ");
            var startDateInput = _console.ReadLine();

            if (!DateTime.TryParse(startDateInput, out DateTime startDate))
            {
                _console.DisplayError("Invalid start date format. Please use yyyy-mm-dd.");
                _console.PressAnyKeyToContinue();
                return;
            }

            _console.Write("Enter end date (yyyy-mm-dd): ");
            var endDateInput = _console.ReadLine();

            if (!DateTime.TryParse(endDateInput, out DateTime endDate))
            {
                _console.DisplayError("Invalid end date format. Please use yyyy-mm-dd.");
                _console.PressAnyKeyToContinue();
                return;
            }

            if (startDate > endDate)
            {
                _console.DisplayError("Start date cannot be later than end date.");
                _console.PressAnyKeyToContinue();
                return;
            }

            if (startDate > DateTime.Today)
            {
                _console.DisplayError("Start date cannot be in the future.");
                _console.PressAnyKeyToContinue();
                return;
            }

            while (true)
            {
                _displayService.DisplayUserWelcome(user.Username, DateTime.Now);
                _consoleDisplay.DisplayCategoryMenu();

                _console.Write("Enter your choice: ");
                var choice = _console.ReadLine();

                string category = choice switch
                {
                    "1" => "all",
                    "2" => "business",
                    "3" => "entertainment",
                    "4" => "sports",
                    "5" => "technology",
                    _ => null
                };

                if (category == null)
                {
                    _console.DisplayError("Invalid choice. Please select 1-5.");
                    _console.PressAnyKeyToContinue();
                    continue;
                }

                await DisplayNewsArticlesAsync(user, category, startDate, endDate.AddDays(1).AddSeconds(-1));
                return;
            }
        }
        catch (Exception ex)
        {
            _console.DisplayError($"Error processing date range: {ex.Message}");
            _console.PressAnyKeyToContinue();
        }
    }

    private async Task DisplayNewsArticlesAsync(UserDto user, string category, DateTime? fromDate = null, DateTime? toDate = null)
    {
        try
        {
            _console.WriteLine("Loading headlines...", ConsoleColor.Yellow);

            var response = await _apiService.GetHeadlinesAsync(category);

            if (response.Success && response.Data != null && response.Data.Articles != null && response.Data.Articles.Any())
            {
                while (true)
                {
                    _displayService.DisplayUserWelcome(user.Username, DateTime.Now);
                    _newsDisplay.DisplayNewsArticles(response.Data.Articles, "Headlines");
                    _consoleDisplay.DisplayArticleActionMenu();

                    _console.Write("Enter your choice: ");
                    var choice = _console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            return; // Back
                        case "2":
                            _console.WriteLine("Logging out...", ConsoleColor.Yellow);
                            Environment.Exit(0);
                            break;
                        case "3":
                                await SaveArticleAsync(user);
                            break;
                        default:
                            _console.DisplayError("Invalid choice. Please select 1-3.");
                            _console.PressAnyKeyToContinue();
                            break;
                    }
                }
            }
            else
            {
                _console.DisplayError(response.Message ?? "No headlines found for the selected criteria.");
                _console.PressAnyKeyToContinue();
            }
        }
        catch (Exception ex)
        {
            _console.DisplayError($"Error loading headlines: {ex.Message}");
            _console.PressAnyKeyToContinue();
        }
    }

    private async Task SaveArticleAsync(UserDto user)
    {
        try
        {
            _console.Write("Enter Article ID to save: ");
            var input = _console.ReadLine();

            if (!int.TryParse(input, out int articleId) || articleId <= 0)
            {
                _console.DisplayError("Invalid Article ID. Please enter a valid positive number.");
                _console.PressAnyKeyToContinue();
                return;
            }

            _console.WriteLine("Saving article...", ConsoleColor.Yellow);

             var response = await _apiService.SaveArticleAsync(user, articleId);

            if (response.Success)
            {
                _console.DisplaySuccess($"Article {articleId} saved successfully!");
            }
            else
            {
                _console.DisplayError(response.Message ?? "Failed to save article.");
            }
        }
        catch (Exception ex)
        {
            _console.DisplayError($"Error saving article: {ex.Message}");
        }

        _console.PressAnyKeyToContinue();
    }

    private async Task DeleteSavedArticleAsync(UserDto user)
    {
        try
        {
            _console.Write("Enter Article ID to delete: ");
            var input = _console.ReadLine();

            if (!int.TryParse(input, out int articleId) || articleId <= 0)
            {
                _console.DisplayError("Invalid Article ID. Please enter a valid positive number.");
                _console.PressAnyKeyToContinue();
                return;
            }

            _console.WriteLine("Deleting article...", ConsoleColor.Yellow);

            var response = await _apiService.DeleteSavedArticleAsync(user.Id, articleId);

            if (response.Success)
            {
                _console.DisplaySuccess($"Article {articleId} deleted successfully!");
            }
            else
            {
                _console.DisplayError(response.Message ?? "Failed to delete article.");
            }
        }
        catch (Exception ex)
        {
            _console.DisplayError($"Error deleting article: {ex.Message}");
        }

        _console.PressAnyKeyToContinue();
    }

    private async Task HandleSavedArticlesAsync(UserDto user)
    {
        try
        {
            _console.WriteLine("Loading saved articles...", ConsoleColor.Yellow);

            var response = await _apiService.GetSavedArticlesAsync(user.Id);

            if (response.Success && response.Data != null && response.Data.Any())
            {
                while (true)
                {
                    _displayService.DisplayUserWelcome(user.Username, DateTime.Now);
                    _newsDisplay.DisplaySavedArticles(response.Data);
                    _consoleDisplay.DisplaySavedArticleActionMenu();

                    _console.Write("Enter your choice: ");
                    var choice = _console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            return; // Back
                        case "2":
                            _console.WriteLine("Logging out...", ConsoleColor.Yellow);
                            Environment.Exit(0);
                            break;
                        case "3":
                            await DeleteSavedArticleAsync(user);
                            // Refresh the saved articles list
                            response = await _apiService.GetSavedArticlesAsync(user.Id);
                            if (!response.Success || response.Data == null || !response.Data.Any())
                            {
                                _console.WriteLine("No more saved articles.", ConsoleColor.Yellow);
                                _console.PressAnyKeyToContinue();
                                return;
                            }
                            break;
                        default:
                            _console.DisplayError("Invalid choice. Please select 1-3.");
                            _console.PressAnyKeyToContinue();
                            break;
                    }
                }
            }
            else
            {
                _console.DisplayError("No saved articles found.");
                _console.PressAnyKeyToContinue();
            }
        }
        catch (Exception ex)
        {
            _console.DisplayError($"Error loading saved articles: {ex.Message}");
            _console.PressAnyKeyToContinue();
        }
    }

    //private async Task HandleSearchAsync(UserResponse user)
    //{
    //    try
    //    {
    //        _console.Write("Enter search query: ");
    //        var query = _console.ReadLine();

    //        if (string.IsNullOrWhiteSpace(query))
    //        {
    //            _console.DisplayError("Search query cannot be empty.");
    //            _console.PressAnyKeyToContinue();
    //            return;
    //        }

    //        // Optional: Ask for date range
    //        _console.Write("Filter by date range? (y/n): ");
    //        var useDateRange = _console.ReadLine()?.ToLower() == "y";

    //        DateTime? fromDate = null;
    //        DateTime? toDate = null;

    //        if (useDateRange)
    //        {
    //            _console.Write("Enter start date (yyyy-mm-dd) or press Enter to skip: ");
    //            var startDateInput = _console.ReadLine();
    //            if (!string.IsNullOrWhiteSpace(startDateInput) && DateTime.TryParse(startDateInput, out DateTime startDate))
    //            {
    //                fromDate = startDate;
    //            }

    //            _console.Write("Enter end date (yyyy-mm-dd) or press Enter to skip: ");
    //            var endDateInput = _console.ReadLine();
    //            if (!string.IsNullOrWhiteSpace(endDateInput) && DateTime.TryParse(endDateInput, out DateTime endDate))
    //            {
    //                toDate = endDate.AddDays(1).AddSeconds(-1);
    //            }
    //        }

    //        _console.WriteLine("Searching...", ConsoleColor.Yellow);

    //        var searchRequest = new SearchRequest
    //        {
    //            Query = query,
    //            FromDate = fromDate,
    //            ToDate = toDate,
    //            UserId = user.Id
    //        };

    //        var response = await _apiService.SearchNewsAsync(searchRequest);

    //        if (response.Success && response.Data != null && response.Data.Any())
    //        {
    //            while (true)
    //            {
    //                _displayService.DisplayUserWelcome(user.Username, DateTime.Now);
    //                _newsDisplay.DisplaySearchResults(response.Data, query);
    //                _consoleDisplay.DisplayArticleActionMenu();

    //                _console.Write("Enter your choice: ");
    //                var choice = _console.ReadLine();

    //                switch (choice)
    //                {
    //                    case "1":
    //                        return; // Back
    //                    case "2":
    //                        _console.WriteLine("Logging out...", ConsoleColor.Yellow);
    //                        Environment.Exit(0);
    //                        break;
    //                    case "3":
    //                        await SaveArticleAsync(user);
    //                        break;
    //                    default:
    //                        _console.DisplayError("Invalid choice. Please select 1-3.");
    //                        _console.PressAnyKeyToContinue();
    //                        break;
    //                }
    //            }
    //        }
    //        else
    //        {
    //            _console.DisplayError($"No results found for '{query}'.");
    //            _console.PressAnyKeyToContinue();
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        _console.DisplayError($"Error during search: {ex.Message}");
    //        _console.PressAnyKeyToContinue();
    //    }
    //}

    //private async Task HandleNotificationsAsync(UserResponse user)
    //{
    //    while (true)
    //    {
    //        _displayService.DisplayUserWelcome(user.Username, DateTime.Now);
    //        _consoleDisplay.DisplayNotificationsMenu();

    //        _console.Write("Enter your choice: ");
    //        var choice = _console.ReadLine();

    //        switch (choice)
    //        {
    //            case "1":
    //                await ViewNotificationsAsync(user);
    //                break;
    //            case "2":
    //                await ConfigureNotificationsAsync(user);
    //                break;
    //            case "3":
    //                return; // Back
    //            case "4":
    //                _console.WriteLine("Logging out...", ConsoleColor.Yellow);
    //                Environment.Exit(0);
    //                break;
    //            default:
    //                _console.DisplayError("Invalid choice. Please select 1-4.");
    //                _console.PressAnyKeyToContinue();
    //                break;
    //        }
    //    }
    //}

    //private async Task ViewNotificationsAsync(UserResponse user)
    //{
    //    try
    //    {
    //        _console.WriteLine("Loading notifications...", ConsoleColor.Yellow);

    //        var response = await _apiService.GetNotificationsAsync(user.Id);

    //        if (response.Success && response.Data != null && response.Data.Any())
    //        {
    //            _displayService.DisplayUserWelcome(user.Username, DateTime.Now);
    //            _console.WriteLine("N O T I F I C A T I O N S", ConsoleColor.Cyan);
    //            _console.WriteLine(new string('=', 50), ConsoleColor.Gray);

    //            for (int i = 0; i < response.Data.Count; i++)
    //            {
    //                _console.WriteLine($"{i + 1}. {response.Data[i]}", ConsoleColor.White);
    //            }
    //        }
    //        else
    //        {
    //            _console.DisplayError("No notifications found.");
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        _console.DisplayError($"Error loading notifications: {ex.Message}");
    //    }

    //    _console.PressAnyKeyToContinue();
    //}

    //private async Task ConfigureNotificationsAsync(UserResponse user)
    //{
    //    while (true)
    //    {
    //        _displayService.DisplayUserWelcome(user.Username, DateTime.Now);
    //        _consoleDisplay.DisplayNotificationConfigMenu();

    //        _console.Write("Enter your option: ");
    //        var choice = _console.ReadLine();

    //        switch (choice)
    //        {
    //            case "1":
    //            case "2":
    //            case "3":
    //            case "4":
    //                await ToggleCategoryNotificationAsync(user, choice);
    //                break;
    //            case "5":
    //                await ConfigureKeywordsAsync(user);
    //                break;
    //            case "6":
    //                return; // Back
    //            case "7":
    //                _console.WriteLine("Logging out...", ConsoleColor.Yellow);
    //                Environment.Exit(0);
    //                break;
    //            default:
    //                _console.DisplayError("Invalid choice. Please select 1-7.");
    //                _console.PressAnyKeyToContinue();
    //                break;
    //        }
    //    }
    //}

    //private async Task ToggleCategoryNotificationAsync(UserResponse user, string categoryChoice)
    //{
    //    try
    //    {
    //        string category = categoryChoice switch
    //        {
    //            "1" => "Business",
    //            "2" => "Entertainment",
    //            "3" => "Sports",
    //            "4" => "Technology",
    //            _ => ""
    //        };

    //        _console.Write($"Enable {category} notifications? (y/n): ");
    //        var enable = _console.ReadLine()?.ToLower() == "y";

    //        var settings = new Dictionary<string, bool> { { category.ToLower(), enable } };

    //        var response = await _apiService.ConfigureNotificationsAsync(user.Id, settings);

    //        if (response.Success)
    //        {
    //            var status = enable ? "enabled" : "disabled";
    //            _console.DisplaySuccess($"{category} notifications {status} successfully!");
    //        }
    //        else
    //        {
    //            _console.DisplayError(response.Message ?? "Failed to update notification settings.");
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        _console.DisplayError($"Error updating notifications: {ex.Message}");
    //    }

    //    _console.PressAnyKeyToContinue();
    //}

    //private async Task ConfigureKeywordsAsync(UserResponse user)
    //{
    //    try
    //    {
    //        _console.WriteLine("Current keywords will be replaced with new ones.");
    //        _console.Write("Enter keywords separated by commas: ");
    //        var keywordsInput = _console.ReadLine();

    //        if (string.IsNullOrWhiteSpace(keywordsInput))
    //        {
    //            _console.DisplayError("Keywords cannot be empty.");
    //            _console.PressAnyKeyToContinue();
    //            return;
    //        }

    //        var keywords = keywordsInput.Split(',')
    //            .Select(k => k.Trim())
    //            .Where(k => !string.IsNullOrWhiteSpace(k))
    //            .ToList();

    //        if (!keywords.Any())
    //        {
    //            _console.DisplayError("No valid keywords provided.");
    //            _console.PressAnyKeyToContinue();
    //            return;
    //        }

    //        var response = await _apiService.ConfigureKeywordsAsync(user.Id, keywords);

    //        if (response.Success)
    //        {
    //            _console.DisplaySuccess($"Keywords configured successfully! ({keywords.Count} keywords)");
    //            _console.WriteLine($"Keywords: {string.Join(", ", keywords)}", ConsoleColor.Gray);
    //        }
    //        else
    //        {
    //            _console.DisplayError(response.Message ?? "Failed to configure keywords.");
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        _console.DisplayError($"Error configuring keywords: {ex.Message}");
    //    }

    //    _console.PressAnyKeyToContinue();
    //}
}