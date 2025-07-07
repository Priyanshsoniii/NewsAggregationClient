# News Aggregation Client

## Overview

The News Aggregation Client is a .NET console application that provides a user-friendly interface for accessing news content from the News Aggregation Server. It features user authentication, role-based access control (Admin/User), news browsing, article management, search functionality, and notification configuration. The application communicates with the server via RESTful APIs and provides a comprehensive news aggregation experience.

---

## Features

### Core Features
- **User Authentication:** Secure login and registration with email validation
- **Role-Based Access:** Different interfaces for Admin and User roles
- **News Browsing:** View headlines by category, date range, and personalized recommendations
- **Article Management:** Save, delete, like, dislike, and report articles
- **Search Functionality:** Search articles with date range filtering and sorting options
- **Notification System:** Configure and manage email notifications based on preferences and keywords
- **Personalization:** Receive tailored news recommendations based on user behavior

### Admin Features
- **External Server Management:** View, update, and manage external news sources
- **Category Management:** Add new news categories
- **Content Moderation:** View reported articles and hide inappropriate content
- **System Management:** Trigger news aggregation and fix data issues

### User Features
- **News Consumption:** Browse headlines by category and date
- **Article Interaction:** Save, like, dislike, and report articles
- **Personalized Feed:** Get recommendations based on preferences
- **Notification Configuration:** Set up email alerts for specific categories and keywords

---

## Architecture

### Project Structure
```
NewsAggregationClient/
├── Configuration/
│   └── HttpClient.cs          # HTTP client configuration
├── Models/
│   ├── ClientModels/          # Request models
│   ├── DTOs/                  # Data transfer objects
│   └── ResponseModels/        # API response models
├── Services/
│   ├── Interfaces/            # Service contracts
│   ├── ApiService.cs          # API communication
│   └── ConsoleService.cs      # Console I/O operations
├── UI/
│   ├── DisplayServices/       # Display logic
│   ├── Interfaces/            # UI contracts
│   ├── MenuHandlers/          # Menu navigation logic
│   └── Validators/            # Input validation
└── Program.cs                 # Application entry point
```

### Design Patterns
- **Dependency Injection:** Services are injected for loose coupling
- **Repository Pattern:** API service abstracts data access
- **Factory Pattern:** HTTP client factory for configuration
- **Strategy Pattern:** Different menu handlers for different user roles
- **Observer Pattern:** Event-driven notification system

---

## User Roles

### Admin Role
- Full system access and management capabilities
- External server configuration and monitoring
- Content moderation and category management
- System maintenance and troubleshooting

### User Role
- News browsing and consumption
- Article interaction (save, like, dislike, report)
- Notification configuration
- Personalized content recommendations

---

## Application Flow

### 1. Application Startup
```
Welcome to the News Aggregator application. Please choose the options below.
1. Login
2. Sign up
3. Exit
```

### 2. Authentication Flow
- **Login:** Email and password validation
- **Registration:** Username, email (with validation), and password
- **Role Detection:** Automatic role-based menu assignment

### 3. Admin Menu Flow
```
1. View the list of external servers and status
2. View the external server's details
3. Update/Edit the external server's details
4. Add new News Category
5. View reported articles
6. Hide articles
7. Hide categories
8. Manage filtered keywords
9. Trigger news aggregation
10. Fix invalid categories
11. Logout
```

### 4. User Menu Flow
```
1. Headlines
2. Personalized Headlines
3. Saved Articles
4. Search
5. Notifications
6. Logout
```

---

## Menu Structure

### Headlines Sub-Menu
```
1. Today
2. Date range
3. Back
```

### Category Selection
```
1. All
2. Business
3. Entertainment
4. Sports
5. Technology
```

### Article Actions
```
1. Back
2. Logout
3. Save Article
4. Like Article
5. Dislike Article
6. Report Article
```

### Search Features
- **Query Input:** Free-text search
- **Date Range Filtering:** Start and end date selection
- **Sorting Options:** By likes, dislikes, date
- **Results Display:** Paginated article list

### Notification Configuration
```
1. View Notifications
2. Configure Notifications
3. Back
4. Logout
```

### Notification Settings
```
1. Business - Enabled/Disabled
2. Entertainment - Enabled/Disabled
3. Sports - Enabled/Disabled
4. Technology - Enabled/Disabled
5. Keywords - Enabled/Disabled
6. Back
7. Logout
```

---

## API Integration

### Base Configuration
- **Base URL:** `https://localhost:7000/api`
- **Authentication:** JWT Bearer token
- **Content Type:** JSON

### Key API Endpoints
- **Authentication:** `/api/AuthManagement/login`, `/api/AuthManagement/register`
- **News:** `/api/News/headlines/today`, `/api/News/saved`, `/api/News/search`
- **User Actions:** `/api/News/save`, `/api/News/like`, `/api/News/dislike`, `/api/News/report`
- **Notifications:** `/api/Notifications`, `/api/Notifications/configure`
- **Admin:** `/api/Admin/servers`, `/api/Admin/categories`

---

## Models and DTOs

### Request Models
- `LoginRequest`: Email and password for authentication
- `RegisterRequest`: Username, email, and password for registration
- `SearchRequest`: Query, date range, and sorting parameters
- `ReportArticleRequest`: Article ID and reason for reporting

### Response Models
- `ApiResponse<T>`: Generic API response wrapper
- `NewsResponse`: News articles with pagination
- `TokenResponseDto`: Authentication token and user information
- `NotificationSettings`: User notification preferences

