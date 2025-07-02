using NewsAggregation.Client.Models.ResponseModels;
using NewsAggregation.Client.Models.ClientModels;
using NewsAggregation.Client.Services.Interfaces;
using NewsAggregation.Client.UI.DisplayServices;
using NewsAggregation.Client.UI.Interfaces;
using NewsAggregationClient.Models.ClientModels;

namespace NewsAggregation.Client.UI.MenuHandlers;

public class AdminMenuHandler : IMenuHandler
{
    private readonly IConsoleService _console;
    private readonly IApiService _apiService;
    private readonly IDisplayService _displayService;
    private readonly ConsoleDisplayService _consoleDisplay;

    public AdminMenuHandler(
        IConsoleService console,
        IApiService apiService,
        IDisplayService displayService,
        ConsoleDisplayService consoleDisplay)
    {
        _console = console;
        _apiService = apiService;
        _displayService = displayService;
        _consoleDisplay = consoleDisplay;
    }

    public async Task HandleMenuAsync(UserDto user)
    {
        while (true)
        {
            _displayService.DisplayUserWelcome(user.Username, DateTime.Now);
            _consoleDisplay.DisplayAdminMenu();

            _console.Write("Enter your choice: ");
            var choice = _console.ReadLine();

            switch (choice)
            {
                case "1":
                    await ViewExternalServersAsync();
                    break;
                case "2":
                    await ViewExternalServerDetailsAsync();
                    break;
                case "3":
                    await UpdateExternalServerAsync();
                    break;
                case "4":
                    await AddNewsCategoryAsync();
                    break;
                case "5":
                    await ViewReportedArticlesAsync();
                    break;
                case "6":
                    await ViewReportedArticlesAsync();
                    break;
                case "7":
                    await HideArticleAsync();
                    break;
                case "8":
                    await HideCategoryAsync();
                    break;
                case "9":
                    await ManageFilteredKeywordsAsync();
                    break;
                case "10":
                    _console.WriteLine("Logging out...", ConsoleColor.Yellow);
                    return;
                default:
                    _console.DisplayError("Invalid choice. Please select 1-10.");
                    _console.PressAnyKeyToContinue();
                    break;
            }
        }
    }

    private async Task ViewExternalServersAsync()
    {
        try
        {
            _console.WriteLine("Loading external servers...", ConsoleColor.Yellow);
            var response = await _apiService.GetExternalServersAsync();
            if (response.Success && response.Data != null && response.Data.Count > 0)
            {
                _console.WriteLine($"\nList of external servers (count: {response.Data.Count}):", ConsoleColor.Cyan);
                foreach (var server in response.Data)
                {
                    _console.WriteLine($"{server.Id}. {server.Name} - {server.Status} - last accessed: {server.LastAccessed}", ConsoleColor.White);
                }
            }
            else
            {
                _console.DisplayError($"No external servers found or unable to parse data. Raw message: {response.Message}");
                if (response.Data != null)
                {
                    _console.WriteLine($"[DEBUG] Data count: {response.Data.Count}", ConsoleColor.Yellow);
                }
            }
        }
        catch (Exception ex)
        {
            _console.DisplayError($"Error loading external servers: {ex.Message}");
        }
        _console.PressAnyKeyToContinue();
    }

    private async Task ViewExternalServerDetailsAsync()
    {
        try
        {
            // First, show a summary list of all servers and their API keys (masked)
            var allServersResponse = await _apiService.GetExternalServersAsync();
            if (allServersResponse.Success && allServersResponse.Data != null && allServersResponse.Data.Count > 0)
            {
                _console.WriteLine("\nList of external server details:", ConsoleColor.Cyan);
                foreach (var server in allServersResponse.Data)
                {
                    var apiKeyDisplay = server.ApiUrl != null ? $"<API KEY: ****{(server.ApiUrl.Length > 4 ? server.ApiUrl[^4..] : server.ApiUrl)}>": "<API KEY: N/A>";
                    _console.WriteLine($"{server.Id}. {server.Name} - {apiKeyDisplay}", ConsoleColor.White);
                }
            }
            else
            {
                _console.DisplayError("No external servers found.");
            }

            // Then, allow the user to select a server for full details
            _console.Write("\nEnter the external server ID to view full details: ");
            var input = _console.ReadLine();

            if (!int.TryParse(input, out int serverId) || serverId <= 0)
            {
                _console.DisplayError("Invalid server ID. Please enter a valid positive number.");
                _console.PressAnyKeyToContinue();
                return;
            }

            _console.WriteLine("Loading server details...", ConsoleColor.Yellow);

            var response = await _apiService.GetExternalServerDetailsAsync(serverId);

            if (response.Success && response.Data != null)
            {
                var server = response.Data;
                _console.WriteLine($"\nServer Details:", ConsoleColor.Cyan);
                _console.WriteLine($"ID: {server.Id}", ConsoleColor.White);
                _console.WriteLine($"Name: {server.Name}", ConsoleColor.White);
                _console.WriteLine($"Type: {server.ServerType}", ConsoleColor.White);
                _console.WriteLine($"Status: {server.Status}", ConsoleColor.White);
                _console.WriteLine($"Last Accessed: {server.LastAccessed}", ConsoleColor.White);
                _console.WriteLine($"API URL: {server.ApiUrl}", ConsoleColor.White);
                _console.WriteLine($"Requests Per Hour: {server.RequestsPerHour}", ConsoleColor.White);
                _console.WriteLine($"Current Hour Requests: {server.CurrentHourRequests}", ConsoleColor.White);
                _console.WriteLine($"Created At: {server.CreatedAt}", ConsoleColor.White);
            }
            else
            {
                _console.DisplayError(response.Message ?? "Failed to load server details.");
            }
        }
        catch (Exception ex)
        {
            _console.DisplayError($"Error loading server details: {ex.Message}");
        }
        _console.PressAnyKeyToContinue();
    }

