using NewsAggregationClient.Models.ClientModels;
using NewsAggregationClient.Services.Interfaces;
using NewsAggregationClient.UI.Validators;

namespace NewsAggregationClient.UI.MenuHandlers;

public class AuthMenuHandler(IConsoleService console, IApiService apiService, AdminMenuHandler adminHandler, UserMenuHandler userHandler)
{
    private readonly IConsoleService _console = console;
    private readonly IApiService _apiService = apiService;
    private readonly AdminMenuHandler _adminHandler = adminHandler;
    private readonly UserMenuHandler _userHandler = userHandler;

    public async Task HandleLoginAsync()
    {
        _console.DisplayHeader("User Login");

        _console.Write("Email: ");
        var email = _console.ReadLine();
        if (!InputValidator.IsValidEmail(email))
        {
            _console.DisplayError(InputValidator.GetEmailValidationMessage());
            _console.PressAnyKeyToContinue();
            return;
        }

        _console.Write("Password: ");
        var password = _console.ReadPassword();
        if (!InputValidator.IsValidPassword(password))
        {
            _console.DisplayError(InputValidator.GetPasswordValidationMessage());
            _console.PressAnyKeyToContinue();
            return;
        }

        _console.WriteLine("Logging in...", ConsoleColor.Yellow);

        var loginRequest = new LoginRequest
        {
            Email = email,
            Password = password
        };

        var response = await _apiService.LoginAsync(loginRequest);

        if (response != null && response.User != null)
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
            _console.DisplayError(response?.Message ?? "Login failed. Please check your credentials.");
            _console.PressAnyKeyToContinue();
        }
    }

    public async Task HandleRegisterAsync()
    {
        _console.DisplayHeader("User Sign up");

        _console.Write("Username: ");
        var username = _console.ReadLine();
        if (!InputValidator.IsValidUsername(username))
        {
            _console.DisplayError(InputValidator.GetUsernameValidationMessage());
            _console.PressAnyKeyToContinue();
            return;
        }

        _console.Write("Email: ");
        var email = _console.ReadLine();
        if (!InputValidator.IsValidEmail(email))
        {
            _console.DisplayError(InputValidator.GetEmailValidationMessage());
            _console.PressAnyKeyToContinue();
            return;
        }

        _console.Write("Password: ");
        var password = _console.ReadPassword();
        if (!InputValidator.IsValidPassword(password))
        {
            _console.DisplayError(InputValidator.GetPasswordValidationMessage());
            _console.PressAnyKeyToContinue();
            return;
        }

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
        if (response != null && response.User != null)
        {
            _console.DisplaySuccess("Sign up successful! Logging you in...");
            _console.PressAnyKeyToContinue();
            await _userHandler.HandleMenuAsync(response.User);
        }
        else
        {
            _console.DisplayError(response?.Message ?? "Sign up failed. Please try again.");
            _console.PressAnyKeyToContinue();
        }
    }
}
