using NewsAggregation.Client.Models.ClientModels;
using NewsAggregation.Client.Services.Interfaces;

namespace NewsAggregation.Client.UI.MenuHandlers;

public class AuthMenuHandler(IConsoleService console, IApiService apiService, AdminMenuHandler adminHandler, UserMenuHandler userHandler)
{
    private readonly IConsoleService _console = console;
    private readonly IApiService _apiService = apiService;
    private readonly AdminMenuHandler _adminHandler = adminHandler;
    private readonly UserMenuHandler _userHandler = userHandler;

    public async Task HandleLoginAsync()
    {
        _console.DisplayHeader("User Login");

        _console.Write("Username: ");
        var username = _console.ReadLine();

        _console.Write("Password: ");
        var password = _console.ReadPassword();

        if (string.IsNullOrWhiteSpace(password))
        {
            _console.DisplayError("Password cannot be empty.");
            _console.PressAnyKeyToContinue();
            return;
        }

        _console.WriteLine("Logging in...", ConsoleColor.Yellow);

        var loginRequest = new LoginRequest
        {
            Username = username,
            Password = password
        };

        var response = await _apiService.LoginAsync(loginRequest);

        if (response!= null && response.User != null)
        {
            _console.DisplaySuccess("Login successful!");
            _console.PressAnyKeyToContinue();

            if (response.User.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                await _adminHandler.HandleMenuAsync(response.User);
            }
            else
            {
                await _userHandler.HandleMenuAsync(response.User);
            }
        }
        else
        {
            _console.DisplayError(response.Message);
            if (response.Message.Any())
            {
                _console.DisplayError(response.Message);
            }
            _console.PressAnyKeyToContinue();
        }
    }

    public async Task HandleRegisterAsync()
    {
        _console.DisplayHeader("User Sign up");

        _console.Write("Username: ");
        var username = _console.ReadLine();

        _console.Write("Email: ");
        var email = _console.ReadLine();

        _console.Write("Password: ");
        var password = _console.ReadPassword();

        _console.Write("Confirm Password: ");
        var confirmPassword = _console.ReadPassword();

        if (password != confirmPassword)
        {
            _console.DisplayError("Passwords do not match.");
            _console.PressAnyKeyToContinue();
            return;
        }

        _console.WriteLine("Creating account...", ConsoleColor.Yellow);

        var registerRequest = new RegisterRequest
        {
            Username = username,
            Email = email,
            Password = password
        };

        var response = await _apiService.RegisterAsync(registerRequest);
    
       // await _userHandler.HandleMenuAsync(response.User);
    }
}
