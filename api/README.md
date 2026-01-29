# 🎓 Aptiverse API - AI-Powered Student Success Platform

<img width="1916" height="908" alt="Aptiverse API" src="https://github.com/user-attachments/assets/1b8c5117-eb54-426a-bf2b-d1c0fee7c50c" />

**Aptiverse API** is the core backend service for an innovative AI-powered educational platform designed to help South African Grade 12 students achieve academic success through personalized learning, mental health support, and comprehensive university readiness planning.

> 🚀 Transforming education from toxic competition to empowering growth through AI-driven personalized learning and holistic student support.

---

## 🌟 Platform Overview

Aptiverse is a comprehensive educational ecosystem that moves beyond traditional learning apps by combining:

- **🤖 AI-Powered Academic Support** - Personalized goal setting and practice tests
- **💭 Mental Health Integration** - Psychologist access and well-being tracking
- **🎯 Future Planning** - University readiness and bursary guidance
- **👥 Community Learning** - Tutor connections and peer collaboration
- **📊 Analytics & Insights** - Progress tracking for students, parents, and teachers

### Target Audience
- **Grade 11 & 12 South African Students** preparing for NSC/IEB examinations
- **Teachers & School Administrators** managing classroom activities
- **Parents & Guardians** supporting their children's educational journey
- **Tutors & Educational Psychologists** providing specialized support

---

## 🏗️ Architecture & Technology Stack

### Microservices Architecture
```
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│   Auth Service  │    │  Email Service   │    │  Main API       │
│    (.NET 10)    │◄──►│     (Go)         │◄──►│   (.NET 10)     │
└─────────────────┘    └──────────────────┘    └─────────────────┘
         │                       │                       │
         ▼                       ▼                       ▼
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│   PostgreSQL    │    │   RabbitMQ       │    │     Redis       │
│   (Identity)    │    │   (Messaging)    │    │    (Cache)      │
└─────────────────┘    └──────────────────┘    └─────────────────┘
```

### Core Technology Stack

| Component | Technology |
|-----------|------------|
| **Framework** | .NET 10, ASP.NET Core |
| **Database** | PostgreSQL with Entity Framework Core |
| **Caching** | Redis for session management and performance |
| **Messaging** | RabbitMQ for async communication |
| **Authentication** | JWT Tokens with separate Auth Provider |
| **API Documentation** | Swagger/OpenAPI |
| **Containerization** | Docker |

---

## 📁 Project Structure

```
src/
├── Aptiverse.Api/                          # Main API Gateway
│   ├── Controllers/                       # API Endpoints
│   ├── Application/                       # Application Layer (DTOs, Services)
│   ├── Core/                             # Core Domain Models
│   ├── Domain/                           # Domain Entities & Business Rules
│   ├── Infrastructure/                   # Data Access & External Services
│   └── Utilities/                        # Helpers & Extensions
├── Aptiverse.Auth.Provider/              # Authentication Service
└── email-service/                        # Go-based Email Service
```

### Key Microservices

1. **Main API Service** (`Aptiverse.Api`) - Core business logic and API gateway
2. **Auth Provider** - Dedicated authentication and user management
3. **Email Service** (Go) - Async email processing via RabbitMQ

---

## 🚀 Core Features

### 🎯 Academic Features
- **AI-Powered Goal Setting** - Smart academic targets based on historical performance
- **Personalized Practice Tests** - AI-generated assessments aligned with SBAs
- **Progress Analytics** - Strength tracking and subject proficiency predictions
- **Past Paper Integration** - NSC/IEB question bank with AI recommendations

### 💡 Learning Support
- **Collaborative Study Groups** - Peer-to-peer learning environments
- **"Explain It To Me"** - Concept reinforcement through teaching
- **Knowledge Gap Analysis** - AI-identified learning opportunities
- **Multi-language Support** - isiZulu, Afrikaans, isiXhosa content

### 🧠 Mental Health & Well-being
- **Integrated Diary** - Academic and emotional tracking
- **Psychologist Access** - Professional mental health support
- **Well-being Check-ins** - Mood and stress level monitoring
- **"Take a Break" Features** - Mindfulness and relaxation exercises

### 🎓 Future Planning
- **University Readiness** - Course matching and preparation strategies
- **Bursary Navigator** - Simplified application processes
- **Career Guidance** - Subject-to-career pathway mapping
- **Financial Literacy** - University cost and budgeting education

### 👥 Multi-role Platform
- **Student View** - Personalized learning journey and progress tracking
- **Teacher View** - Classroom analytics and assignment management
- **Parent View** - Child progress monitoring and support guidance
- **School Admin** - Institutional analytics and reporting

