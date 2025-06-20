using System.Net.Http.Json;
using System.Text.Json;
using NewsAggregation.Client.Models.ClientModels;
using NewsAggregation.Client.Models.ResponseModels;
using NewsAggregation.Client.Services.Interfaces;
using NewsAggregationClient.Models.ClientModels;

namespace NewsAggregation.Client.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<TokenResponseDto?> LoginAsync(LoginRequest loginRequest)
    {
        var user = new UserLoginDto
        {
            Username = loginRequest.Username,
            Password = loginRequest.Password
        };

        var response = await _httpClient.PostAsJsonAsync("api/AuthManagement/login", user);

        if (response.IsSuccessStatusCode)
        {
            var token = await response.Content.ReadFromJsonAsync<TokenResponseDto>();
            if (token?.User != null)
            {
                Console.WriteLine($"Login successful. Welcome, {token.User.Username} ({token.User.Role})");
            }
            else
            {
                Console.WriteLine("Login successful, but user information is missing in the response.");
            }
            return token;
        }
        else
        {
            Console.WriteLine($"Login failed: {await response.Content.ReadAsStringAsync()}");
            return null;
        }
    }

    public async Task<TokenResponseDto?> RegisterAsync(RegisterRequest registerRequest)
    {

        var user = new UserRegisterDto
        {
            Email = registerRequest.Email,
            Username = registerRequest.Username,
            Password = registerRequest.Password
        };

        var response = await _httpClient.PostAsJsonAsync("api/AuthManagement/register", user);

        if (response.IsSuccessStatusCode)
        {
            var token = await response.Content.ReadFromJsonAsync<TokenResponseDto>();
            if (token?.Message == "User registered successfully")
            {
                Console.WriteLine($"Sign up successful. Welcome)");
            }
            return token;
        }
        else
        {
            Console.WriteLine($"Sign up failed: {await response.Content.ReadAsStringAsync()}");
            return null;
        }
    }

    public async Task<ApiResponse<List<NewsArticle>>> GetSavedArticlesAsync(int userId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/News/saved/{userId}");
            var responseContent = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<List<NewsArticle>>>(responseContent, _jsonOptions);
            return result ?? new ApiResponse<List<NewsArticle>> { Success = false, Message = "Failed to fetch saved articles" };
        }
        catch (Exception ex)
        {
            return new ApiResponse<List<NewsArticle>>
            {
                Success = false,
                Message = "Network error occurred",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<NewsResponse>> GetHeadlinesAsync(string category = "all", DateTime? date = null)
    {
        try
        {
            // Use the correct endpoint
            var url = "api/News/headlines/today";

            var response = await _httpClient.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new ApiResponse<NewsResponse>
                {
                    Success = false,
                    Message = $"Failed to fetch headlines: {response.ReasonPhrase}",
                    Errors = new List<string> { responseContent }
                };
            }

            var articles = JsonSerializer.Deserialize<List<NewsArticle>>(responseContent, _jsonOptions) ?? new List<NewsArticle>();

            var newsResponse = new NewsResponse
            {
                Articles = articles,
                TotalCount = articles.Count,
                Page = 1,
                PageSize = articles.Count
            };

            return new ApiResponse<NewsResponse>
            {
                Success = true,
                Message = "Headlines fetched successfully",
                Data = newsResponse
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<NewsResponse>
            {
                Success = false,
                Message = "Network error occurred",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<bool>> SaveArticleAsync(UserDto user, int articleId)
    {
        try
        {
            var content = JsonContent.Create(new
            {
                userId = user.Id,
                articleId = articleId
            });

            var response = await _httpClient.PostAsync("api/News/saved", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseContent);
            var root = doc.RootElement;

            if (root.TryGetProperty("message", out var messageProp))
            {
                var message = messageProp.GetString() ?? "Article saved successfully";
                return new ApiResponse<bool>
                {
                    Success = true,
                    Message = message,
                    Data = true
                };
            }

            if (bool.TryParse(responseContent, out var saved))
            {
                return new ApiResponse<bool>
                {
                    Success = saved,
                    Message = saved ? "Article saved successfully" : "Failed to save article",
                    Data = saved
                };
            }

            return new ApiResponse<bool> { Success = false, Message = "Failed to save article" };
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool>
            {
                Success = false,
                Message = "Network error occurred",
                Errors = new List<string> { ex.Message }
            };
        }
    }


    Task<ApiResponse<object>> IApiService.AddNewsCategoryAsync(AddCategoryRequest request)
    {
        throw new NotImplementedException();
    }

    Task<ApiResponse<ExternalServerResponse>> IApiService.GetExternalServerDetailsAsync(int serverId)
    {
        throw new NotImplementedException();
    }

    Task<ApiResponse<List<ExternalServerResponse>>> IApiService.GetExternalServersAsync()
    {
        throw new NotImplementedException();
    }

    Task<ApiResponse<object>> IApiService.UpdateExternalServerAsync(UpdateExternalServerRequest request)
    {
        throw new NotImplementedException();
    }

    //public async Task<ApiResponse<UserResponse>> RegisterAsync(RegisterRequest request)
    //{
    //    try
    //    {
    //        var json = JsonSerializer.Serialize(request, _jsonOptions);
    //        var content = new StringContent(json, Encoding.UTF8, "application/json");

    //        var response = await _httpClient.PostAsync($"{_config.BaseUrl}/api/auth/register", content);
    //        var responseContent = await response.Content.ReadAsStringAsync();

    //        var result = JsonSerializer.Deserialize<ApiResponse<UserResponse>>(responseContent, _jsonOptions);
    //        return result ?? new ApiResponse<UserResponse> { Success = false, Message = "Registration failed" };
    //    }
    //    catch (Exception ex)
    //    {
    //        return new ApiResponse<UserResponse>
    //        {
    //            Success = false,
    //            Message = "Network error occurred",
    //            Errors = new List<string> { ex.Message }
    //        };
    //    }
    //}


    //public async Task<ApiResponse<NewsResponse>> GetHeadlinesByDateRangeAsync(string category, DateTime startDate, DateTime endDate)
    //{
    //    try
    //    {
    //        var url = $"{_config.BaseUrl}/api/news/headlines/daterange?category={category}&startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";

    //        var response = await _httpClient.GetAsync(url);
    //        var responseContent = await response.Content.ReadAsStringAsync();

    //        var result = JsonSerializer.Deserialize<ApiResponse<NewsResponse>>(responseContent, _jsonOptions);
    //        return result ?? new ApiResponse<NewsResponse> { Success = false, Message = "Failed to fetch headlines" };
    //    }
    //    catch (Exception ex)
    //    {
    //        return new ApiResponse<NewsResponse>
    //        {
    //            Success = false,
    //            Message = "Network error occurred",
    //            Errors = new List<string> { ex.Message }
    //        };
    //    }
    //}

    //public async Task<ApiResponse<NewsResponse>> SearchNewsAsync(SearchRequest request)
    //{
    //    try
    //    {
    //        var json = JsonSerializer.Serialize(request, _jsonOptions);
    //        var content = new StringContent(json, Encoding.UTF8, "application/json");

    //        var response = await _httpClient.PostAsync($"{_config.BaseUrl}/api/news/search", content);
    //        var responseContent = await response.Content.ReadAsStringAsync();

    //        var result = JsonSerializer.Deserialize<ApiResponse<NewsResponse>>(responseContent, _jsonOptions);
    //        return result ?? new ApiResponse<NewsResponse> { Success = false, Message = "Search failed" };
    //    }
    //    catch (Exception ex)
    //    {
    //        return new ApiResponse<NewsResponse>
    //        {
    //            Success = false,
    //            Message = "Network error occurred",
    //            Errors = new List<string> { ex.Message }
    //        };
    //    }
    //}


    //public async Task<ApiResponse<bool>> DeleteSavedArticleAsync(int articleId)
    //{
    //    try
    //    {
    //        var response = await _httpClient.DeleteAsync($"{_config.BaseUrl}/api/news/saved/{articleId}");
    //        var responseContent = await response.Content.ReadAsStringAsync();

    //        var result = JsonSerializer.Deserialize<ApiResponse<bool>>(responseContent, _jsonOptions);
    //        return result ?? new ApiResponse<bool> { Success = false, Message = "Failed to delete article" };
    //    }
    //    catch (Exception ex)
    //    {
    //        return new ApiResponse<bool>
    //        {
    //            Success = false,
    //            Message = "Network error occurred",
    //            Errors = new List<string> { ex.Message }
    //        };
    //    }
    //}

    //public async Task<ApiResponse<List<ExternalServerResponse>>> GetExternalServersAsync()
    //{
    //    try
    //    {
    //        var response = await _httpClient.GetAsync($"{_config.BaseUrl}/api/admin/external-servers");
    //        var responseContent = await response.Content.ReadAsStringAsync();

    //        var result = JsonSerializer.Deserialize<ApiResponse<List<ExternalServerResponse>>>(responseContent, _jsonOptions);
    //        return result ?? new ApiResponse<List<ExternalServerResponse>> { Success = false, Message = "Failed to fetch external servers" };
    //    }
    //    catch (Exception ex)
    //    {
    //        return new ApiResponse<List<ExternalServerResponse>>
    //        {
    //            Success = false,
    //            Message = "Network error occurred",
    //            Errors = new List<string> { ex.Message }
    //        };
    //    }
    //}

    //public async Task<ApiResponse<ExternalServerResponse>> GetExternalServerDetailsAsync(int serverId)
    //{
    //    try
    //    {
    //        var response = await _httpClient.GetAsync($"{_config.BaseUrl}/api/admin/external-servers/{serverId}");
    //        var responseContent = await response.Content.ReadAsStringAsync();

    //        var result = JsonSerializer.Deserialize<ApiResponse<ExternalServerResponse>>(responseContent, _jsonOptions);
    //        return result ?? new ApiResponse<ExternalServerResponse> { Success = false, Message = "Failed to fetch server details" };
    //    }
    //    catch (Exception ex)
    //    {
    //        return new ApiResponse<ExternalServerResponse>
    //        {
    //            Success = false,
    //            Message = "Network error occurred",
    //            Errors = new List<string> { ex.Message }
    //        };
    //    }
    //}

    //public async Task<ApiResponse<bool>> UpdateExternalServerAsync(int serverId, string apiKey)
    //{
    //    try
    //    {
    //        var request = new { ApiKey = apiKey };
    //        var json = JsonSerializer.Serialize(request, _jsonOptions);
    //        var content = new StringContent(json, Encoding.UTF8, "application/json");

    //        var response = await _httpClient.PutAsync($"{_config.BaseUrl}/api/admin/external-servers/{serverId}", content);
    //        var responseContent = await response.Content.ReadAsStringAsync();

    //        var result = JsonSerializer.Deserialize<ApiResponse<bool>>(responseContent, _jsonOptions);
    //        return result ?? new ApiResponse<bool> { Success = false, Message = "Failed to update server" };
    //    }
    //    catch (Exception ex)
    //    {
    //        return new ApiResponse<bool>
    //        {
    //            Success = false,
    //            Message = "Network error occurred",
    //            Errors = new List<string> { ex.Message }
    //        };
    //    }
    //}

    //public async Task<ApiResponse<bool>> AddNewsCategoryAsync(string categoryName)
    //{
    //    try
    //    {
    //        var request = new { Name = categoryName };
    //        var json = JsonSerializer.Serialize(request, _jsonOptions);
    //        var content = new StringContent(json, Encoding.UTF8, "application/json");

    //        var response = await _httpClient.PostAsync($"{_config.BaseUrl}/api/admin/categories", content);
    //        var responseContent = await response.Content.ReadAsStringAsync();

    //        var result = JsonSerializer.Deserialize<ApiResponse<bool>>(responseContent, _jsonOptions);
    //        return result ?? new ApiResponse<bool> { Success = false, Message = "Failed to add category" };
    //    }
    //    catch (Exception ex)
    //    {
    //        return new ApiResponse<bool>
    //        {
    //            Success = false,
    //            Message = "Network error occurred",
    //            Errors = new List<string> { ex.Message }
    //        };
    //    }
    //}

    //public async Task<ApiResponse<NotificationSettings>> GetNotificationSettingsAsync()
    //{
    //    try
    //    {
    //        var response = await _httpClient.GetAsync($"{_config.BaseUrl}/api/notifications/settings");
    //        var responseContent = await response.Content.ReadAsStringAsync();

    //        var result = JsonSerializer.Deserialize<ApiResponse<NotificationSettings>>(responseContent, _jsonOptions);
    //        return result ?? new ApiResponse<NotificationSettings> { Success = false, Message = "Failed to fetch notification settings" };
    //    }
    //    catch (Exception ex)
    //    {
    //        return new ApiResponse<NotificationSettings>
    //        {
    //            Success = false,
    //            Message = "Network error occurred",
    //            Errors = new List<string> { ex.Message }
    //        };
    //    }
    //}

    //public async Task<ApiResponse<bool>> UpdateNotificationSettingsAsync(NotificationSettings settings)
    //{
    //    try
    //    {
    //        var json = JsonSerializer.Serialize(settings, _jsonOptions);
    //        var content = new StringContent(json, Encoding.UTF8, "application/json");

    //        var response = await _httpClient.PutAsync($"{_config.BaseUrl}/api/notifications/settings", content);
    //        var responseContent = await response.Content.ReadAsStringAsync();

    //        var result = JsonSerializer.Deserialize<ApiResponse<bool>>(responseContent, _jsonOptions);
    //        return result ?? new ApiResponse<bool> { Success = false, Message = "Failed to update settings" };
    //    }
    //    catch (Exception ex)
    //    {
    //        return new ApiResponse<bool>
    //        {
    //            Success = false,
    //            Message = "Network error occurred",
    //            Errors = new List<string> { ex.Message }
    //        };
    //    }
    //}

    //public async Task<ApiResponse<List<NotificationResponse>>> GetNotificationsAsync()
    //{
    //    try
    //    {
    //        var response = await _httpClient.GetAsync($"{_config.BaseUrl}/api/notifications");
    //        var responseContent = await response.Content.ReadAsStringAsync();

    //        var result = JsonSerializer.Deserialize<ApiResponse<List<NotificationResponse>>>(responseContent, _jsonOptions);
    //        return result ?? new ApiResponse<List<NotificationResponse>> { Success = false, Message = "Failed to fetch notifications" };
    //    }
    //    catch (Exception ex)
    //    {
    //        return new ApiResponse<List<NotificationResponse>>
    //        {
    //            Success = false,
    //            Message = "Network error occurred",
    //            Errors = new List<string> { ex.Message }
    //        };
    //    }
    //}

    //public async Task<ApiResponse<bool>> MarkNotificationAsReadAsync(int notificationId)
    //{
    //    try
    //    {
    //        var response = await _httpClient.PutAsync($"{_config.BaseUrl}/api/notifications/{notificationId}/read", null);
    //        var responseContent = await response.Content.ReadAsStringAsync();

    //        var result = JsonSerializer.Deserialize<ApiResponse<bool>>(responseContent, _jsonOptions);
    //        return result ?? new ApiResponse<bool> { Success = false, Message = "Failed to mark notification as read" };
    //    }
    //    catch (Exception ex)
    //    {
    //        return new ApiResponse<bool>
    //        {
    //            Success = false,
    //            Message = "Network error occurred",
    //            Errors = new List<string> { ex.Message }
    //        };
    //    }
    //}

    //public Task AddNewsCategoryAsync(AddCategoryRequest addCategoryRequest)
    //{
    //    throw new NotImplementedException();
    //}
}