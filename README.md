# Clean Architecture API

A modern .NET 8 API built following Clean Architecture principles and domain-driven design.

## Architecture Overview

This project demonstrates a clean architecture implementation with clear separation of concerns:

![Clean Architecture Layers](https://miro.medium.com/max/1400/1*vG3-JBd56z3GT9YIQ-cR0A.png)

### Layers

- **Domain Layer**: Core business entities, value objects, and rules
- **Application Layer**: Use cases, CQRS implementation with MediatR
- **Infrastructure Layer**: External concerns like persistence, caching, and external services
- **Web Layer**: API endpoints, controllers, and middleware

## Key Features

- **CQRS Pattern**: Commands and Queries are separated for better scaling and performance
- **Result Pattern**: Explicit error handling without exceptions
- **Repository Pattern**: Abstract data access implementations
- **Domain Events**: For decoupling domain behaviors
- **Validation Pipeline**: Using FluentValidation for robust input validation
- **Caching**: Query caching with Redis
- **Docker Support**: Containerization for easy deployment

## Technical Stack

- **.NET 8.0**: Latest .NET version
- **PostgreSQL**: Primary database
- **Redis**: Distributed caching
- **Entity Framework Core 8**: ORM for data persistence
- **Dapper**: For optimized read queries
- **MediatR**: For mediator pattern implementation
- **FluentValidation**: For request validation
- **Serilog**: Structured logging
- **Swashbuckle/Swagger**: API documentation

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- PostgreSQL and Redis (included in docker-compose)

### Running the Application

#### Using Docker Compose

```bash
docker-compose up
```

This will start:
- The API web application
- PostgreSQL database
- Redis cache

#### Using .NET CLI

```bash
# Run the database and Redis using Docker
docker-compose up postgres redis

# Run the API
cd Api.Web
dotnet run
```

The API will be available at:
- HTTP: http://localhost:5121
- HTTPS: https://localhost:7099
- Swagger UI: https://localhost:7099/swagger

## Project Structure

- **Api.Domain**: Core domain entities and business rules
- **Api.Application**: Application use cases and business logic
- **Api.Infrastructure**: External concerns implementation
- **Api.Web**: API controllers and endpoints
- **Api.SharedKernal**: Common utilities and base components

## Application Flow

1. HTTP request comes into the API layer
2. Request is mapped to a Command/Query
3. Mediator dispatches Command/Query to the appropriate handler
4. Pipeline behaviors handle cross-cutting concerns
   - Logging
   - Validation
   - Caching (for queries)
5. Handler interacts with domain entities via repositories
6. Infrastructure layer handles persistence
7. Result is returned and mapped to HTTP response

## API Endpoints

### User Management

- Create User: `POST /api/users`
- Get User by ID: `GET /api/users/{id}`
- Get User by Email: `GET /api/users/email/{email}`

## Error Handling

The API uses Problem Details (RFC 7807) for standardized error responses. Error types include:

- Validation errors (400)
- Not found errors (404)
- Conflict errors (409)
- Server errors (500)

## Development

### Database Migrations

```bash
# Add a new migration
dotnet ef migrations add MigrationName --project Api.Infrastructure --startup-project Api.Web

# Apply migrations
dotnet ef database update --project Api.Infrastructure --startup-project Api.Web
```

### Running Tests

```bash
dotnet test
```

## Docker Support

The application can be containerized using the provided Dockerfile and docker-compose files:

```bash
# Build and run with Docker Compose
docker-compose up --build
```

## Contributing

Please follow these guidelines when contributing:

1. Use the .editorconfig for consistent formatting
2. Follow the existing architecture patterns
3. Write unit tests for new features

## License

This project is licensed under the MIT License.