    private async Task UpdateExternalServerAsync()
    {
        try
        {
            _console.Write("Enter the external server ID: ");
            var input = _console.ReadLine();

            if (!int.TryParse(input, out int serverId) || serverId <= 0)
            {
                _console.DisplayError("Invalid server ID. Please enter a valid positive number.");
                _console.PressAnyKeyToContinue();
                return;
            }

            _console.Write("Enter the updated API key: ");
            var apiKey = _console.ReadLine();

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                _console.DisplayError("API key cannot be empty.");
                _console.PressAnyKeyToContinue();
                return;
            }

            if (apiKey.Length < 10)
            {
                _console.DisplayError("API key seems too short. Please enter a valid API key.");
                _console.PressAnyKeyToContinue();
                return;
            }

            _console.WriteLine("Updating server...", ConsoleColor.Yellow);

            var response = await _apiService.UpdateExternalServerAsync(serverId, apiKey);

            if (response.Success)
            {
                _console.DisplaySuccess("Server updated successfully!");
            }
            else
            {
                _console.DisplayError(response.Message ?? "Failed to update server.");
            }
        }
        catch (Exception ex)
        {
            _console.DisplayError($"Error updating server: {ex.Message}");
        }
        _console.PressAnyKeyToContinue();
    }

    private async Task AddNewsCategoryAsync()
    {
        try
        {
            _console.Write("Enter new category name: ");
            var categoryName = _console.ReadLine();

            if (string.IsNullOrWhiteSpace(categoryName))
            {
                _console.DisplayError("Category name cannot be empty.");
                _console.PressAnyKeyToContinue();
                return;
            }

            categoryName = categoryName.Trim();
            if (categoryName.Length < 2)
            {
                _console.DisplayError("Category name must be at least 2 characters long.");
                _console.PressAnyKeyToContinue();
                return;
            }

            if (categoryName.Length > 50)
            {
                _console.DisplayError("Category name cannot exceed 50 characters.");
                _console.PressAnyKeyToContinue();
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(categoryName, @"^[a-zA-Z0-9\s\-]+$"))
            {
                _console.DisplayError("Category name can only contain letters, numbers, spaces, and hyphens.");
                _console.PressAnyKeyToContinue();
                return;
            }

            _console.Write("Enter category description (optional): ");
            var description = _console.ReadLine() ?? string.Empty;
            if (description.Length > 255)
            {
                _console.DisplayError("Description cannot exceed 255 characters.");
                _console.PressAnyKeyToContinue();
                return;
            }

            _console.WriteLine("Adding new category...", ConsoleColor.Yellow);

            var response = await _apiService.AddNewsCategoryAsync(categoryName, description);

            if (response.Success)
            {
                _console.DisplaySuccess($"Category '{categoryName}' added successfully!");
            }
            else
            {
                _console.DisplayError(response.Message ?? "Failed to add category.");
            }
        }
        catch (Exception ex)
        {
            _console.DisplayError($"Error adding category: {ex.Message}");
        }
        _console.PressAnyKeyToContinue();
    }

    private async Task ViewReportedArticlesAsync()
    {
        try
        {
            _console.WriteLine("Loading reported articles...", ConsoleColor.Yellow);
            var response = await _apiService.GetReportedArticlesAsync();
            
            if (response.Success && response.Data != null && response.Data.Count > 0)
            {
                _console.DisplayHeader("R E P O R T E D   A R T I C L E S");
                _console.WriteLine($"Total reported articles: {response.Data.Count}", ConsoleColor.Cyan);
                _console.WriteLine("");

                foreach (var article in response.Data)
                {
                    _console.WriteLine($"Article: {article}", ConsoleColor.White);
                    _console.DisplaySeparator();
                }
            }
            else
            {
                _console.DisplaySuccess("No reported articles found.");
            }
        }
        catch (Exception ex)
        {
            _console.DisplayError($"Error loading reported articles: {ex.Message}");
        }
        _console.PressAnyKeyToContinue();
    }

    private async Task HideArticleAsync()
    {
        try
        {
            _console.Write("Enter Article ID to hide: ");
            var input = _console.ReadLine();

            if (!int.TryParse(input, out int articleId) || articleId <= 0)
            {
                _console.DisplayError("Invalid Article ID. Please enter a valid positive number.");
                _console.PressAnyKeyToContinue();
                return;
            }

            _console.WriteLine("Hiding article...", ConsoleColor.Yellow);

            var response = await _apiService.HideArticleAsync(articleId);

            if (response.Success)
            {
                _console.DisplaySuccess("Article hidden successfully!");
            }
            else
            {
                _console.DisplayError(response.Message ?? "Failed to hide article.");
            }
        }
        catch (Exception ex)
        {
            _console.DisplayError($"Error hiding article: {ex.Message}");
        }

        _console.PressAnyKeyToContinue();
    }

    private async Task HideCategoryAsync()
    {
        try
        {
            _console.Write("Enter Category ID to hide: ");
            var input = _console.ReadLine();

            if (!int.TryParse(input, out int categoryId) || categoryId <= 0)
            {
                _console.DisplayError("Invalid Category ID. Please enter a valid positive number.");
                _console.PressAnyKeyToContinue();
                return;
            }

            _console.WriteLine("Hiding category...", ConsoleColor.Yellow);

            var response = await _apiService.HideCategoryAsync(categoryId);

            if (response.Success)
            {
                _console.DisplaySuccess("Category hidden successfully!");
            }
            else
            {
                _console.DisplayError(response.Message ?? "Failed to hide category.");
            }
        }
        catch (Exception ex)
        {
            _console.DisplayError($"Error hiding category: {ex.Message}");
        }

        _console.PressAnyKeyToContinue();
    }

    private async Task ManageFilteredKeywordsAsync()
    {
        try
        {
            while (true)
            {
                _console.WriteLine("Filtered Keywords Management:", ConsoleColor.Cyan);
                _console.WriteLine("1. View all filtered keywords");
                _console.WriteLine("2. Add new filtered keyword");
                _console.WriteLine("3. Back to main menu");
                _console.WriteLine("");

                _console.Write("Enter your choice: ");
                var choice = _console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await ViewFilteredKeywordsAsync();
                        break;
                    case "2":
                        await AddFilteredKeywordAsync();
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
        catch (Exception ex)
        {
            _console.DisplayError($"Error managing filtered keywords: {ex.Message}");
            _console.PressAnyKeyToContinue();
        }
    }

    private async Task ViewFilteredKeywordsAsync()
    {
        try
        {
            _console.WriteLine("Loading filtered keywords...", ConsoleColor.Yellow);
            var response = await _apiService.GetFilteredKeywordsAsync();

            if (response.Success && response.Data != null && response.Data.Count > 0)
            {
                _console.WriteLine($"\nFiltered Keywords (count: {response.Data.Count}):", ConsoleColor.Cyan);
                foreach (var keyword in response.Data)
                {
                    _console.WriteLine($"- {keyword}", ConsoleColor.White);
                }
            }
            else
            {
                _console.DisplaySuccess("No filtered keywords found.");
            }
        }
        catch (Exception ex)
        {
            _console.DisplayError($"Error loading filtered keywords: {ex.Message}");
        }
        _console.PressAnyKeyToContinue();
    }

    private async Task AddFilteredKeywordAsync()
    {
        try
        {
            _console.Write("Enter keyword to filter: ");
            var keyword = _console.ReadLine();

            if (string.IsNullOrWhiteSpace(keyword))
            {
                _console.DisplayError("Keyword cannot be empty.");
                _console.PressAnyKeyToContinue();
                return;
            }

            keyword = keyword.Trim().ToLower();
            if (keyword.Length < 2)
            {
                _console.DisplayError("Keyword must be at least 2 characters long.");
                _console.PressAnyKeyToContinue();
                return;
            }

            _console.WriteLine("Adding filtered keyword...", ConsoleColor.Yellow);

            var response = await _apiService.AddFilteredKeywordAsync(keyword);

            if (response.Success)
            {
                _console.DisplaySuccess($"Keyword '{keyword}' added to filter successfully!");
            }
            else
            {
                _console.DisplayError(response.Message ?? "Failed to add filtered keyword.");
            }
        }
        catch (Exception ex)
        {
            _console.DisplayError($"Error adding filtered keyword: {ex.Message}");
        }
        _console.PressAnyKeyToContinue();
    }
}