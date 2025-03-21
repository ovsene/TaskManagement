# Task Management API

A .NET 8 Web API project implementing Clean Architecture with CQRS pattern.

## Prerequisites

- .NET 8 SDK
- Docker (optional, for containerized deployment)

## Project Structure

- **TaskManagement.API**: Web API layer
- **TaskManagement.Application**: Application layer with CQRS commands and queries
- **TaskManagement.Domain**: Domain layer with entities and interfaces
- **TaskManagement.Infrastructure**: Infrastructure layer with data access and external services

## Setup Instructions

### Local Development

1. Clone the repository
2. Navigate to the API project directory:
   ```bash
   cd TaskManagement.API
   ```
3. Run the application:
   ```bash
   dotnet run
   ```
4. The API will be available at:
   - HTTPS: https://localhost:44396
   - HTTP: http://localhost:5000

### Docker Deployment

1. Build the Docker image:
   ```bash
   docker build -t taskmanagement-api .
   ```

2. Run the container:
   ```bash
   docker run -d -p 80:80 -p 443:443 taskmanagement-api
   ```

## API Documentation

1. Swagger UI is available at: https://localhost:44396/swagger
2. Postman collection is available in the `TaskManagement.postman_collection.json` file

## Features

- Clean Architecture implementation
- CQRS pattern with MediatR
- JWT Authentication
- In-memory database
- Fluent Validation
- Swagger documentation
- Docker support

## Testing the API

1. Import the Postman collection from `TaskManagement.postman_collection.json`
2. Set up environment variables in Postman:
   - `baseUrl`: https://localhost:44396
   - `token`: Your JWT token after login
   - `taskId`: A valid task ID
   - `userId`: A valid user ID
   - `departmentId`: A valid department ID

3. Start with the login request to get a JWT token
4. Use the token in subsequent requests 