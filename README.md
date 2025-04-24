# Identity Service

A robust identity management service built with .NET Core that provides authentication, authorization, and user management capabilities.

## Technologies Used

- .NET 8.0
- Entity Framework Core
- ASP.NET Core Identity
- JWT Authentication
- Redis
- SQL Server
- MassTransit
- gRPC

## Prerequisites

- .NET 8.0 SDK
- SQL Server
- Redis Server
- Visual Studio 2022 or VS Code

## Configuration

1. Configure your database connection string in `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Your_SQL_Connection_String",
  "Redis": "Your_Redis_Connection_String"
}
```

2. Configure JWT settings:

```json
"Jwt": {
  "Key": "Your_Secret_Key",
  "Issuer": "Your_Issuer",
  "Audience": "Your_Audience"
}
```

## Setup Instructions

1. Clone the repository
2. Configure the connection strings in `appsettings.json`
3. Run database migrations:

```bash
dotnet ef database update
```

4. Start the application:

```bash
dotnet run
```

## Key Features

- User Authentication & Authorization
- JWT Token Management
- Role-based Access Control
- Permission Management
- OTP Service
- Redis Caching
- gRPC Integration
- MassTransit Message Queue Integration

## API Endpoints

### Authentication

- POST `/api/auth/login` - User login
- POST `/api/auth/logout` - User logout
- POST `/api/auth/auth-check` - Verify authentication status

### User Management

- GET `/api/users` - Get users
- POST `/api/users` - Create user
- PUT `/api/users/{id}` - Update user
- DELETE `/api/users/{id}` - Delete user

### Role Management

- GET `/api/roles` - Get roles
- POST `/api/roles` - Create role
- PUT `/api/roles/{id}` - Update role
- DELETE `/api/roles/{id}` - Delete role

## Security Features

- JWT Token Authentication
- HTTP-only Cookies
- Password Hashing
- Role-based Authorization
- Permission-based Access Control

## Contributing

Please read our contributing guidelines before submitting pull requests.

## License

This project is licensed under the MIT License.
