# Task Management API

A simple task management application built with .NET 8 using Clean Architecture principles.

## Features

- User authentication with email
- Task management (create, read, update, delete)
- Department-based task assignment
- Task status tracking (Created, Assigned, InProgress, Completed, Rejected)
- Task filtering by user and department

## Prerequisites

- .NET 8 SDK
- Docker (optional, for containerized deployment)

## Getting Started

### Local Development

1. Clone the repository
2. Navigate to the solution directory
3. Run the following commands:
   ```bash
   dotnet restore
   dotnet build
   cd TaskManagement.API
   dotnet run
   ```

The API will be available at `https://localhost:7001` and `http://localhost:5001`

### Docker Deployment

1. Build the Docker image:
   ```bash
   docker build -t task-management-api .
   ```

2. Run the container:
   ```bash
   docker run -d -p 80:80 task-management-api
   ```

The API will be available at `http://localhost:80`

## API Endpoints

### Users

- `POST /api/users/login` - Login with email

### Tasks

- `GET /api/tasks` - Get all tasks
- `GET /api/tasks/user/{userId}` - Get tasks created by a specific user
- `GET /api/tasks/department/{departmentId}` - Get tasks assigned to a department
- `GET /api/tasks/{id}` - Get task details
- `POST /api/tasks` - Create a new task
- `PUT /api/tasks/{id}` - Update a task
- `DELETE /api/tasks/{id}?userId={userId}` - Delete a task
- `POST /api/tasks/{id}/complete?userId={userId}` - Complete a task
- `POST /api/tasks/{id}/reject?userId={userId}` - Reject a task

## Architecture

The application follows Clean Architecture principles with the following layers:

- **Domain**: Contains business entities and interfaces
- **Application**: Contains business logic, commands, and queries
- **Infrastructure**: Contains data access and external service implementations
- **API**: Contains controllers and API-specific configurations

## Technologies Used

- .NET 8
- Entity Framework Core
- MediatR (CQRS)
- FluentValidation
- AutoMapper
- Swagger/OpenAPI 