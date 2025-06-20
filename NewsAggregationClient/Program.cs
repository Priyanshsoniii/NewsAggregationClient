using NewsAggregation.Client.Services.Interfaces;
using NewsAggregation.Client.Services;
using NewsAggregation.Client.UI.DisplayServices;
using NewsAggregation.Client.UI.MenuHandlers;

public class Program
{
    static async Task Main(string[] args)
    {
        var httpClient = HttpClientFactory.CreateClient();
        IApiService? apiService = new ApiService(httpClient);

        IConsoleService consoleService = new ConsoleService();
        var displayService = new ConsoleDisplayService(consoleService);

  
        var adminHandler = new AdminMenuHandler(consoleService, null, null, null);
        var userHandler = new UserMenuHandler(consoleService, null, null, null, null);

        var authHandler = new AuthMenuHandler(consoleService, apiService, adminHandler, userHandler);

        var mainMenuHandler = new MainMenuHandler(consoleService, displayService, authHandler);

        await mainMenuHandler.HandleMenuAsync();

    }
}


//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Configuration;
//using NewsAggregation.Client.Services.Interfaces;
//using NewsAggregation.Client.Services;
//using NewsAggregation.Client.UI.Interfaces;
//using NewsAggregation.Client.UI.DisplayServices;
//using NewsAggregation.Client.UI.MenuHandlers;
//using System;
//using System.Threading.Tasks;

//namespace NewsAggregation.Client
//{
//    class Program
//    {
//        static async Task Main(string[] args)
//        {
//            // Create host builder with dependency injection
//            var host = CreateHostBuilder(args).Build();

//            try
//            {
//                // Get the main menu handler and start the application
//                var mainMenuHandler = host.Services.GetRequiredService<MainMenuHandler>();
//                await mainMenuHandler.HandleMenuAsync();
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Application error: {ex.Message}");
//                Console.WriteLine("Press any key to exit...");
//                Console.ReadKey();
//            }
//        }

//        static IHostBuilder CreateHostBuilder(string[] args) =>
//            Host.CreateDefaultBuilder(args)
//                .ConfigureServices((context, services) =>
//                {
//                    // Register configuration
//                    var configuration = context.Configuration;

//                    // Register HTTP client
//                    services.AddHttpClient<IApiService, ApiService>(client =>
//                    {
//                        // Configure base address from appsettings.json or use default
//                        var baseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7001";
//                        client.BaseAddress = new Uri(baseUrl);
//                        client.Timeout = TimeSpan.FromSeconds(30);
//                    });

//                    // Register services
//                    services.AddScoped<IApiService, ApiService>();
//                    services.AddScoped<IConsoleService, ConsoleService>();
//                    services.AddScoped<IDisplayService, DisplayService>();
//                    services.AddScoped<ConsoleDisplayService>();

//                    // Register menu handlers
//                    services.AddScoped<MainMenuHandler>();
//                    services.AddScoped<AdminMenuHandler>();
//                    services.AddScoped<UserMenuHandler>();
//                    services.AddScoped<GuestMenuHandler>();

//                    // Register authentication service if you have one
//                    services.AddScoped<IAuthenticationService, AuthenticationService>();
//                });
//    }

//    // Example service implementations that you'll need to create
//    public interface IAuthenticationService
//    {
//        Task<bool> LoginAsync(string username, string password);
//        Task<bool> RegisterAsync(string username, string password, string email);
//        Task LogoutAsync();
//        bool IsAuthenticated { get; }
//        string? CurrentUsername { get; }
//    }

//    public class AuthenticationService : IAuthenticationService
//    {
//        private readonly IApiService _apiService;
//        private readonly IConsoleService _console;

//        public AuthenticationService(IApiService apiService, IConsoleService console)
//        {
//            _apiService = apiService;
//            _console = console;
//        }

//        public bool IsAuthenticated { get; private set; }
//        public string? CurrentUsername { get; private set; }

//        public async Task<bool> LoginAsync(string username, string password)
//        {
//            try
//            {
//                // Implement your login logic here
//                // This is a placeholder - replace with actual API call
//                var loginResult = await _apiService.LoginAsync(username, password);

//                if (loginResult.Success)
//                {
//                    IsAuthenticated = true;
//                    CurrentUsername = username;
//                    return true;
//                }

//                return false;
//            }
//            catch (Exception ex)
//            {
//                _console.DisplayError($"Login failed: {ex.Message}");
//                return false;
//            }
//        }

//        public async Task<bool> RegisterAsync(string username, string password, string email)
//        {
//            try
//            {
//                // Implement your registration logic here
//                var registerResult = await _apiService.RegisterAsync(username, password, email);
//                return registerResult.Success;
//            }
//            catch (Exception ex)
//            {
//                _console.DisplayError($"Registration failed: {ex.Message}");
//                return false;
//            }
//        }

//        public async Task LogoutAsync()
//        {
//            IsAuthenticated = false;
//            CurrentUsername = null;
//            await Task.CompletedTask;
//        }
//    }

//    // Example MainMenuHandler that you'll need to create
//    public class MainMenuHandler : IMenuHandler
//    {
//        private readonly IConsoleService _console;
//        private readonly IAuthenticationService _authService;
//        private readonly AdminMenuHandler _adminMenuHandler;
//        private readonly UserMenuHandler _userMenuHandler;
//        private readonly GuestMenuHandler _guestMenuHandler;
//        private readonly ConsoleDisplayService _consoleDisplay;

