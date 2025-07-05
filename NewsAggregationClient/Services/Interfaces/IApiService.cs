using NewsAggregationClient.Models.ClientModels;
using NewsAggregationClient.Models.ResponseModels;
using NewsAggregationClient.Models.DTOs.ResponseDTOs;

namespace NewsAggregationClient.Services.Interfaces;

public interface IApiService
{
    Task<TokenResponseDto?> LoginAsync(LoginRequest loginRequest);
    Task<TokenResponseDto?> RegisterAsync(RegisterRequest registerRequest);
  //  Task<ApiResponse<List<ExternalServerResponse>>> GetExternalServersAsync();
  //  Task<ApiResponse<ExternalServerResponse>> GetExternalServerDetailsAsync(int serverId);
   // Task<ApiResponse<object>> UpdateExternalServerAsync(UpdateExternalServerRequest request);
  //  Task<ApiResponse<object>> AddNewsCategoryAsync(AddCategoryRequest request);

    Task<ApiResponse<NewsResponse>> GetHeadlinesAsync(string category = "all", DateTime? date = null);
    Task<ApiResponse<NewsResponse>> GetHeadlinesByDateRangeAsync(string category, DateTime startDate, DateTime endDate);
    Task<ApiResponse<NewsResponse>> SearchNewsAsync(NewsAggregationClient.Models.ClientModels.SearchRequest request);
    Task<ApiResponse<List<SavedArticle>>> GetSavedArticlesAsync(int userId);
    Task<ApiResponse<bool>> SaveArticleAsync(UserDto user, int articleId);
    Task<ApiResponse<bool>> DeleteSavedArticleAsync(int userId, int articleId);
    //Task<ApiResponse<List<ExternalServerResponse>>> GetExternalServersAsync();
    Task<ApiResponse<ExternalServerResponseDto>> GetExternalServerDetailsAsync(int serverId);
    Task<ApiResponse<bool>> UpdateExternalServerAsync(int serverId, string apiKey);
    Task<ApiResponse<bool>> AddNewsCategoryAsync(string categoryName, string description);
    Task<ApiResponse<NotificationSettings>> GetNotificationSettingsAsync();
    Task<ApiResponse<bool>> UpdateNotificationSettingsAsync(NotificationSettings settings);
    Task<ApiResponse<List<NotificationResponse>>> GetNotificationsAsync();
    Task<ApiResponse<bool>> MarkNotificationAsReadAsync(int notificationId);
    //void SetAuthToken(string token);
    //Task<bool> ValidateTokenAsync();
    //Task AddNewsCategoryAsync(AddCategoryRequest addCategoryRequest);
    //Task UpdateExternalServerAsync(UpdateExternalServerRequest updateRequest);
    Task<ApiResponse<List<ExternalServerResponseDto>>> GetExternalServersAsync();
    Task<ApiResponse<bool>> ReportArticleAsync(int articleId, string? reason);
    Task<ApiResponse<List<object>>> GetReportedArticlesAsync();
    Task<ApiResponse<bool>> HideArticleAsync(int articleId);
    Task<ApiResponse<bool>> HideCategoryAsync(int categoryId);
    Task<ApiResponse<bool>> AddFilteredKeywordAsync(string keyword);
    Task<ApiResponse<List<string>>> GetFilteredKeywordsAsync();
    Task<ApiResponse<NewsResponse>> GetPersonalizedHeadlinesAsync();
    Task<ApiResponse<bool>> LikeArticleAsync(int articleId);
    Task<ApiResponse<bool>> DislikeArticleAsync(int articleId);
    Task<ApiResponse<bool>> MarkArticleAsReadAsync(int articleId);
    Task<List<Category>> GetCategoriesAsync();
    Task<ApiResponse<bool>> SendTestEmailNotificationAsync();
    Task<ApiResponse<bool>> TriggerNewsAggregationAsync();
}