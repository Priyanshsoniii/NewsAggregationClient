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

    public async Task<ApiResponse<List<SavedArticle>>> GetSavedArticlesAsync(int userId)
    {
        try
        {
            var response = await _httpClient.GetAsync("api/News/saved");
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new ApiResponse<List<SavedArticle>>
                {
                    Success = false,
                    Message = $"Failed to fetch saved articles: {response.ReasonPhrase}",
                    Errors = new List<string> { responseContent }
                };
            }

            List<SavedArticle> articles = new();
            using (var doc = JsonDocument.Parse(responseContent))
            {
                var root = doc.RootElement;
                if (root.TryGetProperty("savedArticles", out var savedArticlesElement) && savedArticlesElement.ValueKind == JsonValueKind.Array)
                {
                    articles = JsonSerializer.Deserialize<List<SavedArticle>>(savedArticlesElement.GetRawText(), _jsonOptions) ?? new List<SavedArticle>();
                }
            }

            return new ApiResponse<List<SavedArticle>>
            {
                Success = true,
                Message = "Saved articles fetched successfully",
                Data = articles
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<List<SavedArticle>>
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

            List<NewsArticle> articles = new();
            try
            {
                articles = JsonSerializer.Deserialize<List<NewsArticle>>(responseContent, _jsonOptions) ?? new List<NewsArticle>();
            }
            catch
            {
                // fallback: try to parse as { "headlines": [...] }
                using (var doc = JsonDocument.Parse(responseContent))
                {
                    var root = doc.RootElement;
                    if (root.TryGetProperty("headlines", out var headlinesElement) && headlinesElement.ValueKind == JsonValueKind.Array)
                    {
                        articles = JsonSerializer.Deserialize<List<NewsArticle>>(headlinesElement.GetRawText(), _jsonOptions) ?? new List<NewsArticle>();
                    }
                }
            }

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

    public async Task<ApiResponse<NewsResponse>> GetHeadlinesByDateRangeAsync(string category, DateTime startDate, DateTime endDate)
    {
        try
        {
            var url = $"api/News/headlines/daterange?category={category}&startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";

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

            List<NewsArticle> articles = new();
            try
            {
                articles = JsonSerializer.Deserialize<List<NewsArticle>>(responseContent, _jsonOptions) ?? new List<NewsArticle>();
            }
            catch
            {
                // fallback: try to parse as { "headlines": [...] }
                using (var doc = JsonDocument.Parse(responseContent))
                {
                    var root = doc.RootElement;
                    if (root.TryGetProperty("headlines", out var headlinesElement) && headlinesElement.ValueKind == JsonValueKind.Array)
                    {
                        articles = JsonSerializer.Deserialize<List<NewsArticle>>(headlinesElement.GetRawText(), _jsonOptions) ?? new List<NewsArticle>();
                    }
                }
            }

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

    public async Task<ApiResponse<NewsResponse>> SearchNewsAsync(NewsAggregationClient.Models.ClientModels.SearchRequest request)
    {
        try
        {
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/News/search", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new ApiResponse<NewsResponse>
                {
                    Success = false,
                    Message = $"Search failed: {response.ReasonPhrase}",
                    Errors = new List<string> { responseContent }
                };
            }

            List<NewsArticle> articles = new();
            using (var doc = JsonDocument.Parse(responseContent))
            {
                var root = doc.RootElement;
                if (root.TryGetProperty("articles", out var articlesElement) && articlesElement.ValueKind == JsonValueKind.Array)
                {
                    articles = JsonSerializer.Deserialize<List<NewsArticle>>(articlesElement.GetRawText(), _jsonOptions) ?? new List<NewsArticle>();
                }
                else if (root.TryGetProperty("data", out var dataElement) && dataElement.TryGetProperty("articles", out var dataArticlesElement) && dataArticlesElement.ValueKind == JsonValueKind.Array)
                {
                    articles = JsonSerializer.Deserialize<List<NewsArticle>>(dataArticlesElement.GetRawText(), _jsonOptions) ?? new List<NewsArticle>();
                }
            }

            var newsResponse = new NewsResponse
            {
                Articles = articles,
                TotalCount = articles.Count,
                Page = request.Page,
                PageSize = request.PageSize
            };

            return new ApiResponse<NewsResponse>
            {
                Success = true,
                Message = "Search completed successfully",
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

    public async Task<ApiResponse<NotificationSettings>> GetNotificationSettingsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/Notification/settings");
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new ApiResponse<NotificationSettings>
                {
                    Success = false,
                    Message = $"Failed to fetch notification settings: {response.ReasonPhrase}",
                    Errors = new List<string> { responseContent }
                };
            }

            var settings = JsonSerializer.Deserialize<NotificationSettings>(responseContent, _jsonOptions);
            return new ApiResponse<NotificationSettings>
            {
                Success = true,
                Message = "Notification settings fetched successfully",
                Data = settings
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<NotificationSettings>
            {
                Success = false,
                Message = "Network error occurred",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<bool>> UpdateNotificationSettingsAsync(NotificationSettings settings)
    {
        try
        {
            var json = JsonSerializer.Serialize(settings, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync("api/Notification/settings", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Notification settings updated successfully",
                    Data = true
                };
            }
            else
            {
                using var doc = JsonDocument.Parse(responseContent);
                var root = doc.RootElement;
                if (root.TryGetProperty("message", out var messageProp))
                {
                    return new ApiResponse<bool>
                    {
                        Success = false,
                        Message = messageProp.GetString() ?? "Failed to update notification settings",
                        Data = false
                    };
                }
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Failed to update notification settings",
                    Data = false
                };
            }
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

    public async Task<ApiResponse<List<NotificationResponse>>> GetNotificationsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/Notification");
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new ApiResponse<List<NotificationResponse>>
                {
                    Success = false,
                    Message = $"Failed to fetch notifications: {response.ReasonPhrase}",
                    Errors = new List<string> { responseContent }
                };
            }

            List<NotificationResponse> notifications = new();
            using (var doc = JsonDocument.Parse(responseContent))
            {
                var root = doc.RootElement;
                if (root.TryGetProperty("notifications", out var notificationsElement) && notificationsElement.ValueKind == JsonValueKind.Array)
                {
                    notifications = JsonSerializer.Deserialize<List<NotificationResponse>>(notificationsElement.GetRawText(), _jsonOptions) ?? new List<NotificationResponse>();
                }
                else if (root.ValueKind == JsonValueKind.Array)
                {
                    notifications = JsonSerializer.Deserialize<List<NotificationResponse>>(responseContent, _jsonOptions) ?? new List<NotificationResponse>();
                }
            }

            return new ApiResponse<List<NotificationResponse>>
            {
                Success = true,
                Message = "Notifications fetched successfully",
                Data = notifications
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<List<NotificationResponse>>
            {
                Success = false,
                Message = "Network error occurred",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<bool>> MarkNotificationAsReadAsync(int notificationId)
    {
        try
        {
            var response = await _httpClient.PatchAsync($"api/Notification/{notificationId}/mark-read", null);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Notification marked as read",
                    Data = true
                };
            }
            else
            {
                using var doc = JsonDocument.Parse(responseContent);
                var root = doc.RootElement;
                if (root.TryGetProperty("message", out var messageProp))
                {
                    return new ApiResponse<bool>
                    {
                        Success = false,
                        Message = messageProp.GetString() ?? "Failed to mark notification as read",
                        Data = false
                    };
                }
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Failed to mark notification as read",
                    Data = false
                };
            }
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

    public async Task<ApiResponse<bool>> ReportArticleAsync(int articleId, string? reason)
    {
        try
        {
            var request = new ReportArticleRequest
            {
                ArticleId = articleId,
                Reason = reason
            };

            var response = await _httpClient.PostAsJsonAsync("api/News/report", request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Article reported successfully",
                    Data = true
                };
            }
            else
            {
                using var doc = JsonDocument.Parse(responseContent);
                var root = doc.RootElement;
                if (root.TryGetProperty("message", out var messageProp))
                {
                    return new ApiResponse<bool>
                    {
                        Success = false,
                        Message = messageProp.GetString() ?? "Failed to report article",
                        Data = false
                    };
                }
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Failed to report article",
                    Data = false
                };
            }
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

    public async Task<ApiResponse<List<object>>> GetReportedArticlesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/Admin/reported-articles");
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new ApiResponse<List<object>>
                {
                    Success = false,
                    Message = $"Failed to fetch reported articles: {response.ReasonPhrase}",
                    Errors = new List<string> { responseContent }
                };
            }

            List<object> reportedArticles = new();
            using (var doc = JsonDocument.Parse(responseContent))
            {
                var root = doc.RootElement;
                if (root.TryGetProperty("reportedArticles", out var reportedArticlesElement) && reportedArticlesElement.ValueKind == JsonValueKind.Array)
                {
                    reportedArticles = JsonSerializer.Deserialize<List<object>>(reportedArticlesElement.GetRawText(), _jsonOptions) ?? new List<object>();
                }
            }

            return new ApiResponse<List<object>>
            {
                Success = true,
                Message = "Reported articles fetched successfully",
                Data = reportedArticles
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<List<object>>
            {
                Success = false,
                Message = "Network error occurred",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<bool>> HideArticleAsync(int articleId)
    {
        try
        {
            var response = await _httpClient.PutAsync($"api/Admin/articles/{articleId}/hide", null);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Article hidden successfully",
                    Data = true
                };
            }
            else
            {
                using var doc = JsonDocument.Parse(responseContent);
                var root = doc.RootElement;
                if (root.TryGetProperty("message", out var messageProp))
                {
                    return new ApiResponse<bool>
                    {
                        Success = false,
                        Message = messageProp.GetString() ?? "Failed to hide article",
                        Data = false
                    };
                }
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Failed to hide article",
                    Data = false
                };
            }
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

    public async Task<ApiResponse<bool>> HideCategoryAsync(int categoryId)
    {
        try
        {
            var response = await _httpClient.PutAsync($"api/Admin/categories/{categoryId}/hide", null);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Category hidden successfully",
                    Data = true
                };
            }
            else
            {
                using var doc = JsonDocument.Parse(responseContent);
                var root = doc.RootElement;
                if (root.TryGetProperty("message", out var messageProp))
                {
                    return new ApiResponse<bool>
                    {
                        Success = false,
                        Message = messageProp.GetString() ?? "Failed to hide category",
                        Data = false
                    };
                }
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Failed to hide category",
                    Data = false
                };
            }
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

    public async Task<ApiResponse<bool>> AddFilteredKeywordAsync(string keyword)
    {
        try
        {
            var request = new { Keyword = keyword };
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/Admin/filtered-keywords", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Keyword added to filter successfully",
                    Data = true
                };
            }
            else
            {
                using var doc = JsonDocument.Parse(responseContent);
                var root = doc.RootElement;
                if (root.TryGetProperty("message", out var messageProp))
                {
                    return new ApiResponse<bool>
                    {
                        Success = false,
                        Message = messageProp.GetString() ?? "Failed to add filtered keyword",
                        Data = false
                    };
                }
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Failed to add filtered keyword",
                    Data = false
                };
            }
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

    public async Task<ApiResponse<List<string>>> GetFilteredKeywordsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/Admin/filtered-keywords");
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new ApiResponse<List<string>>
                {
                    Success = false,
                    Message = $"Failed to fetch filtered keywords: {response.ReasonPhrase}",
                    Errors = new List<string> { responseContent }
                };
            }

            List<string> keywords = new();
            using (var doc = JsonDocument.Parse(responseContent))
            {
                var root = doc.RootElement;
                if (root.TryGetProperty("keywords", out var keywordsElement) && keywordsElement.ValueKind == JsonValueKind.Array)
                {
                    keywords = JsonSerializer.Deserialize<List<string>>(keywordsElement.GetRawText(), _jsonOptions) ?? new List<string>();
                }
            }

            return new ApiResponse<List<string>>
            {
                Success = true,
                Message = "Filtered keywords fetched successfully",
                Data = keywords
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<List<string>>
            {
                Success = false,
                Message = "Network error occurred",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<NewsResponse>> GetPersonalizedHeadlinesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/News/personalized");
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new ApiResponse<NewsResponse>
                {
                    Success = false,
                    Message = $"Failed to fetch personalized headlines: {response.ReasonPhrase}",
                    Errors = new List<string> { responseContent }
                };
            }

            List<NewsArticle> articles = new();
            using (var doc = JsonDocument.Parse(responseContent))
            {
                var root = doc.RootElement;
                if (root.TryGetProperty("articles", out var articlesElement) && articlesElement.ValueKind == JsonValueKind.Array)
                {
                    articles = JsonSerializer.Deserialize<List<NewsArticle>>(articlesElement.GetRawText(), _jsonOptions) ?? new List<NewsArticle>();
                }
                else if (root.TryGetProperty("data", out var dataElement) && dataElement.TryGetProperty("articles", out var dataArticlesElement) && dataArticlesElement.ValueKind == JsonValueKind.Array)
                {
                    articles = JsonSerializer.Deserialize<List<NewsArticle>>(dataArticlesElement.GetRawText(), _jsonOptions) ?? new List<NewsArticle>();
                }
            }

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
                Message = "Personalized headlines fetched successfully",
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

    public async Task<ApiResponse<bool>> LikeArticleAsync(int articleId)
    {
        try
        {
            var response = await _httpClient.PostAsync($"api/News/{articleId}/like", null);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Article liked successfully",
                    Data = true
                };
            }
            else
            {
                using var doc = JsonDocument.Parse(responseContent);
                var root = doc.RootElement;
                if (root.TryGetProperty("message", out var messageProp))
                {
                    return new ApiResponse<bool>
                    {
                        Success = false,
                        Message = messageProp.GetString() ?? "Failed to like article",
                        Data = false
                    };
                }
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Failed to like article",
                    Data = false
                };
            }
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

    public async Task<ApiResponse<bool>> DislikeArticleAsync(int articleId)
    {
        try
        {
            var response = await _httpClient.PostAsync($"api/News/{articleId}/dislike", null);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Article disliked successfully",
                    Data = true
                };
            }
            else
            {
                using var doc = JsonDocument.Parse(responseContent);
                var root = doc.RootElement;
                if (root.TryGetProperty("message", out var messageProp))
                {
                    return new ApiResponse<bool>
                    {
                        Success = false,
                        Message = messageProp.GetString() ?? "Failed to dislike article",
                        Data = false
                    };
                }
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Failed to dislike article",
                    Data = false
                };
            }
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

    public async Task<ApiResponse<bool>> MarkArticleAsReadAsync(int articleId)
    {
        try
        {
            var response = await _httpClient.PostAsync($"api/News/{articleId}/read", null);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Article marked as read",
                    Data = true
                };
            }
            else
            {
                using var doc = JsonDocument.Parse(responseContent);
                var root = doc.RootElement;
                if (root.TryGetProperty("message", out var messageProp))
                {
                    return new ApiResponse<bool>
                    {
                        Success = false,
                        Message = messageProp.GetString() ?? "Failed to mark article as read",
                        Data = false
                    };
                }
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Failed to mark article as read",
                    Data = false
                };
            }
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

    public async Task<List<Category>> GetCategoriesAsync()
    {
        var response = await _httpClient.GetAsync("api/Category");
        var responseContent = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            return new List<Category>();
        }
        return JsonSerializer.Deserialize<List<Category>>(responseContent, _jsonOptions) ?? new List<Category>();
    }

    public async Task<ApiResponse<bool>> SendTestEmailNotificationAsync()
    {
        try
        {
            var response = await _httpClient.PostAsync("api/Notification/test-email", null);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Test email sent successfully.",
                    Data = true
                };
            }
            else
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Failed to send test email.",
                    Errors = new List<string> { responseContent }
                };
            }
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
}