//        public MainMenuHandler(
//            IConsoleService console,
//            IAuthenticationService authService,
//            AdminMenuHandler adminMenuHandler,
//            UserMenuHandler userMenuHandler,
//            GuestMenuHandler guestMenuHandler,
//            ConsoleDisplayService consoleDisplay)
//        {
//            _console = console;
//            _authService = authService;
//            _adminMenuHandler = adminMenuHandler;
//            _userMenuHandler = userMenuHandler;
//            _guestMenuHandler = guestMenuHandler;
//            _consoleDisplay = consoleDisplay;
//        }

//        public async Task HandleMenuAsync()
//        {
//            _console.WriteLine("Welcome to News Aggregation Client!", ConsoleColor.Cyan);

//            while (true)
//            {
//                _consoleDisplay.DisplayMainMenu();

//                _console.Write("Enter your choice: ");
//                var choice = _console.ReadLine();

//                switch (choice)
//                {
//                    case "1":
//                        await HandleLoginAsync();
//                        break;
//                    case "2":
//                        await HandleRegisterAsync();
//                        break;
//                    case "3":
//                        await _guestMenuHandler.HandleMenuAsync();
//                        break;
//                    case "4":
//                        _console.WriteLine("Thank you for using News Aggregation Client!", ConsoleColor.Green);
//                        return;
//                    default:
//                        _console.DisplayError("Invalid choice. Please select 1-4.");
//                        _console.PressAnyKeyToContinue();
//                        break;
//                }
//            }
//        }

//        private async Task HandleLoginAsync()
//        {
//            _console.WriteLine("=== LOGIN ===", ConsoleColor.Yellow);

//            _console.Write("Username: ");
//            var username = _console.ReadLine();

//            if (string.IsNullOrWhiteSpace(username))
//            {
//                _console.DisplayError("Username cannot be empty.");
//                _console.PressAnyKeyToContinue();
//                return;
//            }

//            _console.Write("Password: ");
//            var password = _console.ReadLineSecure(); // You'll need to implement this for password input

//            if (string.IsNullOrWhiteSpace(password))
//            {
//                _console.DisplayError("Password cannot be empty.");
//                _console.PressAnyKeyToContinue();
//                return;
//            }

//            _console.WriteLine("Logging in...", ConsoleColor.Yellow);

//            var success = await _authService.LoginAsync(username, password);

//            if (success)
//            {
//                _console.DisplaySuccess("Login successful!");

//                // Determine user role and redirect to appropriate menu
//                // This is a placeholder - you'll need to implement role checking
//                var userRole = await GetUserRoleAsync(username);

//                if (userRole == "Admin")
//                {
//                    // You'll need to get user data for admin menu
//                    var userData = await GetUserDataAsync(username);
//                    await _adminMenuHandler.HandleMenuAsync(userData);
//                }
//                else
//                {
//                    await _userMenuHandler.HandleMenuAsync();
//                }
//            }
//            else
//            {
//                _console.DisplayError("Login failed. Please check your credentials.");
//            }

//            _console.PressAnyKeyToContinue();
//        }

//        private async Task HandleRegisterAsync()
//        {
//            _console.WriteLine("=== REGISTER ===", ConsoleColor.Yellow);

//            _console.Write("Username: ");
//            var username = _console.ReadLine();

//            _console.Write("Email: ");
//            var email = _console.ReadLine();

//            _console.Write("Password: ");
//            var password = _console.ReadLineSecure();

//            // Add validation here

//            var success = await _authService.RegisterAsync(username, password, email);

//            if (success)
//            {
//                _console.DisplaySuccess("Registration successful! You can now login.");
//            }
//            else
//            {
//                _console.DisplayError("Registration failed.");
//            }

//            _console.PressAnyKeyToContinue();
//        }

//        private async Task<string> GetUserRoleAsync(string username)
//        {
//            // Implement role checking logic here
//            // This is a placeholder
//            return "User"; // or "Admin"
//        }

//        private async Task<NewsAggregation.Client.Models.ResponseModels.UserResponse> GetUserDataAsync(string username)
//        {
//            // Implement user data retrieval logic here
//            // This is a placeholder
//            return new NewsAggregation.Client.Models.ResponseModels.UserResponse
//            {
//                Username = username
//                // Add other properties as needed
//            };
//        }
//    }

//    // Placeholder menu handlers - you'll need to implement these
//    public class UserMenuHandler : IMenuHandler
//    {
//        private readonly IConsoleService _console;

//        public UserMenuHandler(IConsoleService console)
//        {
//            _console = console;
//        }

//        public async Task HandleMenuAsync()
//        {
//            _console.WriteLine("User Menu - Coming Soon!", ConsoleColor.Yellow);
//            _console.PressAnyKeyToContinue();
//            await Task.CompletedTask;
//        }
//    }

//    public class GuestMenuHandler : IMenuHandler
//    {
//        private readonly IConsoleService _console;

//        public GuestMenuHandler(IConsoleService console)
//        {
//            _console = console;
//        }

//        public async Task HandleMenuAsync()
//        {
//            _console.WriteLine("Guest Menu - Coming Soon!", ConsoleColor.Yellow);
//            _console.PressAnyKeyToContinue();
//            await Task.CompletedTask;
//        }
//    }
//}