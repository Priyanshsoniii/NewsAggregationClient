using System.Net.Http.Json;

namespace NewsAggregationClient.Services
{
    public class AuthService: IAuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<TokenResponseDto?> LoginAsync()
        {
            Console.WriteLine("Enter Username:");
            var username = Console.ReadLine()?.Trim();

            Console.WriteLine("Enter Password:");
            var password = Console.ReadLine()?.Trim();

            var user = new UserLoginDto
            {
                Username = username,
                Password = password
            };

            var response = await _httpClient.PostAsJsonAsync("api/AuthManagement/login", user);

            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadFromJsonAsync<TokenResponseDto>();
                Console.WriteLine($"Login successful. Welcome, {token.Username} ({token.Role})");
                return token;
            }
            else
            {
                Console.WriteLine($"Login failed: {await response.Content.ReadAsStringAsync()}");
                return null;
            }
        }

        public Task RegisterAsync()
        {
            throw new NotImplementedException();
        }
    }
}