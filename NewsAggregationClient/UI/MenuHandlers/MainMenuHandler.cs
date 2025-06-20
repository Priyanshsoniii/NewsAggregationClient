using NewsAggregation.Client.Services.Interfaces;
using NewsAggregation.Client.UI.DisplayServices;
using NewsAggregation.Client.UI.Interfaces;

namespace NewsAggregation.Client.UI.MenuHandlers;

public class MainMenuHandler : IMenuHandler
{
    private readonly IConsoleService _console;
    private readonly ConsoleDisplayService _displayService;
    private readonly AuthMenuHandler _authHandler;

    public MainMenuHandler(
        IConsoleService console,
        ConsoleDisplayService displayService,
        AuthMenuHandler authHandler)
    {
        _console = console;
        _displayService = displayService;
        _authHandler = authHandler;
    }

    public async Task HandleMenuAsync()
    {
        while (true)
        {
            _displayService.DisplayMainMenu();
            _console.Write("Enter your choice: ");
            var choice = _console.ReadLine();

            switch (choice)
            {
                case "1":
                    await _authHandler.HandleLoginAsync();
                    break;
                case "2":
                    await _authHandler.HandleRegisterAsync();
                    break;
                case "3":
                    _console.WriteLine("Thank you for using News Aggregator. Goodbye!", ConsoleColor.Green);
                    return;
                default:
                    _console.DisplayError("Invalid choice. Please select 1, 2, or 3.");
                    _console.PressAnyKeyToContinue();
                    break;
            }
        }
    }

    public Task HandleMenuAsync(UserDto user)
    {
        throw new NotImplementedException();
    }
}