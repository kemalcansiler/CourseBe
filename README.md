# Course Learning Platform - Backend API

A comprehensive .NET 8 Web API for an online course learning platform, built with Clean Architecture principles.

## 🚀 Tech Stack

- **Framework**: .NET 8.0
- **Architecture**: Clean Architecture (DDD)
- **Database**: PostgreSQL 17
- **ORM**: Entity Framework Core 8.0
- **Authentication**: ASP.NET Identity + JWT
- **API Versioning**: ASP.NET API Versioning
- **Logging**: Serilog
- **Mapping**: AutoMapper
- **Containerization**: Docker & Docker Compose
- **Design Patterns**: Repository Pattern, Unit of Work, Specification Pattern

## 📋 Features

- ✅ JWT-based Authentication & Authorization
- ✅ User Profile Management
- ✅ Course Management with Categories
- ✅ Advanced Filtering & Sorting
- ✅ Pagination Support
- ✅ Swagger API Documentation
- ✅ Database Seeding with 200+ Sample Courses
- ✅ CORS Support for Angular Frontend
- ✅ Structured Logging
- ✅ Clean Architecture (Core, Application, Infrastructure, API)

## 🏗️ Clean Architecture Structure

```
CourseBe/
├── Course.Core/                    # Domain Layer
│   ├── Entities/                   # Domain entities (Course, User, Category, etc.)
│   ├── Interfaces/                 # Repository and service interfaces
│   └── Specifications/             # Query specifications
│
├── Course.Application/             # Application Layer
│   ├── Services/                   # Business logic implementations
│   │   ├── Implementations/        # Service implementations
│   │   └── Interfaces/             # Service contracts
│   ├── DTOs/                       # Data Transfer Objects
│   └── Mappings/                   # AutoMapper profiles
│
├── Course.Infrastructure/          # Infrastructure Layer
│   ├── Data/                       # Database context and seeding
│   │   ├── ApplicationDbContext.cs
│   │   └── SeedData.cs
│   ├── Repositories/               # Repository implementations
│   └── Migrations/                 # EF Core migrations
│
├── Course.Api/                     # Presentation Layer
│   ├── Controllers/                # API endpoints
│   │   ├── AuthController.cs      # Authentication
│   │   ├── CoursesController.cs   # Course management
│   │   └── ProfileController.cs   # User profile
│   ├── Program.cs                  # App configuration
│   └── Dockerfile                  # API container config
│
├── docker-compose.yml              # Multi-container orchestration
└── CourseBe.sln                    # Solution file
```

### Architecture Principles:
- **Dependency Inversion**: Inner layers don't depend on outer layers
- **Separation of Concerns**: Each layer has distinct responsibilities
- **Testability**: Business logic isolated from infrastructure
- **Maintainability**: Clear boundaries between layers

## 🔧 Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- PostgreSQL 17 (if running locally without Docker)

## 🚀 Getting Started

### Option 1: Docker (Recommended)

1. **Clone the repository**
   ```bash
   git clone https://github.com/kemalcansiler/CourseBe.git
   cd CourseBe
   ```

2. **Start the application**
   ```bash
   docker-compose up --build -d
   ```

3. **Access the API**
   - API: http://localhost:5002
   - Swagger UI: http://localhost:5002 (root endpoint)
   - PostgreSQL: localhost:5432

The database will be **automatically seeded** with 200 sample courses on first run in development mode!

### Docker Services:
- **course_api**: .NET 8 Web API (Port 5002)
- **course_postgres**: PostgreSQL 17 (Port 5432)
- **Network**: course_network (bridge)
- **Volume**: postgres_data (persistent database storage)

### Option 2: Local Development

1. **Clone the repository**
   ```bash
   git clone https://github.com/kemalcansiler/CourseBe.git
   cd CourseBe
   ```

2. **Update connection string**
   Edit `Course.Api/appsettings.Development.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5432;Database=course_db;Username=your_user;Password=your_password"
     }
   }
   ```

3. **Run database migrations**
   ```bash
   cd Course.Infrastructure
   dotnet ef database update --startup-project ../Course.Api
   ```

4. **Start the API**
   ```bash
   cd Course.Api
   dotnet run
   ```

5. **Access the API**
   - API: http://localhost:5005
   - Swagger UI: http://localhost:5005

## 📊 Database Seeding

The application automatically seeds the database with sample data in **development mode**.

### Seed Data Includes:
- 200 diverse courses across 5 categories
- Categories: Web Development, Programming, Mobile Development, Data Science, Business
- Realistic course data with ratings, prices, durations, and descriptions

