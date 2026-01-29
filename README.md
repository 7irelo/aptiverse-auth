# 🔐 Aptiverse Auth Provider

<img width="1916" height="908" alt="Aptiverse Auth Provider" src="https://github.com/user-attachments/assets/1b8c5117-eb54-426a-bf2b-d1c0fee7c50c" />

**Aptiverse Auth Provider** is a secure, scalable authentication and authorization service built with **.NET 10**, designed to power user management, authentication, and role-based access control for the Aptiverse educational ecosystem.

> 🔧 This repository contains the **authentication and user management backend** for the Aptiverse platform, providing robust identity services with JWT, Google OAuth, and role-based authorization.

---

## 🚀 Overview

Aptiverse Auth Provider is a dedicated authentication service that handles:

* **User Registration & Management** - Complete user lifecycle management
* **Multi-factor Authentication** - JWT tokens with refresh capabilities
* **Social Authentication** - Google OAuth integration
* **Role-Based Access Control** - Admin, User, Student, and custom roles
* **Security Features** - Rate limiting, password policies, and secure token management

Unlike monolithic auth solutions, this provider is built with **enterprise-grade security** and **educational platform requirements** in mind.

---

## 🛡️ Core Features

### 🔐 Authentication
* **JWT Bearer Token** authentication with configurable expiration
* **Google OAuth** integration for social login
* **Cookie-based authentication** for web applications
* **Token refresh** mechanism for seamless user experience

### 👥 User Management
* **User registration** with email verification
* **Password management** (change, reset, forgot password)
* **Profile management** and user information retrieval
* **Role assignment** and permission management

### 🎯 Role-Based Access Control
* **Pre-defined roles**: Admin, User, Student, Parent, SuperUser
* **Custom policy-based authorization**
* **Fine-grained permission control** per endpoint
* **Hierarchical role permissions**

### 📊 Security & Performance
* **Intelligent rate limiting** with different tiers for user roles
* **Multiple rate limiting strategies** (Fixed Window, Sliding Window, Token Bucket)
* **CORS configuration** for cross-origin requests
* **Request validation** and exception handling

---

## 🛠️ Tech Stack

| Layer                       | Technology                            |
| --------------------------- | ------------------------------------- |
| **Framework**               | .NET 10, ASP.NET Core                 |
| **Authentication**          | JWT Bearer, Google OAuth, Cookies     |
| **Database**                | Entity Framework Core, PostgreSQL     |
| **Caching**                 | Redis                                 |
| **Security**                | ASP.NET Core Identity                 |
| **API Documentation**       | OpenAPI, Scalar, ReDoc               |
| **Containerization**        | Docker                               |
| **Rate Limiting**           | ASP.NET Core Rate Limiting           |

---

## 📂 Project Structure

```
Aptiverse.Auth.Web           → Main web project with controllers and middleware
Aptiverse.Application        → Application layer: DTOs, Services, Business logic
Aptiverse.Core              → Core domain models and exceptions
Aptiverse.Domain            → Domain entities and business rules
Aptiverse.Infrastructure    → Data access, Entity Framework, Redis integration
```

### Key Components

* **AuthController** - Handles authentication flows (login, register, token refresh)
* **UsersController** - User management (CRUD operations)
* **StudentsController** - Student-specific operations
* **Identity Services** - JWT, OAuth, and cookie authentication
* **Rate Limiting** - Multi-tier request limiting based on user roles

---

## 🔧 Configuration

### Authentication Setup
```json
{
  "Jwt": {
    "Key": "your-secret-key",
    "Issuer": "aptiverse",
    "Audience": "aptiverse-users"
  },
  "Authentication": {
    "Google": {
      "ClientId": "your-google-client-id",
      "ClientSecret": "your-google-client-secret"
    }
  }
}
```

### Rate Limiting Tiers
* **Premium Users** (Admin/SuperUser): 1000 requests/second
* **Authenticated Users**: 500 requests/second  
* **Anonymous Users**: 100 requests/minute

---

## 🚀 API Endpoints

### Authentication
- `POST /api/auth/register` - User registration
- `POST /api/auth/login` - User login
- `POST /api/auth/refresh-token` - Refresh JWT token
- `POST /api/auth/logout` - User logout
- `POST /api/auth/change-password` - Password change
- `POST /api/auth/forgot-password` - Password reset request
- `POST /api/auth/reset-password` - Password reset
- `GET /api/auth/me` - Get current user info

### User Management
- `GET/POST/PUT/DELETE /api/users` - User CRUD operations (Admin only)
- `GET/POST/PUT/DELETE /api/students` - Student management

---

## 🛡️ Security Features

### JWT Configuration
```csharp
options.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = configuration["Jwt:Issuer"],
    ValidAudience = configuration["Jwt:Audience"],
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
    ClockSkew = TimeSpan.Zero
};
```

### Role-Based Policies
```csharp
services.AddAuthorizationBuilder()
    .AddPolicy("RequireAuthenticatedUser", policy => policy.RequireAuthenticatedUser())
    .AddPolicy("Admin", policy => policy.RequireRole("Admin"))
    .AddPolicy("User", policy => policy.RequireRole("User", "Admin"));
```

---

## 📚 Integration Guide

### Frontend Integration
```javascript
// Login example
const response = await fetch('/api/auth/login', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email, password })
});

const { token, refreshToken } = await response.json();
localStorage.setItem('token', token);
```

### API Client Usage
```csharp
// Include JWT token in requests
client.DefaultRequestHeaders.Authorization = 
    new AuthenticationHeaderValue("Bearer", jwtToken);
```

---

## 🔄 Development

### Running with Docker
```bash
docker build -t aptiverse-auth .
docker run -p 8080:8080 aptiverse-auth
```

### Local Development
```bash
dotnet restore
dotnet run
```

---

## 📊 Monitoring & Documentation

* **OpenAPI Documentation**: `/openapi/v1.json`
* **Scalar API Reference**: `/dev`
* **ReDoc Documentation**: `/docs`
* **Health Checks**: Built-in ASP.NET Core health monitoring

---

## 🔐 Best Practices Implemented

- ✅ **Secure Password Hashing** with ASP.NET Core Identity
- ✅ **JWT Best Practices** with short-lived tokens and refresh mechanism
- ✅ **CORS Configuration** for controlled cross-origin access
- ✅ **Rate Limiting** to prevent abuse and DDoS attacks
- ✅ **Input Validation** with custom filters and model validation
- ✅ **Exception Handling** with structured error responses
- ✅ **Role-Based Access Control** with policy-based authorization
- ✅ **Secure Cookie Configuration** for web authentication

---

## 🚀 Deployment

The Auth Provider is container-ready and can be deployed to:
- **Docker Containers**
- **Kubernetes** clusters
- **Cloud Platforms** (AWS, Azure, GCP)
- **Traditional IIS** hosting

---

## 🤝 Contributing

We welcome contributions to improve the Aptiverse Auth Provider! Please see our contributing guidelines for more information.

---

## 📄 License

> This project is part of the Aptiverse educational ecosystem and is currently in active development.

---

### 🎯 Aptiverse Auth: Secure Identity for Education

Built with ❤️ for the Aptiverse educational platform, providing robust and scalable authentication services to empower learners and educators worldwide.
