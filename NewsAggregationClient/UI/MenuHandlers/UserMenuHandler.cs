using NewsAggregationClient.Services.Interfaces;
using NewsAggregationClient.UI.DisplayServices;
using NewsAggregationClient.UI.Interfaces;
using NewsAggregationClient.Models.ResponseModels;

namespace NewsAggregationClient.UI.MenuHandlers;

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
                    await HandlePersonalizedHeadlinesAsync(user);
                    break;
                case "3":
                    await HandleSavedArticlesAsync(user);
                    break;
                case "4":
                    await HandleSearchAsync(user);
                    break;
                case "5":
                    await HandleNotificationsAsync(user);
                    break;
                case "6":
                    _console.WriteLine("Logging out...", ConsoleColor.Yellow);
                    return;
                default:
                    _console.DisplayError("Invalid choice. Please select 1-6.");
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
                    return; 
                default:
                    _console.DisplayError("Invalid choice. Please select 1-3.");
                    _console.PressAnyKeyToContinue();
                    break;
            }
        }
    }

    private async Task ShowHeadlinesByDateAsync(UserDto user, string dateOption)
    {
        DateTime? fromDate = null;
        DateTime? toDate = null;
        bool isToday = dateOption == "today";

        if (isToday)
        {
            fromDate = DateTime.Today;
            toDate = DateTime.Today.AddDays(1).AddSeconds(-1);
        }

        var categories = await _apiService.GetCategoriesAsync();
        if (categories == null || categories.Count == 0)
        {
            _console.DisplayError("No categories available.");
            _console.PressAnyKeyToContinue();
            return;
        }

        while (true)
        {
            _displayService.DisplayUserWelcome(user.Username, DateTime.Now);
            _consoleDisplay.DisplayCategoryMenu(categories);

            _console.Write("Enter your choice: ");
            var choice = _console.ReadLine();

            if (!int.TryParse(choice, out int index) || index < 1 || index > categories.Count)
            {
                _console.DisplayError($"Invalid choice. Please select 1-{categories.Count}.");
                _console.PressAnyKeyToContinue();
                continue;
            }

            var category = categories[index - 1].Name;
            await DisplayNewsArticlesAsync(user, category, fromDate, toDate, isToday);
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

            var categories = await _apiService.GetCategoriesAsync();
            if (categories == null || categories.Count == 0)
            {
                _console.DisplayError("No categories available.");
                _console.PressAnyKeyToContinue();
                return;
            }

            while (true)
            {
                _displayService.DisplayUserWelcome(user.Username, DateTime.Now);
                _consoleDisplay.DisplayCategoryMenu(categories);

                _console.Write("Enter your choice: ");
                var choice = _console.ReadLine();

                if (!int.TryParse(choice, out int index) || index < 1 || index > categories.Count)
                {
                    _console.DisplayError($"Invalid choice. Please select 1-{categories.Count}.");
                    _console.PressAnyKeyToContinue();
                    continue;
                }

                var category = categories[index - 1].Name; 
                await DisplayNewsArticlesAsync(user, category, startDate, endDate.AddDays(1).AddSeconds(-1), false);
                return;
            }
        }
        catch (Exception ex)
        {
            _console.DisplayError($"Error processing date range: {ex.Message}");
            _console.PressAnyKeyToContinue();
        }
    }

    private async Task DisplayNewsArticlesAsync(UserDto user, string category, DateTime? fromDate = null, DateTime? toDate = null, bool isToday = false)
    {
        try
        {
            _console.WriteLine("Loading headlines...", ConsoleColor.Yellow);

            ApiResponse<NewsResponse> response;
            if (isToday)
            {
                response = await _apiService.GetHeadlinesAsync(category);
            }
            else if (fromDate != null && toDate != null)
            {
                response = await _apiService.GetHeadlinesByDateRangeAsync(category, fromDate.Value, toDate.Value);
            }
            else
            {
                response = await _apiService.GetHeadlinesAsync(category);
            }

            if (response.Success && response.Data != null && response.Data.Articles != null && response.Data.Articles.Any())
            {
                var articles = response.Data.Articles;
                
                if (!string.IsNullOrEmpty(category) && category != "all")
                {
                    articles = articles
                        .Where(a => a.Category?.Name?.Equals(category, StringComparison.OrdinalIgnoreCase) == true)
                        .ToList();
                }
                if (articles.Any())
                {
                    await MarkArticlesAsReadAsync(articles);
                    
                    while (true)
                    {
                        _displayService.DisplayUserWelcome(user.Username, DateTime.Now);
                        _newsDisplay.DisplayNewsArticles(articles, "Headlines");
                        _consoleDisplay.DisplayArticleActionMenu();

                        _console.Write("Enter your choice: ");
                        var choice = _console.ReadLine();

                        switch (choice)
                        {
                            case "1":
                                return;
                            case "2":
                                _console.WriteLine("Logging out...", ConsoleColor.Yellow);
                                Environment.Exit(0);
                                break;
                            case "3":
                                await SaveArticleAsync(user);
                                break;
                            case "4":
                                await ReportArticleAsync(user);
                                break;
                            case "5":
                                await LikeArticleAsync(user);
                                break;
                            case "6":
                                await DislikeArticleAsync(user);
                                break;
                            default:
                                _console.DisplayError("Invalid choice. Please select 1-6.");
                                _console.PressAnyKeyToContinue();
                                break;
                        }
                    }
                }
                else
                {
                    _console.DisplayError("No headlines found for the selected criteria.");
                    _console.PressAnyKeyToContinue();
                }
            }
            else if (response.Success && (response.Data == null || response.Data.Articles == null || !response.Data.Articles.Any()))
            {
                _console.DisplayError("No headlines found for the selected criteria.");
                _console.PressAnyKeyToContinue();
            }
            else
            {
                _console.DisplayError(response.Message ?? "Failed to fetch headlines.");
                _console.PressAnyKeyToContinue();
            }
        }
        catch (Exception ex)
        {
            _console.DisplayError($"Error loading headlines: {ex.Message}");
            _console.PressAnyKeyToContinue();
        }
    }

    private async Task MarkArticlesAsReadAsync(List<NewsArticle> articles)
    {
        try
        {
            var tasks = articles.Select(article => _apiService.MarkArticleAsReadAsync(article.Id));
            await Task.WhenAll(tasks);
            
            _console.WriteLine($"Marked {articles.Count} articles as read", ConsoleColor.Gray);
        }
        catch (Exception ex)
        {
            _console.WriteLine($"Note: Read tracking failed: {ex.Message}", ConsoleColor.Gray);
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
                var articles = response.Data.Select(sa => new NewsArticle 
                { 
                    Id = sa.Id, 
                    Title = sa.Title, 
                    Description = sa.Description, 
                    Url = sa.Url, 
                    Source = sa.Source, 
                    PublishedAt = sa.PublishedAt, 
                    CategoryId = sa.CategoryId,
                    Category = new Category 
                    { 
                        Id = sa.CategoryId, 
                        Name = sa.CategoryName, 
                        Description = sa.CategoryName,
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    }
                }).ToList();
                await MarkArticlesAsReadAsync(articles);
                
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
                            return;
                        case "2":
                            _console.WriteLine("Logging out...", ConsoleColor.Yellow);
                            Environment.Exit(0);
                            break;
                        case "3":
                            await DeleteSavedArticleAsync(user);

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

    private async Task HandleSearchAsync(UserDto user)
    {
        try
        {
            _consoleDisplay.DisplaySearchMenu();
            
            _console.Write("Enter search query: ");
            var query = _console.ReadLine();

            if (string.IsNullOrWhiteSpace(query))
            {
                _console.DisplayError("Search query cannot be empty.");
                _console.PressAnyKeyToContinue();
                return;
            }

            _console.Write("Filter by date range? (y/n): ");
            var useDateRange = _console.ReadLine()?.ToLower() == "y";

            DateTime? fromDate = null;
            DateTime? toDate = null;

            if (useDateRange)
            {
                _console.Write("Enter start date (yyyy-mm-dd) or press Enter to skip: ");
                var startDateInput = _console.ReadLine();
                if (!string.IsNullOrWhiteSpace(startDateInput) && DateTime.TryParse(startDateInput, out DateTime startDate))
                {
                    fromDate = startDate;
                }

                _console.Write("Enter end date (yyyy-mm-dd) or press Enter to skip: ");
                var endDateInput = _console.ReadLine();
                if (!string.IsNullOrWhiteSpace(endDateInput) && DateTime.TryParse(endDateInput, out DateTime endDate))
                {
                    toDate = endDate.AddDays(1).AddSeconds(-1);
                }
            }

            _consoleDisplay.DisplaySearchSortOptions();
            _console.Write("Enter your choice (1-4): ");
            var sortChoice = _console.ReadLine();

            string sortBy = "publishedAt";
            string sortOrder = "desc";

            switch (sortChoice)
            {
                case "1": 
                    sortBy = "publishedAt";
                    sortOrder = "desc";
                    break;
                case "2": 
                    sortBy = "publishedAt";
                    sortOrder = "asc";
                    break;
                case "3": 
                    sortBy = "likes";
                    sortOrder = "desc";
                    break;
                case "4": 
                    sortBy = "dislikes";
                    sortOrder = "desc";
                    break;
                default:
                    _console.WriteLine("Using default sorting (newest first).", ConsoleColor.Yellow);
                    break;
            }

            _console.WriteLine("Searching...", ConsoleColor.Yellow);

            var searchRequest = new NewsAggregationClient.Models.ClientModels.SearchRequest
            {
                Query = query,
                FromDate = fromDate,
                ToDate = toDate,
                SortBy = sortBy,
                SortOrder = sortOrder,
                Page = 1,
                PageSize = 20
            };

            var response = await _apiService.SearchNewsAsync(searchRequest);

            if (response.Success && response.Data != null && response.Data.Articles != null && response.Data.Articles.Any())
            {

                await MarkArticlesAsReadAsync(response.Data.Articles);
                
                while (true)
                {
                    _displayService.DisplayUserWelcome(user.Username, DateTime.Now);
                    _newsDisplay.DisplayNewsArticles(response.Data.Articles, $"Search Results for '{query}'");
                    _consoleDisplay.DisplayArticleActionMenu();

                    _console.Write("Enter your choice: ");
                    var choice = _console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            return; 
                        case "2":
                            _console.WriteLine("Logging out...", ConsoleColor.Yellow);
                            Environment.Exit(0);
                            break;
                        case "3":
                            await SaveArticleAsync(user);
                            break;
                        case "4":
                            await ReportArticleAsync(user);
                            break;
                        case "5":
                            await LikeArticleAsync(user);
                            break;
                        case "6":
                            await DislikeArticleAsync(user);
                            break;
                        default:
                            _console.DisplayError("Invalid choice. Please select 1-6.");
                            _console.PressAnyKeyToContinue();
                            break;
                    }
                }
            }
            else
            {
                _console.DisplayError($"No results found for '{query}'.");
                _console.PressAnyKeyToContinue();
            }
        }
        catch (Exception ex)
        {
            _console.DisplayError($"Error during search: {ex.Message}");
            _console.PressAnyKeyToContinue();
        }
    }

    private async Task HandlePersonalizedHeadlinesAsync(UserDto user)
    {
        try
        {
            _console.WriteLine("Loading personalized headlines...", ConsoleColor.Yellow);

            var response = await _apiService.GetPersonalizedHeadlinesAsync();

            if (response.Success && response.Data != null && response.Data.Articles != null && response.Data.Articles.Any())
            {

                await MarkArticlesAsReadAsync(response.Data.Articles);
                
                while (true)
                {
                    _displayService.DisplayUserWelcome(user.Username, DateTime.Now);
                    _newsDisplay.DisplayNewsArticles(response.Data.Articles, "Personalized Headlines");
                    _consoleDisplay.DisplayArticleActionMenu();

                    _console.Write("Enter your choice: ");
                    var choice = _console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            return; 
                        case "2":
                            _console.WriteLine("Logging out...", ConsoleColor.Yellow);
                            Environment.Exit(0);
                            break;
                        case "3":
                            await SaveArticleAsync(user);
                            break;
                        case "4":
                            await ReportArticleAsync(user);
                            break;
                        case "5":
                            await LikeArticleAsync(user);
                            break;
                        case "6":
                            await DislikeArticleAsync(user);
                            break;
                        default:
                            _console.DisplayError("Invalid choice. Please select 1-6.");
                            _console.PressAnyKeyToContinue();
                            break;
                    }
                }
            }
            else if (response.Success && (response.Data == null || response.Data.Articles == null || !response.Data.Articles.Any()))
            {
                _console.DisplayError("No personalized headlines found.");
                _console.PressAnyKeyToContinue();
            }
            else
            {
                _console.DisplayError(response.Message ?? "Failed to fetch personalized headlines.");
                _console.PressAnyKeyToContinue();
            }
        }
        catch (Exception ex)
        {
            _console.DisplayError($"Error loading personalized headlines: {ex.Message}");
            _console.PressAnyKeyToContinue();
        }
    }

    private async Task ReportArticleAsync(UserDto user)
    {
        try
        {
            _console.Write("Enter Article ID to report: ");
            var input = _console.ReadLine();

            if (!int.TryParse(input, out int articleId) || articleId <= 0)
            {
                _console.DisplayError("Invalid Article ID. Please enter a valid positive number.");
                _console.PressAnyKeyToContinue();
                return;
            }

            _console.Write("Enter reason for reporting (optional): ");
            var reason = _console.ReadLine();

            _console.WriteLine("Reporting article...", ConsoleColor.Yellow);

            var response = await _apiService.ReportArticleAsync(articleId, reason);

            if (response.Success)
            {
                _console.DisplaySuccess($"Article {articleId} reported successfully!");
            }
            else
            {
                _console.DisplayError(response.Message ?? "Failed to report article.");
            }
        }
        catch (Exception ex)
        {
            _console.DisplayError($"Error reporting article: {ex.Message}");
        }

        _console.PressAnyKeyToContinue();
    }

    private async Task LikeArticleAsync(UserDto user)
    {
        try
        {
            _console.Write("Enter Article ID to like: ");
            var input = _console.ReadLine();

            if (!int.TryParse(input, out int articleId) || articleId <= 0)
            {
                _console.DisplayError("Invalid Article ID. Please enter a valid positive number.");
                _console.PressAnyKeyToContinue();
                return;
            }

            _console.WriteLine("Liking article...", ConsoleColor.Yellow);

            var response = await _apiService.LikeArticleAsync(articleId);

            if (response.Success)
            {
                _console.DisplaySuccess($"Article {articleId} liked successfully!");
            }
            else
            {
                _console.DisplayError(response.Message ?? "Failed to like article.");
            }
        }
        catch (Exception ex)
        {
            _console.DisplayError($"Error liking article: {ex.Message}");
        }

        _console.PressAnyKeyToContinue();
    }

    private async Task DislikeArticleAsync(UserDto user)
    {
        try
        {
            _console.Write("Enter Article ID to dislike: ");
            var input = _console.ReadLine();

            if (!int.TryParse(input, out int articleId) || articleId <= 0)
            {
                _console.DisplayError("Invalid Article ID. Please enter a valid positive number.");
                _console.PressAnyKeyToContinue();
                return;
            }

            _console.WriteLine("Disliking article...", ConsoleColor.Yellow);

            var response = await _apiService.DislikeArticleAsync(articleId);

            if (response.Success)
            {
                _console.DisplaySuccess($"Article {articleId} disliked successfully!");
            }
            else
            {
                _console.DisplayError(response.Message ?? "Failed to dislike article.");
            }
        }
        catch (Exception ex)
        {
            _console.DisplayError($"Error disliking article: {ex.Message}");
        }

        _console.PressAnyKeyToContinue();
    }

    private async Task HandleNotificationsAsync(UserDto user)
    {
        while (true)
        {
            _displayService.DisplayUserWelcome(user.Username, DateTime.Now);
            _consoleDisplay.DisplayNotificationsMenu();

            _console.Write("Enter your choice: ");
            var choice = _console.ReadLine();

            switch (choice)
            {
                case "1":
                    await ViewNotificationsAsync(user);
                    break;
                case "2":
                    await ConfigureNotificationsAsync(user);
                    break;
                case "3":
                    return; 
                case "4":
                    _console.WriteLine("Logging out...", ConsoleColor.Yellow);
                    Environment.Exit(0);
                    break;
                default:
                    _console.DisplayError("Invalid choice. Please select 1-4.");
                    _console.PressAnyKeyToContinue();
                    break;
            }
        }
    }

    private async Task ViewNotificationsAsync(UserDto user)
    {
        try
        {
            _console.WriteLine("Loading notifications...", ConsoleColor.Yellow);

            var response = await _apiService.GetNotificationsAsync();

            if (response.Success && response.Data != null && response.Data.Any())
            {
                _displayService.DisplayUserWelcome(user.Username, DateTime.Now);
                _consoleDisplay.DisplayNotifications(response.Data);
            }
            else
            {
                _console.DisplayError("No notifications found.");
            }
        }
        catch (Exception ex)
        {
            _console.DisplayError($"Error loading notifications: {ex.Message}");
        }

        _console.PressAnyKeyToContinue();
    }

    private async Task ConfigureNotificationsAsync(UserDto user)
    {
        try
        {
            _console.WriteLine("Loading notification settings...", ConsoleColor.Yellow);

            var response = await _apiService.GetNotificationSettingsAsync();

            if (response.Success && response.Data != null)
            {
                await HandleNotificationSettingsAsync(user, response.Data);
            }
            else
            {
                _console.DisplayError("Failed to load notification settings.");
            }
        }
        catch (Exception ex)
        {
            _console.DisplayError($"Error loading notification settings: {ex.Message}");
        }

        _console.PressAnyKeyToContinue();
    }

    private async Task HandleNotificationSettingsAsync(UserDto user, NotificationSettings settings)
    {
        while (true)
        {
            _displayService.DisplayUserWelcome(user.Username, DateTime.Now);
            
            var categories = await _apiService.GetCategoriesAsync();
            if (categories == null || categories.Count == 0)
            {
                _console.DisplayError("No categories available.");
                _console.PressAnyKeyToContinue();
                return;
            }

            var settingsResponse = await _apiService.GetNotificationSettingsAsync();
            if (settingsResponse.Success && settingsResponse.Data != null)
            {
                settings = settingsResponse.Data;
            }

            await DisplayDynamicNotificationSettingsMenu(settings, categories);

            _console.Write("Enter your choice: ");
            var choice = _console.ReadLine();

            if (choice == "1")
            {
                settings.EmailNotifications = !settings.EmailNotifications;
                await UpdateNotificationSettingsAsync(settings);
            }
            else if (choice == "2")
            {
                await ConfigureKeywordsAsync(settings);
            }
            else if (choice == "3")
            {
                await SendTestEmailNotificationAsync(user);
            }
            else if (choice == "4")
            {
                return; 
            }
            else if (choice == "5")
            {
                _console.WriteLine("Logging out...", ConsoleColor.Yellow);
                Environment.Exit(0);
            }
            else if (int.TryParse(choice, out int categoryChoice) && categoryChoice >= 6 && categoryChoice <= 5 + categories.Count)
            {
                var categoryIndex = categoryChoice - 6;
                var category = categories[categoryIndex];
                await ToggleNotificationCategoryAsync(category.Id);
            }
            else
            {
                _console.DisplayError($"Invalid choice. Please select 1-{5 + categories.Count}.");
                _console.PressAnyKeyToContinue();
            }
        }
    }

    private async Task DisplayDynamicNotificationSettingsMenu(NotificationSettings settings, List<Category> categories)
    {
        _console.WriteLine("Notification Settings:", ConsoleColor.Yellow);
        _console.WriteLine("");

        var emailStatus = settings.EmailNotifications ? "[ENABLED]" : "[DISABLED]";
        _console.WriteLine($"1. Toggle Email Notifications {emailStatus}", ConsoleColor.White);

        var currentKeywords = new List<string>();
        var hasKeywords = false;
        
        try
        {
            var keywordsResponse = await _apiService.GetCurrentKeywordsAsync();
            if (keywordsResponse.Success && keywordsResponse.Data.Any())
            {
                currentKeywords = keywordsResponse.Data;
                hasKeywords = true;
            }
        }
        catch
        {
            hasKeywords = !string.IsNullOrEmpty(settings.Keywords);
        }
        
        var keywordStatus = hasKeywords ? "[CONFIGURED]" : "[NOT CONFIGURED]";
        _console.WriteLine($"2. Configure Keywords {keywordStatus}", ConsoleColor.White);
        
        if (hasKeywords)
        {
            if (currentKeywords.Any())
            {
                _console.WriteLine($"   Current keywords: {string.Join(", ", currentKeywords)}", ConsoleColor.Cyan);
            }
            else if (!string.IsNullOrEmpty(settings.Keywords))
            {
                _console.WriteLine($"   Current keywords: {settings.Keywords}", ConsoleColor.Cyan);
            }
        }
        
        _console.WriteLine("3. Send Test Email", ConsoleColor.White);
        _console.WriteLine("4. Back to Main Menu", ConsoleColor.White);
        _console.WriteLine("5. Logout", ConsoleColor.White);

        _console.WriteLine("", ConsoleColor.White);
        _console.WriteLine("Category Notifications:", ConsoleColor.Yellow);

        for (int i = 0; i < categories.Count; i++)
        {
            var category = categories[i];
            var categoryStatus = GetCategoryNotificationStatus(settings, category.Name) ? "[ENABLED]" : "[DISABLED]";
            _console.WriteLine($"{i + 6}. Toggle {category.Name} Notifications {categoryStatus}", ConsoleColor.White);
        }

        _console.WriteLine("");
    }

    private bool GetCategoryNotificationStatus(NotificationSettings settings, string categoryName)
    {
        return settings.GetCategoryNotification(categoryName);
    }

    private async Task ToggleNotificationCategoryAsync(int categoryId)
    {
        try
        {
            var response = await _apiService.ToggleNotificationCategoryAsync(categoryId);
            if (response.Success)
            {
                _console.WriteLine("Settings updated successfully!", ConsoleColor.Green);
            }
            else
            {
                _console.DisplayError($"Failed to update settings: {response.Message}");
            }
        }
        catch (Exception ex)
        {
            _console.DisplayError($"Error updating settings: {ex.Message}");
        }
    }

    private async Task UpdateNotificationSettingsAsync(NotificationSettings settings)
    {
        try
        {
            var response = await _apiService.UpdateNotificationSettingsAsync(settings);
            if (response.Success)
            {
                _console.WriteLine("Settings updated successfully!", ConsoleColor.Green);
            }
            else
            {
                _console.DisplayError($"Failed to update settings: {response.Message}");
            }
        }
        catch (Exception ex)
        {
            _console.DisplayError($"Error updating settings: {ex.Message}");
        }
        _console.PressAnyKeyToContinue();
    }

    private async Task ConfigureKeywordsAsync(NotificationSettings settings)
    {
        _console.WriteLine("Enter keywords (comma-separated): ", ConsoleColor.Yellow);
        _console.WriteLine("Example: tech, AI, business, sports", ConsoleColor.Gray);
        _console.WriteLine("Enter 'clear' to remove all keywords", ConsoleColor.Gray);
        var keywordsInput = _console.ReadLine();
        
        if (!string.IsNullOrEmpty(keywordsInput))
        {
            try
            {
                if (keywordsInput.Trim().ToLower() == "clear")
                {
                    var response = await _apiService.UpdateKeywordsAsync(new List<string>());
                    if (response.Success)
                    {
                        _console.WriteLine("All keywords cleared successfully!", ConsoleColor.Green);
                    }
                    else
                    {
                        _console.DisplayError($"Failed to clear keywords: {response.Message}");
                    }
                }
                else
                {
                    var keywords = keywordsInput.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .Where(k => !string.IsNullOrWhiteSpace(k))
                        .Select(k => k.Trim())
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList();

                    if (keywords.Any())
                    {
                        _console.WriteLine($"Processing {keywords.Count} keywords...", ConsoleColor.Yellow);
                        
                        var response = await _apiService.UpdateKeywordsAsync(keywords);
                        
                        if (response.Success)
                        {
                            _console.WriteLine("Keywords updated successfully!", ConsoleColor.Green);
                            _console.WriteLine($"Configured keywords: {string.Join(", ", keywords)}", ConsoleColor.Cyan);
                            _console.WriteLine("You will now receive notifications for articles containing these keywords.", ConsoleColor.Green);
                        }
                        else
                        {
                            _console.DisplayError($"Failed to update keywords: {response.Message}");
                        }
                    }
                    else
                    {
                        _console.DisplayError("No valid keywords entered. Please enter at least one keyword.");
                    }
                }
            }
            catch (Exception ex)
            {
                _console.DisplayError($"Error processing keywords: {ex.Message}");
            }
        }
        else
        {
            _console.DisplayError("No keywords entered.");
        }
        
        _console.PressAnyKeyToContinue();
    }

    private async Task SendTestEmailNotificationAsync(UserDto user)
    {
        try
        {
            _console.WriteLine("Sending test email notification...", ConsoleColor.Yellow);

            var response = await _apiService.SendTestEmailNotificationAsync(user.Email);

            if (response.Success)
            {
                _console.DisplaySuccess("Test email notification sent successfully!");
            }
            else
            {
                _console.DisplayError(response.Message ?? "Failed to send test email notification.");
            }
        }
        catch (Exception ex)
        {
            _console.DisplayError($"Error sending test email: {ex.Message}");
        }
        _console.PressAnyKeyToContinue();
    }
}