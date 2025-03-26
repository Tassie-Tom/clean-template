# Clean Architecture .NET API

A modern .NET 8 API built following Clean Architecture principles, CQRS pattern, and domain-driven design.

## Architecture Overview

This project implements clean architecture with clear separation of concerns across multiple layers:

```
┌───────────────────┐
│     API Layer     │  →  Controllers, Middleware, API Endpoints
├───────────────────┤
│ Application Layer │  →  Use Cases, Commands/Queries, Validation
├───────────────────┤
│   Domain Layer    │  →  Entities, Value Objects, Domain Events
├───────────────────┤
│Infrastructure Layer│  →  Data Access, External Services, Auth
└───────────────────┘
```

### Project Structure

- **Api.Domain**: Core domain entities, value objects, and domain services
- **Api.Application**: Application use cases, CQRS implementation, and behaviors
- **Api.Infrastructure**: Data access, caching, authentication, and external services
- **Api.Web**: API controllers, middleware, and configuration
- **Api.SharedKernel**: Shared abstractions, result pattern, and common utilities

## Key Features

- **Clean Architecture**: Clear separation of concerns with domain-centric approach
- **CQRS Pattern**: Command and Query Responsibility Segregation for better scalability
- **Domain-Driven Design**: Focus on core business logic with rich domain model
- **Result Pattern**: Explicit error handling without exceptions
- **Firebase Authentication**: Secure API with Firebase integration
- **Database Flexibility**: Support for PostgreSQL, SQL Server, and MySQL
- **Caching**: Redis implementation for query caching
- **Swagger Documentation**: API documentation with Swagger/OpenAPI
- **Outbox Pattern**: Reliable domain event publishing
- **Health Checks**: Built-in monitoring for database and services
- **Docker Support**: Containerization for deployment
- **Structured Logging**: Using Serilog with Seq integration

## Technical Stack

- **.NET 8.0**: Latest .NET framework
- **PostgreSQL/SQL Server/MySQL**: Database options
- **Entity Framework Core 8**: ORM for data persistence
- **Dapper**: Micro-ORM for optimized query performance
- **Redis**: Distributed caching
- **MediatR**: CQRS and mediator pattern implementation
- **FluentValidation**: Request validation
- **Firebase Admin SDK**: Authentication and user management
- **Serilog**: Structured logging with Seq integration
- **Swashbuckle/Swagger**: API documentation
- **HealthChecks**: Infrastructure health monitoring
- **Docker/Docker Compose**: Containerization and orchestration

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- Firebase project (for authentication)

### Configuration

1. Create a `.env` file in the root directory with your environment variables:

```
# Database
ConnectionStrings__Database=Host=localhost;Port=5432;Database=ApiDb;Username=postgres;Password=postgres;

# Redis Cache
ConnectionStrings__Cache=localhost:6379,abortConnect=false,ssl=false

# Firebase (choose one method)
FIREBASE_CREDENTIALS={"your-firebase-credentials-json"}
# or
FIREBASE_PROJECT_ID=your-project-id
```

2. Firebase Configuration:
   - Set up a Firebase project and download the service account credentials
   - Configure Firebase auth settings in `appsettings.json` or environment variables

### Running the Application

#### Using Docker Compose

```bash
# Start all services
docker-compose up

# Build and start
docker-compose up --build
```

This starts:
- API application
- PostgreSQL database
- Redis cache
- Seq log server

#### Local Development

```bash
# Start dependencies
docker-compose up postgres redis seq

# Run the API
cd Api.Web
dotnet run
```

The API will be available at:
- HTTP: http://localhost:8080
- HTTPS: https://localhost:8081
- Swagger UI: https://localhost:8080/swagger
- Seq Dashboard: http://localhost:8880

## Application Flow

1. HTTP request arrives at API controller
2. Request is mapped to a Command/Query
3. Pipeline behaviors process the request:
   - Logging
   - Validation
   - Caching (for queries)
4. Handler processes the command/query
5. Domain operations are performed with repository abstractions
6. Results are returned through the Result pattern
7. API controller maps the result to HTTP response

## API Endpoints

### User Management

- **Create User**: `POST /auth/createuser`
  - Creates a new user after Firebase authentication

## Authentication

This API uses Firebase Authentication:

1. Client authenticates with Firebase to obtain a token
2. Token is sent in Authorization header with requests
3. API validates the token and establishes user context
4. User operations are authorized based on identity

## Domain Model

### Core Entities

- **User**: Represents a system user with identity and profile data
  - Properties: Id, Email, Name, FirebaseId
  - Value Objects: Email, Name

## Error Handling

The API implements structured error handling with:

- **Result Pattern**: All operations return a Result object indicating success/failure
- **Error Types**: Classification of errors (Validation, NotFound, Conflict, etc.)
- **Problem Details**: RFC 7807 compliant error responses

## Development

### Database Migrations

```bash
# Add a new migration
dotnet ef migrations add MigrationName --project Api.Infrastructure --startup-project Api.Web

# Apply migrations
dotnet ef database update --project Api.Infrastructure --startup-project Api.Web
```

### Adding a New Feature

1. Define domain entities/value objects in `Api.Domain`
2. Create commands/queries in `Api.Application`
3. Implement handlers with business logic
4. Add any needed repositories in `Api.Infrastructure`
5. Create API endpoints in `Api.Web`

## Deployment

### Docker Deployment

The application is containerized and can be deployed using the provided Dockerfile:

```bash
# Build the Docker image
docker build -t clean-architecture-api -f Api.Web/Dockerfile .

# Run the container
docker run -p 8080:8080 clean-architecture-api
```

### Environment Variables

Key environment variables for deployment:

- `ASPNETCORE_ENVIRONMENT`: Runtime environment (Development, Staging, Production)
- `ConnectionStrings__Database`: Database connection string
- `ConnectionStrings__Cache`: Redis connection string
- `FIREBASE_CREDENTIALS`: Firebase service account JSON
- `FIREBASE_PROJECT_ID`: Firebase project identifier

## Architectural Decisions

### CQRS Implementation

The project uses MediatR to implement CQRS, separating read and write operations:

- **Commands**: State-changing operations (Create, Update, Delete)
- **Queries**: Read-only operations optimized for performance
- **Events**: Domain events for cross-cutting concerns

### Pipeline Behaviors

MediatR pipeline behaviors provide cross-cutting concerns:

- **Validation**: Validates incoming requests using FluentValidation
- **Logging**: Logs request/response details
- **Caching**: Caches query results with Redis
- **Transaction**: Manages database transactions

### Repository Pattern

Data access is abstracted through repositories, allowing:

- Business logic to focus on domain operations
- Simplified testing with mock repositories
- Flexibility to change database providers

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/your-feature`)
3. Commit your changes (`git commit -am 'Add new feature'`)
4. Push to the branch (`git push origin feature/your-feature`)
5. Create a new Pull Request

## License

This project is licensed under the MIT License.
