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
                  //  await ViewExternalServersAsync();
                    break;
                case "2":
                 //   await ViewExternalServerDetailsAsync();
                    break;
                case "3":
                 //   await UpdateExternalServerAsync();
                    break;
                case "4":
                 //   await AddNewsCategoryAsync();
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

    //private async Task UpdateExternalServerAsync()
    //{
    //    throw new NotImplementedException();
    //}

    //private async Task ViewExternalServersAsync()
    //{
    //    try
    //    {
    //        _console.WriteLine("Loading external servers...", ConsoleColor.Yellow);

    //        var response = await _apiService.GetExternalServersAsync();

    //        if (response.Success && response.Data != null)
    //        {
    //            _displayService.DisplayExternalServers(response.Data);
    //        }
    //        else
    //        {
    //            _console.DisplayError(response.Message ?? "Failed to load external servers.");
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        _console.DisplayError($"Error loading external servers: {ex.Message}");
    //    }

    //    _console.PressAnyKeyToContinue();
    //}

    //private async Task ViewExternalServerDetailsAsync()
    //{
    //    try
    //    {
    //        _console.Write("Enter the external server ID: ");
    //        var input = _console.ReadLine();

    //        if (!int.TryParse(input, out int serverId) || serverId <= 0)
    //        {
    //            _console.DisplayError("Invalid server ID. Please enter a valid positive number.");
    //            _console.PressAnyKeyToContinue();
    //            return;
    //        }

    //        _console.WriteLine("Loading server details...", ConsoleColor.Yellow);

    //        var response = await _apiService.GetExternalServerDetailsAsync(serverId);

    //        if (response.Success && response.Data != null)
    //        {
    //            _displayService.DisplayExternalServerDetails(response.Data);
    //        }
    //        else
    //        {
    //            _console.DisplayError(response.Message ?? "Failed to load server details.");
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        _console.DisplayError($"Error loading server details: {ex.Message}");
    //    }

    //    _console.PressAnyKeyToContinue();
    //}

    //private async Task UpdateExternalServerAsync()
    //{
    //    try
    //    {
    //        _console.Write("Enter the external server ID: ");
    //        var input = _console.ReadLine();

    //        if (!int.TryParse(input, out int serverId) || serverId <= 0)
    //        {
    //            _console.DisplayError("Invalid server ID. Please enter a valid positive number.");
    //            _console.PressAnyKeyToContinue();
    //            return;
    //        }

    //        _console.Write("Enter the updated API key: ");
    //        var apiKey = _console.ReadLine();

    //        if (string.IsNullOrWhiteSpace(apiKey))
    //        {
    //            _console.DisplayError("API key cannot be empty.");
    //            _console.PressAnyKeyToContinue();
    //            return;
    //        }

    //        // Validate API key format (basic validation)
    //        if (apiKey.Length < 10)
    //        {
    //            _console.DisplayError("API key seems too short. Please enter a valid API key.");
    //            _console.PressAnyKeyToContinue();
    //            return;
    //        }

    //        _console.WriteLine("Updating server...", ConsoleColor.Yellow);

    //        var updateRequest = new UpdateExternalServerRequest
    //        {
    //            ServerId = serverId,
    //            ApiKey = apiKey
    //        };

    //        var response = await _apiService.UpdateExternalServerAsync(updateRequest);

    //        if (response.Success)
    //        {
    //            _console.DisplaySuccess("Server updated successfully!");
    //        }
    //        else
    //        {
    //            _console.DisplayError(response.Message ?? "Failed to update server.");
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        _console.DisplayError($"Error updating server: {ex.Message}");
    //    }

    //    _console.PressAnyKeyToContinue();
    //}

    //private async Task AddNewsCategoryAsync()
    //{
    //    try
    //    {
    //        _console.Write("Enter new category name: ");
    //        var categoryName = _console.ReadLine();

    //        if (string.IsNullOrWhiteSpace(categoryName))
    //        {
    //            _console.DisplayError("Category name cannot be empty.");
    //            _console.PressAnyKeyToContinue();
    //            return;
    //        }

    //        // Basic validation for category name  
    //        categoryName = categoryName.Trim();
    //        if (categoryName.Length < 2)
    //        {
    //            _console.DisplayError("Category name must be at least 2 characters long.");
    //            _console.PressAnyKeyToContinue();
    //            return;
    //        }

    //        if (categoryName.Length > 50)
    //        {
    //            _console.DisplayError("Category name cannot exceed 50 characters.");
    //            _console.PressAnyKeyToContinue();
    //            return;
    //        }

    //        // Check for valid characters (letters, numbers, spaces, hyphens)  
    //        if (!System.Text.RegularExpressions.Regex.IsMatch(categoryName, @"^[a-zA-Z0-9\s\-]+$"))
    //        {
    //            _console.DisplayError("Category name can only contain letters, numbers, spaces, and hyphens.");
    //            _console.PressAnyKeyToContinue();
    //            return;
    //        }

    //        _console.WriteLine("Adding new category...", ConsoleColor.Yellow);

    //        var addCategoryRequest = new AddCategoryRequest
    //        {
    //            CategoryName = categoryName
    //        };

    //        // Fix for CS0815: Explicitly call the method without assigning its return value to a variable  
    //        await _apiService.AddNewsCategoryAsync(addCategoryRequest);

    //        _console.DisplaySuccess($"Category '{categoryName}' added successfully!");
    //    }
    //    catch (Exception ex)
    //    {
    //        _console.DisplayError($"Error adding category: {ex.Message}");
    //    }

    //    _console.PressAnyKeyToContinue();
    //}
}