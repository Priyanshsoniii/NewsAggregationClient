using NewsAggregationClient.Services.Interfaces;
using NewsAggregationClient.Services;
using NewsAggregationClient.UI.DisplayServices;
using NewsAggregationClient.UI.MenuHandlers;

public class Program
{
    static async Task Main(string[] args)
    {
        var httpClient = HttpClientFactory.CreateClient();
        IApiService apiService = new ApiService(httpClient);

        IConsoleService consoleService = new ConsoleService();
        var displayService = new ConsoleDisplayService(consoleService);
        var newsDisplayService = new NewsDisplayService(consoleService);

        var adminHandler = new AdminMenuHandler(consoleService, apiService, newsDisplayService, displayService);
        var userHandler = new UserMenuHandler(consoleService, apiService, newsDisplayService, displayService, newsDisplayService);

        var authHandler = new AuthMenuHandler(consoleService, apiService, adminHandler, userHandler);

        var mainMenuHandler = new MainMenuHandler(consoleService, displayService, authHandler);

        await mainMenuHandler.HandleMenuAsync();
    }
}