# News Aggregation System

A comprehensive news aggregation platform consisting of a Server and Client application that collects, processes, and delivers news content from multiple external sources.

## 🏗️ Architecture

### Server Application (.NET 8 Web API)
- **Framework**: ASP.NET Core 8.0
- **Database**: SQL Server with Entity Framework Core
- **Authentication**: JWT Bearer Tokens
- **Background Services**: Periodic news fetching (every 3 hours)
- **External APIs**: NewsAPI, TheNewsAPI, Firebase API
- **Email Service**: SMTP-based email notifications

### Client Application (.NET 8 Console)
- **Framework**: .NET 8 Console Application
- **UI**: Console-based interface with color-coded menus
- **Authentication**: Role-based (Admin/User)
- **Features**: News browsing, article saving, search, notifications

## 🚀 Features

### ✅ Implemented Features

#### Server Features
- [x] **Periodic News Fetching**: Background service runs every 3 hours
- [x] **RESTful APIs**: Complete CRUD operations for all entities
- [x] **Database Management**: SQL Server with Entity Framework
- [x] **User Authentication**: JWT-based authentication system
- [x] **Email Notifications**: SMTP email service
- [x] **External API Integration**: Multiple news sources
- [x] **Automatic Category Classification**: AI-based article categorization
- [x] **API Documentation**: Swagger/OpenAPI documentation

#### Client Features
- [x] **User Authentication**: Login/Register with validation
- [x] **Role-Based Access**: Admin and User roles
- [x] **News Browsing**: Headlines by date and category
- [x] **Article Management**: Save/delete articles
- [x] **Search Functionality**: Text search with filtering
- [x] **Notification System**: Configure and receive notifications
- [x] **Email Notifications**: Test email functionality

#### Admin Features
- [x] **External Server Management**: View, update server details
- [x] **Category Management**: Add new news categories
- [x] **Content Moderation**: Hide articles/categories
- [x] **Reporting System**: View reported articles
- [x] **Filter Management**: Manage filtered keywords

#### User Features
- [x] **Personalized Content**: Based on reading history
- [x] **Article Interactions**: Like/dislike articles
- [x] **Reading Progress**: Track read articles
- [x] **Keyword Alerts**: Configure keywords for notifications
- [x] **Notification Settings**: Granular control over notifications

## 🛠️ Technical Requirements Met

### ✅ Core Requirements
- [x] **REST APIs**: All endpoints follow REST standards
- [x] **RESTful Communication**: GET, POST, PUT, DELETE methods
- [x] **Relational Database**: SQL Server with proper relationships
- [x] **Multi-threading**: Background services with async/await
- [x] **SOLID Principles**: Interfaces, dependency injection, separation of concerns
- [x] **Layered Architecture**: Controllers → Services → Repositories → Models
- [x] **Clean Code Principles**: Meaningful names, single responsibility, DRY
- [x] **Unit Tests**: Comprehensive test coverage with xUnit
- [x] **API Documentation**: Swagger UI with JWT authentication

## 📁 Project Structure

```
NewsAggregationClient/
├── NewsAggregation.Server/           # Backend API
│   ├── Controller/                   # API Controllers
│   ├── Services/                     # Business Logic
│   ├── Repository/                   # Data Access
│   ├── Models/                       # Data Models
│   ├── BackgroundServices/           # Background Jobs
│   ├── Utilities/                    # Helper Classes
│   └── Configuration/                # App Settings
├── NewsAggregationClient/            # Frontend Console App
│   ├── Services/                     # API Client Services
│   ├── UI/                          # User Interface
│   ├── Models/                       # Client Models
│   └── Configuration/                # Client Settings
└── NewsAggregation.Server.Tests/     # Unit & Integration Tests
```

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code

### Configuration

1. **Database Connection**
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=your-server;Database=NewsAggregationDB;Trusted_Connection=true;"
   }
   ```

2. **Email Settings**
   ```json
   "EmailSettings": {
     "SmtpServer": "smtp.gmail.com",
     "SmtpPort": 587,
     "Username": "your-email@gmail.com",
     "Password": "your-app-password",
     "FromEmail": "your-email@gmail.com",
     "FromName": "News Aggregation System"
   }
   ```

3. **External API Keys**
   ```json
   "NewsApiSettings": {
     "NewsApiKey": "your-news-api-key",
     "TheNewsApiKey": "your-thenews-api-key",
     "FirebaseApiKey": "your-firebase-api-key"
   }
   ```

### Running the Application

1. **Start the Server**
   ```bash
   cd NewsAggregation.Server
   dotnet run
   ```

2. **Start the Client**
   ```bash
   cd NewsAggregationClient
   dotnet run
   ```

3. **Access API Documentation**
   - Swagger UI: `https://localhost:7000/swagger`

## 🧪 Testing

### Running Tests
```bash
cd NewsAggregation.Server.Tests
dotnet test
```

### Test Coverage
- **Unit Tests**: Service layer, utilities, models
- **Integration Tests**: API controllers, database operations
- **Test Frameworks**: xUnit, Moq, FluentAssertions

## 🔧 API Endpoints

### Authentication
- `POST /api/AuthManagement/login` - User login
- `POST /api/AuthManagement/register` - User registration

### News
- `GET /api/News/headlines/today` - Today's headlines
- `GET /api/News/headlines/daterange` - Headlines by date range
- `POST /api/News/search` - Search articles
- `GET /api/News/saved` - Get saved articles
- `POST /api/News/saved` - Save article
- `DELETE /api/News/saved` - Delete saved article

### Admin
- `GET /api/Admin/servers` - List external servers
- `PUT /api/Admin/servers/{id}` - Update server
- `POST /api/Admin/categories` - Add category
- `GET /api/Admin/reported-articles` - View reported articles

### Notifications
- `GET /api/Notification/settings` - Get notification settings
- `PUT /api/Notification/settings` - Update settings
- `POST /api/Notification/test-email` - Send test email

## 🎯 Key Features Explained

### Automatic Category Classification
The system automatically categorizes news articles without categories using keyword-based classification:
- Analyzes article title and content
- Matches against predefined category keywords
- Assigns the most appropriate category
- Falls back to "general" if no match found

### Background News Aggregation
- Runs every 3 hours automatically
- Fetches from multiple external sources
- Processes and categorizes articles
- Stores in database for client access

### Email Notifications
- Configurable notification settings
- Keyword-based alerts
- Test email functionality
- SMTP integration

## 🔒 Security Features

- JWT-based authentication
- Role-based authorization
- Input validation
- SQL injection prevention
- CORS configuration

## 📊 Database Design

### Key Entities
- **Users**: Authentication and profile data
- **NewsArticles**: News content from external sources
- **Categories**: News categories
- **SavedArticles**: User-saved articles
- **Notifications**: User notifications
- **UserNotificationSettings**: Notification preferences
- **ExternalServers**: External API configurations

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Submit a pull request

## 📝 License

This project is licensed under the MIT License.

## 🆘 Support

For support and questions, please create an issue in the repository. 