### Manual Seeding:
```bash
dotnet run --seed
```

## 🔐 Authentication

### Register a new user:
```bash
POST /api/v1/auth/register
{
  "email": "user@example.com",
  "password": "YourPassword123!",
  "firstName": "John",
  "lastName": "Doe"
}
```

### Login:
```bash
POST /api/v1/auth/login
{
  "email": "user@example.com",
  "password": "YourPassword123!"
}
```

Returns a JWT token to use in subsequent requests.

## 📚 API Endpoints

### Courses
- `GET /api/v1/courses` - Get paginated courses with filtering
- `GET /api/v1/courses/{id}` - Get course by ID
- `GET /api/v1/courses/featured` - Get featured courses
- `GET /api/v1/courses/filters` - Get available filters
- `GET /api/v1/courses/categories` - Get all categories

### Query Parameters:
- `page` - Page number (0-indexed)
- `pageSize` - Items per page (default: 10)
- `sortBy` - Sort option (most-popular, highest-rated, newest, price-low-to-high, price-high-to-low)
- `category` - Filter by category IDs (multiple)
- `ratings` - Filter by minimum rating (multiple)
- `duration` - Filter by duration range (short, medium, long, extra-long)
- `levelFilter` - Filter by level (BEGINNER, INTERMEDIATE, ADVANCED, ALL_LEVELS)

### Authentication
- `POST /api/v1/auth/register` - Register new user
- `POST /api/v1/auth/login` - Login user
- `GET /api/v1/auth/me` - Get current user

### Profile (Requires Authentication)
- `GET /api/v1/profile` - Get user profile
- `PUT /api/v1/profile` - Update user profile

## 🔍 Example Requests

### Get Courses with Filters:
```bash
GET /api/v1/courses?page=0&pageSize=12&sortBy=highest-rated&category=1&category=2&ratings=4.0
```

### Get Course Details:
```bash
GET /api/v1/courses/1
```

## 🐳 Docker Commands

```bash
# Start services
docker-compose up -d

# Rebuild and start
docker-compose up --build -d

# Stop services
docker-compose down

# Stop and remove volumes (reset database)
docker-compose down -v

# View logs
docker logs course_api
docker logs course_postgres
```

## 🔧 Configuration

### Environment Variables (docker-compose.yml):
```yaml
- ASPNETCORE_ENVIRONMENT=Development
- ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=course_db;Username=course_user;Password=course_password
- Jwt__Key=YourSuperSecretKeyThatIsAtLeast32CharactersLong!
- Jwt__Issuer=CourseApi
- Jwt__Audience=CourseApiUsers
```

### JWT Configuration (appsettings.json):
```json
{
  "Jwt": {
    "Key": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "CourseApi",
    "Audience": "CourseApiUsers",
    "ExpirationInMinutes": 60
  }
}
```

## 📝 Logging

Logs are written to:
- Console output
- File: `logs/log-YYYYMMDD.txt` (rolling daily)

## 🧪 API Documentation

Swagger UI is available at the root endpoint:
- Development: http://localhost:5002
- Local: http://localhost:5005

## 🛠️ Development

### Add Migration:
```bash
cd Course.Infrastructure
dotnet ef migrations add MigrationName --startup-project ../Course.Api
```

### Update Database:
```bash
dotnet ef database update --startup-project ../Course.Api
```

### Remove Last Migration:
```bash
dotnet ef migrations remove --startup-project ../Course.Api
```

## 🤝 Frontend Integration

This backend is designed to work with an Angular frontend. CORS is configured to allow requests from:
- http://localhost:4200
- https://localhost:4200

## 📦 NuGet Packages

Key dependencies:
- Microsoft.EntityFrameworkCore (8.0.8)
- Npgsql.EntityFrameworkCore.PostgreSQL (8.0.4)
- Microsoft.AspNetCore.Identity.EntityFrameworkCore (8.0.8)
- AutoMapper (12.0.1)
- Serilog.AspNetCore
- Swashbuckle.AspNetCore

## 🔒 Security Notes

- Change JWT secret key in production
- Use environment variables for sensitive data
- Enable HTTPS in production
- Update CORS policy for production domains
- Use strong passwords for database credentials

## 📄 License

This project is licensed under the MIT License.

## 👤 Author

**Kemal Can Siler**
- GitHub: [@kemalcansiler](https://github.com/kemalcansiler)

## 🙏 Acknowledgments

- Built with ASP.NET Core
- Uses Clean Architecture principles
- Sample course data inspired by popular online learning platforms

