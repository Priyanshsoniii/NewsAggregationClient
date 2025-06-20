﻿using NewsAggregation.Client.Models.ClientModels;
using NewsAggregation.Client.Models.ResponseModels;
using NewsAggregationClient.Models.ClientModels;

namespace NewsAggregation.Client.Services.Interfaces;

public interface IApiService
{
    Task<TokenResponseDto?> LoginAsync(LoginRequest loginRequest);
    Task<TokenResponseDto?> RegisterAsync(RegisterRequest registerRequest);
  //  Task<ApiResponse<List<ExternalServerResponse>>> GetExternalServersAsync();
  //  Task<ApiResponse<ExternalServerResponse>> GetExternalServerDetailsAsync(int serverId);
   // Task<ApiResponse<object>> UpdateExternalServerAsync(UpdateExternalServerRequest request);
  //  Task<ApiResponse<object>> AddNewsCategoryAsync(AddCategoryRequest request);

    Task<ApiResponse<NewsResponse>> GetHeadlinesAsync(string category = "all", DateTime? date = null);
  //  Task<ApiResponse<NewsResponse>> GetHeadlinesByDateRangeAsync(string category, DateTime startDate, DateTime endDate);
    //Task<ApiResponse<NewsResponse>> SearchNewsAsync(SearchRequest request);
    Task<ApiResponse<List<NewsArticle>>> GetSavedArticlesAsync(int userId);
    Task<ApiResponse<bool>> SaveArticleAsync(UserDto user, int articleId);
    Task<ApiResponse<bool>> DeleteSavedArticleAsync(int userId, int articleId);
    //Task<ApiResponse<List<ExternalServerResponse>>> GetExternalServersAsync();
    //Task<ApiResponse<ExternalServerResponse>> GetExternalServerDetailsAsync(int serverId);
    //Task<ApiResponse<bool>> UpdateExternalServerAsync(int serverId, string apiKey);
    //Task<ApiResponse<bool>> AddNewsCategoryAsync(string categoryName);
    //Task<ApiResponse<NotificationSettings>> GetNotificationSettingsAsync();
    //Task<ApiResponse<bool>> UpdateNotificationSettingsAsync(NotificationSettings settings);
    //Task<ApiResponse<List<NotificationResponse>>> GetNotificationsAsync();
    //Task<ApiResponse<bool>> MarkNotificationAsReadAsync(int notificationId);
    //void SetAuthToken(string token);
    //Task<bool> ValidateTokenAsync();
    //Task AddNewsCategoryAsync(AddCategoryRequest addCategoryRequest);
    //Task UpdateExternalServerAsync(UpdateExternalServerRequest updateRequest);
}