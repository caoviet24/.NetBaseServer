# ASP.NET Core Clean Architecture Application

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-13-316192?logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![Redis](https://img.shields.io/badge/Redis-6.2-DC382D?logo=redis&logoColor=white)](https://redis.io/)
[![Docker](https://img.shields.io/badge/Docker-Containerized-2496ED?logo=docker&logoColor=white)](https://www.docker.com/)
[![Clean Architecture](https://img.shields.io/badge/Architecture-Clean-009485)](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

A robust ASP.NET Core application built using Clean Architecture principles with PostgreSQL as the database backend and Redis for caching and session management. The application is containerized using Docker for ease of development and deployment.

## 📑 Table of Contents

- [Architecture Overview](#-architecture-overview)
- [Prerequisites](#-prerequisites)
- [Getting Started](#-getting-started)
  - [Docker Setup](#docker-setup)
  - [Application Access](#application-access)
  - [Database Connection](#database-connection)
- [Development Workflow](#-development-workflow)
  - [Local Development](#local-development-without-docker)
  - [Entity Framework Migrations](#using-entity-framework-migrations)
- [Configuration](#-configuration)
- [Troubleshooting](#-troubleshooting)

## 🏛 Architecture Overview

The application follows Clean Architecture principles with the following layers:

| Layer              | Description                                                             |
| ------------------ | ----------------------------------------------------------------------- |
| **Domain**         | Contains enterprise business rules and entities                         |
| **Application**    | Contains business rules specific to the application                     |
| **Infrastructure** | Contains implementations of interfaces defined in the application layer |
| **WebApi**         | Contains controllers and API endpoints                                  |

Redis is used for:

- Caching query results to improve performance
- Storing temporary data and session information
- Health monitoring
- Implementing distributed caching patterns

## 🔧 Prerequisites

- [Docker](https://www.docker.com/products/docker-desktop) & [Docker Compose](https://docs.docker.com/compose/install/)
- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) (for local development)
- [Visual Studio Code](https://code.visualstudio.com/) (recommended)
- [PostgreSQL Extension](https://marketplace.visualstudio.com/items?itemName=ckolkman.vscode-postgres) for VS Code (optional)
- [Redis](https://redis.io/download) (for local development without Docker)

## 🚀 Getting Started

### Docker Setup

1. Clone this repository
2. Navigate to the project directory
3. Run the following command to build and start the application:

```bash
docker-compose up -d
```

This will start:

- PostgreSQL database on port 5432
- Redis server on port 6379
- ASP.NET Core Web API on port 8080 (HTTP) and 8443 (HTTPS)

To update the Docker Compose configuration to include Redis, add the following to your docker-compose.yml:

```yaml
redis:
  image: redis:6.2-alpine
  ports:
    - "6379:6379"
  volumes:
    - redis-data:/data
  networks:
    - app-network
  restart: unless-stopped
  command: redis-server --appendonly yes
  healthcheck:
    test: ["CMD", "redis-cli", "ping"]
    interval: 10s
    timeout: 5s
    retries: 5
```

And update the volumes section:

```yaml
volumes:
  postgres-data:
  redis-data:
```

Don't forget to add Redis as a dependency in your web application service:

```yaml
webapi:
  # existing configuration...
  depends_on:
    - postgres
    - redis
  environment:
    # existing environment variables...
    - Redis__Configuration=redis:6379
    - Redis__InstanceName=elearning:
```

### Application Access

- API: http://localhost:8080
- Swagger Documentation: http://localhost:8080/swagger
- Redis Health Check: http://localhost:8080/api/redis/health

These settings are configured in:

- `docker-compose.yml` for the containerized environment
- `.vscode/settings.json` for VS Code connection

## 💻 Development Workflow

### Local Development (without Docker)

1. Ensure PostgreSQL and Redis are installed and running locally, or connect to containerized services
2. Update the connection strings in `WebApi/appsettings.json`
3. Navigate to the WebApi project directory
4. Run the application:

```bash
cd WebApi
dotnet run
```

### Using Entity Framework Migrations

To create and apply database migrations:

```bash
# Navigate to the WebApi project
cd WebApi

# Add a new migration
dotnet ef migrations add MigrationName --project ../Infrastructure

# Apply migrations to the database
dotnet ef database update --project ../Infrastructure
```

## ⚙️ Configuration

The application configuration is stored in `WebApi/appsettings.json`. For local development, you can create `appsettings.Development.json` with environment-specific settings.

**Example configuration:**

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "WebApi": "Debug",
      "Infrastructure": "Debug",
      "Application": "Debug"
    },
    "CustomConsole": {
      "LogLevel": {
        "Default": "Debug"
      }
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=aspnetdb;Username=postgres;Password=postgres;"
  },
  "Redis": {
    "Configuration": "localhost:6379",
    "InstanceName": "elearning:"
  },
  "Jwt": {
    "AccessKey": "f71757666c33af119d16f86e20dde626d4dbeb3858e20aac52704d3bf430fdd81c739dd2f47a9d8b45690ae9bed91bb0",
    "AccessExpiresInDay": 1,
    "RefreshKey": "1c9955f76c640f8d19a0d2681c55f7c0829e88cc9b7d7a568bedf1121087aafd354f39a76dab528eccbe4a024a6b5093",
    "RefreshExpiresInDay": 365,
    "Issuer": "PenguinECommerce.Admin.Dev",
    "Audience": "PenguinECommerce.AdminUsers.Dev",
    "ExpiryInMinutes": 1440
  },
  "Cors": {
    "Origins": ["http://localhost:3000"]
  }
}
```

## ❓ Troubleshooting

### Docker Issues

If you encounter issues with Docker:

```bash
# Restart containers
docker-compose down
docker-compose up -d

# View logs
docker-compose logs -f
```

### Database Connection Issues

- Ensure the PostgreSQL container is running: `docker ps`
- Check container logs: `docker-compose logs postgres`
- Verify connection settings in appsettings.json

### Redis Connection Issues

- Ensure the Redis container is running: `docker ps`
- Check Redis logs: `docker-compose logs redis`
- Verify Redis is responding: `docker exec -it <redis-container-id> redis-cli ping`
- Check the Redis health endpoint: `curl http://localhost:8080/api/redis/health`
- Verify Redis configuration in appsettings.json