---

## 🔧 API Endpoints

### Authentication & Users
```
POST /api/auth/login                    # User authentication
POST /api/auth/register                 # User registration
GET  /api/users/me                      # Current user profile
PUT  /api/users/profile                 # Update user information
```

### Academic Features
```
GET  /api/goals                        # Get student goals
POST /api/goals                        # Create new academic goals
GET  /api/practice-tests               # Get available practice tests
POST /api/practice-tests/generate      # Generate personalized test
GET  /api/progress/analytics           # Learning progress analytics
```

### Mental Health
```
GET  /api/diary/entries                # Get diary entries
POST /api/diary/entries                # Create new diary entry
GET  /api/wellbeing/checkins           # Well-being history
POST /api/wellbeing/checkins           # Submit well-being check
```

### Future Planning
```
GET  /api/career/paths                 # Career recommendations
GET  /api/university/matches           # University course matching
GET  /api/bursaries                    # Available bursaries
POST /api/bursaries/reminders          # Set application reminders
```

---

## 🛠️ Development Setup

### Prerequisites
- .NET 10 SDK
- PostgreSQL 13+
- Redis 6+
- RabbitMQ 3.8+
- Docker (optional)

### Environment Configuration
```json
{
  "ConnectionStrings": {
    "PostgreSQL": "Host=localhost;Database=aptiverse;Username=postgres;Password=password",
    "Redis": "localhost:6379",
    "RabbitMQ": "amqp://localhost:5672"
  },
  "Jwt": {
    "Key": "your-secure-key",
    "Issuer": "aptiverse-auth",
    "Audience": "aptiverse-api"
  },
  "Services": {
    "AuthService": "https://auth.aptiverse.com",
    "EmailService": "https://email.aptiverse.com"
  }
}
```

### Running with Docker
```bash
# Start all services
docker-compose up -d

# Build and run specific service
docker build -t aptiverse-api .
docker run -p 8080:8080 aptiverse-api
```

### Local Development
```bash
# Restore dependencies
dotnet restore

# Run database migrations
dotnet ef database update

# Start the application
dotnet run --project src/Aptiverse.Api
```

---

## 🔐 Security & Authentication

### Authentication Flow
1. **Client** authenticates with Auth Provider service
2. **Auth Provider** returns JWT token with user claims
3. **API Service** validates token and applies role-based policies
4. **Redis** manages sessions and token blacklisting

### Rate Limiting
- **Students**: 500 requests/minute
- **Educators**: 1000 requests/minute  
- **Administrators**: 2000 requests/minute

### Data Protection
- **Encryption**: All sensitive data encrypted at rest
- **PII Protection**: Student data anonymized for analytics
- **GDPR Compliance**: Built-in data privacy controls

---

## 📊 Monitoring & Analytics

### Health Checks
```
GET /health          # API service health
GET /health/redis    # Redis connection status
GET /health/db       # Database connectivity
```

### API Documentation
- **Swagger UI**: `/swagger`
- **OpenAPI Spec**: `/openapi/v1.json`

### Logging & Monitoring
- Structured logging with Serilog
- Application Insights integration
- Performance metrics and error tracking

---

## 🎯 Business Model

### Subscription Tiers
- **🎁 Freemium** - Basic features, limited practice tests
- **👨‍🎓 Student** - Full academic features, unlimited tests
- **👪 Family** - Multiple children, parent dashboard
- **🏫 School** - Institutional analytics, teacher tools

### Value Propositions
- **For Students**: Personalized learning and mental health support
- **For Parents**: Actionable insights and progress monitoring
- **For Schools**: Data-driven intervention and university readiness
- **For Tutors**: Student matching and course marketplace

---

## 🤝 Contributing

We welcome contributions from educators, developers, and students passionate about transforming education. Please see our contribution guidelines for more details.

### Development Priorities
1. **AI Model Improvements** - Enhanced prediction accuracy
2. **Multi-language Expansion** - Additional South African languages
3. **Offline Functionality** - Data-light features for limited connectivity
4. **Integration Ecosystem** - LMS and calendar platform connections

---

## 📄 License

This project is part of the Aptiverse educational ecosystem and is proprietary software. All rights reserved.

---

## 🎓 Our Mission

> **Aptiverse exists to transform education from a source of stress to a journey of growth, empowering every South African student to reach their full potential through personalized support, mental wellness, and clear pathways to success.**

---

**Built with ❤️ for the future of South African education**
