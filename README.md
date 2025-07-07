# News Aggregation Server

## Overview

The News Aggregation Server is a .NET-based backend application that periodically collects news from multiple external APIs, processes and categorizes the news, manages user authentication, and serves structured news data to a client application via RESTful APIs. It supports user and admin roles, notification management, article reporting, and personalized news recommendations.

---

## Features

- **Periodic News Aggregation:** Fetches news headlines from external APIs (e.g., NewsAPI, The News API) every 3-4 hours and stores them in a relational database.
- **RESTful API:** Exposes endpoints for user authentication, news retrieval, article management, notifications, and admin operations.
- **User Management:** Handles registration, login, and role-based access (Admin/User).
- **News Categorization:** Automatically assigns categories to news articles, even if missing from the source, using content analysis.
- **Notifications:** Sends news notifications and articles to users via email, based on their preferences and keywords.
- **Personalization:** Recommends news based on user interests, saved articles, likes, and notification settings.
- **Admin Controls:** Allows admins to manage external news sources, categories, filter/hide articles or categories, and handle reported content.
- **Reporting & Moderation:** Users can report articles; admins are notified and can hide content. Articles with excessive reports are auto-hidden.
- **Layered Architecture:** Follows clean code, SOLID principles, and a layered structure (Controllers, Services, Repositories, Models).
- **API Documentation:** All endpoints are documented (e.g., via Swagger).
- **Testing:** Includes unit and system test cases for core logic.

---

## Architecture

- **Controllers:** Handle HTTP requests and responses.
- **Services:** Business logic for news aggregation, user management, notifications, etc.
- **Repositories:** Data access layer for interacting with the database.
- **Models/DTOs:** Define data structures for entities and API communication.
- **Background Services:** Periodically fetch and process news from external APIs.
- **Utilities:** Helper classes for tasks like email sending, category inference, etc.

---

## Database

- **Type:** Relational (e.g., SQL Server, PostgreSQL)
- **Why Relational?**  
  - Supports complex queries, relationships (users, articles, categories, notifications).
  - Ensures data integrity and transactional consistency.
  - Scalable for user/content management and reporting features.

---

## External APIs

- **NewsAPI:** https://newsapi.org/
- **The News API:** https://www.thenewsapi.com/documentation
- **(Optional) Firebase API for test data**

---

## Key Endpoints

- **Authentication:**  
  - `POST /api/AuthManagement/login`  
  - `POST /api/AuthManagement/register`
- **News:**  
  - `GET /api/News/headlines/today`  
  - `GET /api/News/headlines/range`  
  - `POST /api/News/save`  
  - `DELETE /api/News/saved/{id}`
- **Search:**  
  - `POST /api/News/search`
- **Notifications:**  
  - `GET /api/Notifications`  
  - `POST /api/Notifications/configure`
- **Admin:**  
  - `GET /api/Admin/servers`  
  - `PUT /api/Admin/servers/{id}`  
  - `POST /api/Admin/categories`  
  - `POST /api/Admin/hide-article`  
  - `POST /api/Admin/hide-category`  
  - `POST /api/Admin/filter-keywords`
- **Reporting:**  
  - `POST /api/News/report`

(See Swagger UI for full documentation.)

---

## Background Jobs

- **News Fetching:** Runs every 3-4 hours to update news from all configured external sources.
- **Category Inference:** Assigns categories to uncategorized articles using keyword analysis.

---

## Notification & Email

- Users can configure notification preferences and keywords.
- Server sends relevant news and notifications via email (from: priyanshsoni2001@gmail.com).
- All notification actions are available via API and email.

---

## Personalization

- News recommendations are tailored using:
  - User’s notification settings
  - Configured keywords
  - Liked/saved articles
  - Reading history

---

## Admin Features

- View, add, update, and manage external news sources.
- Add/edit news categories.
- View and moderate reported articles.
- Hide articles/categories or filter by keywords.
- Trigger manual news aggregation and fix invalid categories.

---

## Clean Code & Best Practices

- Follows SOLID principles and layered architecture.
- All business logic is separated from controllers.
- Uses dependency injection for services and repositories.
- Comprehensive error handling and validation.
- Unit and system tests included.

---

## Getting Started

1. **Clone the repository**
2. **Configure the database** (update connection string in `appsettings.json`)
3. **Set up external API keys** (NewsAPI, The News API)
4. **Run database migrations**
5. **Start the server application**
6. **Access Swagger UI** for API documentation and testing

---

## API Documentation

- Swagger UI available at `/swagger` when running the server.

---

## Testing

- Run unit and system tests using the test project (see solution structure).

---

## License

MIT

---

## Contact

For any issues or contributions, please open an issue or pull request on the repository.

---

**Note:**  
This server is designed to work seamlessly with the News Aggregation Client application, which provides a console-based user interface for all features described above. 