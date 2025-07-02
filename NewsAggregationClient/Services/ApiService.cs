using System.Net.Http.Json;
using System.Text.Json;
using NewsAggregation.Client.Models.ClientModels;
using NewsAggregation.Client.Models.ResponseModels;
using NewsAggregation.Client.Services.Interfaces;
using NewsAggregationClient.Models.ClientModels;
using NewsAggregationClient.Models.DTOs.ResponseDTOs;
using System.Text;

namespace NewsAggregation.Client.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private string? _jwtToken;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    public void SetJwtToken(string token)
    {
        _jwtToken = token;
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
        else
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }

    public async Task<TokenResponseDto?> LoginAsync(LoginRequest loginRequest)
    {
        var user = new UserLoginDto
        {
            Email = loginRequest.Email,
            Password = loginRequest.Password
        };

        var response = await _httpClient.PostAsJsonAsync("api/AuthManagement/login", user);

        if (response.IsSuccessStatusCode)
        {
            var token = await response.Content.ReadFromJsonAsync<TokenResponseDto>();
            if (!string.IsNullOrEmpty(token?.Token))
            {
                SetJwtToken(token.Token);
            }
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
            if (!string.IsNullOrEmpty(token?.Token))
            {
                SetJwtToken(token.Token);
            }
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

            var articles = JsonSerializer.Deserialize<List<NewsArticle>>(responseContent, _jsonOptions) ?? new List<NewsArticle>();

            return new ApiResponse<List<NewsArticle>>
            {
                Success = true,
                Message = "Saved articles fetched successfully",
                Data = articles
            };
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

    public async Task<ApiResponse<bool>> DeleteSavedArticleAsync(int userId, int articleId)
    {   
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, "api/News/saved")
            {
                Content = JsonContent.Create(new { userId, articleId })
            };

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseContent);
            var root = doc.RootElement;

            if (root.TryGetProperty("message", out var messageProp))
            {
                var message = messageProp.GetString() ?? "Article deleted successfully";
                return new ApiResponse<bool>
                {
                    Success = true,
                    Message = message,
                    Data = true
                };
            }

            if (bool.TryParse(responseContent, out var deleted))
            {
                return new ApiResponse<bool>
                {
                    Success = deleted,
                    Message = deleted ? "Article deleted successfully" : "Failed to delete article",
                    Data = deleted
                };
            }

            return new ApiResponse<bool> { Success = false, Message = "Failed to delete article" };
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

    public async Task<ApiResponse<List<ExternalServerResponseDto>>> GetExternalServersAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/Admin/servers");
            var responseContent = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<List<ExternalServerResponseDto>>>(responseContent, _jsonOptions);
            bool foundServers = false;
            List<ExternalServerResponseDto>? servers = null;
            using (var doc = JsonDocument.Parse(responseContent))
            {
                if (doc.RootElement.TryGetProperty("servers", out var serversElement) && serversElement.ValueKind == JsonValueKind.Array)
                {
                    servers = JsonSerializer.Deserialize<List<ExternalServerResponseDto>>(serversElement.GetRawText(), _jsonOptions);
                    foundServers = servers != null && servers.Count > 0;
                }
            }
            if (foundServers)
            {
                return new ApiResponse<List<ExternalServerResponseDto>>
                {
                    Success = true,
                    Message = "External servers retrieved successfully",
                    Data = servers
                };
            }
            return result ?? new ApiResponse<List<ExternalServerResponseDto>> { Success = false, Message = "Failed to fetch external servers" };
        }
        catch (Exception ex)
        {
            return new ApiResponse<List<ExternalServerResponseDto>>
            {
                Success = false,
                Message = "Network error occurred",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<ExternalServerResponseDto>> GetExternalServerDetailsAsync(int serverId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/Admin/servers/{serverId}");
            var responseContent = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<ExternalServerResponseDto>>(responseContent, _jsonOptions);
            // Fallback: try to extract 'server' property if present
            using (var doc = JsonDocument.Parse(responseContent))
            {
                if (doc.RootElement.TryGetProperty("server", out var serverElement) && serverElement.ValueKind == JsonValueKind.Object)
                {
                    var server = JsonSerializer.Deserialize<ExternalServerResponseDto>(serverElement.GetRawText(), _jsonOptions);
                    return new ApiResponse<ExternalServerResponseDto>
                    {
                        Success = true,
                        Message = "Server details retrieved successfully",
                        Data = server
                    };
                }
            }
            return result ?? new ApiResponse<ExternalServerResponseDto> { Success = false, Message = "Failed to fetch server details" };
        }
        catch (Exception ex)
        {
            return new ApiResponse<ExternalServerResponseDto>
            {
                Success = false,
                Message = "Network error occurred",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<bool>> UpdateExternalServerAsync(int serverId, string apiKey)
    {
        try
        {
            var request = new { ApiKey = apiKey };
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"api/Admin/servers/{serverId}", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Accept both { success, message, data } and { message, server }
            using (var doc = JsonDocument.Parse(responseContent))
            {
                if (doc.RootElement.TryGetProperty("message", out var msgProp) &&
                    msgProp.GetString()?.ToLower().Contains("updated") == true)
                {
                    return new ApiResponse<bool> { Success = true, Message = msgProp.GetString() ?? "Server updated successfully", Data = true };
                }
            }
            return new ApiResponse<bool> { Success = false, Message = "Failed to update server" };
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

    public async Task<ApiResponse<bool>> AddNewsCategoryAsync(string categoryName, string description)
    {
        try
        {
            var request = new { Name = categoryName, Description = description };
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/Admin/categories", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            using (var doc = JsonDocument.Parse(responseContent))
            {
                if (doc.RootElement.TryGetProperty("message", out var msgProp) &&
                    msgProp.GetString()?.ToLower().Contains("created") == true)
                {
                    return new ApiResponse<bool> { Success = true, Message = msgProp.GetString() ?? "Category added successfully", Data = true };
                }
            }
            return new ApiResponse<bool> { Success = false, Message = "Failed to add category" };
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