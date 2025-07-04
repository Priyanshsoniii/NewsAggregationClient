﻿using NewsAggregationClient.Models.ResponseModels;
using NewsAggregationClient.Models.DTOs.ResponseDTOs;

namespace NewsAggregationClient.UI.Interfaces;

public interface IDisplayService
{
    void DisplayNewsArticles(List<NewsArticle> articles, string title);
    void DisplayNewsArticle(NewsArticle article);
    void DisplaySavedArticles(List<SavedArticle> articles);
    void DisplaySearchResults(List<NewsArticle> articles, string query);
    void DisplayExternalServers(List<ExternalServerResponseDto> servers);
    void DisplayExternalServerDetails(ExternalServerResponseDto server);
    void DisplayNotifications(List<NotificationResponse> notifications);
    void DisplayNotificationSettings(NotificationSettings settings);
    void DisplayUserWelcome(string username, DateTime currentTime);
    void DisplayMenu(List<string> menuItems, string title);
    void DisplayPaginatedArticles(List<NewsArticle> articles, int currentPage, int totalPages, string title);
    void DisplayCategoryMenu(List<Category> categories);
    void DisplayNotificationSettingsMenu(NotificationSettings settings);
}