### Data Models
- `NewsArticle`: Complete article information
- `SavedArticle`: User-saved article data
- `Category`: News category information
- `UserDto`: User profile and role information

---

## Services

### ApiService
- **Purpose:** Handles all HTTP communication with the server
- **Features:** JWT token management, request/response serialization, error handling
- **Methods:** Login, register, news operations, notifications, admin functions

### ConsoleService
- **Purpose:** Console input/output operations
- **Features:** Colored output, input validation, user interaction
- **Methods:** ReadLine, WriteLine, DisplayError, PressAnyKeyToContinue

### Display Services
- **ConsoleDisplayService:** Menu and UI rendering
- **NewsDisplayService:** News article formatting and display

---

## Input Validation

### Email Validation
- Format validation using regex patterns
- Duplicate email checking during registration
- Case-insensitive email handling

### Password Validation
- Minimum length requirements
- Complexity validation (if configured)
- Secure password handling

### Date Validation
- Format validation (yyyy-mm-dd)
- Range validation (no future dates)
- Logical validation (start date ≤ end date)

### Input Sanitization
- Trim whitespace
- Escape special characters
- Prevent injection attacks

---

## Error Handling

### Network Errors
- Connection timeout handling
- Retry mechanisms for failed requests
- User-friendly error messages

### API Errors
- HTTP status code interpretation
- Error message display
- Graceful degradation

### Validation Errors
- Real-time input validation
- Clear error messages
- User guidance for corrections

### Application Errors
- Exception logging
- Graceful application shutdown
- Error recovery mechanisms

---

## Getting Started

### Prerequisites
- .NET 6.0 or later
- News Aggregation Server running on `https://localhost:7000`
- Valid API keys for external news sources (configured on server)

### Installation
1. **Clone the repository**
2. **Navigate to the client directory:**
   ```bash
   cd NewsAggregationClient
   ```
3. **Build the application:**
   ```bash
   dotnet build
   ```
4. **Run the application:**
   ```bash
   dotnet run
   ```

### Configuration
- Update the base URL in `Configuration/HttpClient.cs` if the server runs on a different port
- Ensure the server is running and accessible
- Verify external API keys are configured on the server

---

## Usage Examples

### User Registration
```
Welcome to the News Aggregator application. Please choose the options below.
1. Login
2. Sign up
3. Exit

Enter your choice: 2

Enter username: john_doe
Enter email: john@example.com
Enter password: ********

Sign up successful. Welcome!
```

### User Login and News Browsing
```
Enter your choice: 1

Enter email: john@example.com
Enter password: ********

Login successful. Welcome, john_doe (User)

Welcome to the News Application, john_doe! Date: 22-Mar-2025 Time:1:56PM
Please choose the options below
1. Headlines
2. Personalized Headlines
3. Saved Articles
4. Search
5. Notifications
6. Logout

Enter your choice: 1

Please choose the options below for Headlines
1. Today
2. Date range
3. Back

Enter your choice: 1

Please choose the options below for Headlines
1. All
2. Business
3. Entertainment
4. Sports
5. Technology

Enter your choice: 2
```

### Article Interaction
```
Article Id: 123
Tesla Unusual Options Activity - Tesla (NASDAQ: TSLA)
Deep-pocketed investors have adopted a bearish approach towards Tesla TSLA, and it's something market players shouldn't ignore. Our tracking of public options r…
source: benzinga.com
URL: https://www.benzinga.com/insights/options/25/03/44379781/tesla-unusual-options-activity
Business: business

1. Back
2. Logout
3. Save Article
4. Like Article
5. Dislike Article
6. Report Article

Enter your choice: 3

Enter article ID to save: 123
Article saved successfully!
```

### Admin Functions
```
Welcome to the News Application, admin! Date: 22-Mar-2025 Time:1:56PM
Please choose the options below
1. View the list of external servers and status
2. View the external server's details
3. Update/Edit the external server's details
4. Add new News Category
5. View reported articles
6. Hide articles
7. Hide categories
8. Manage filtered keywords
9. Trigger news aggregation
10. Fix invalid categories
11. Logout

Enter your choice: 1

List of external servers (count: 2):
1. News API - Active - last accessed: 21 Mar 2025
2. The News API - Active - last accessed: 21 Mar 2025
```

---

## Development

### Building from Source
```bash
git clone <repository-url>
cd NewsAggregationClient
dotnet restore
dotnet build
dotnet run
```

### Running Tests
```bash
dotnet test
```

### Code Structure
- Follows SOLID principles
- Dependency injection for loose coupling
- Interface-based design for testability
- Clean separation of concerns

---

## Troubleshooting

### Common Issues
1. **Connection Failed:** Ensure the server is running on the correct port
2. **Authentication Errors:** Verify credentials and server authentication
3. **API Errors:** Check server logs and API key configuration
4. **Display Issues:** Ensure console supports UTF-8 encoding

### Debug Mode
- Enable detailed logging by setting environment variables
- Use `--verbose` flag for additional output
- Check server logs for API-related issues

---

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Submit a pull request

---

## License

MIT License - see LICENSE file for details

---

## Support

For issues and questions:
- Check the troubleshooting section
- Review server logs
- Open an issue on the repository
- Contact the development team

---

**Note:** This client application is designed to work with the News Aggregation Server. Ensure the server is properly configured and running before using the client. 