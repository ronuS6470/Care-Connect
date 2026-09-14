# Employee Management System (EMS)

A modern, enterprise-grade Employee Management System built with .NET and C#. This project demonstrates clean architecture principles with CQRS (Command Query Responsibility Segregation) pattern, dependency injection, and repository pattern.

## 📋 Table of Contents

- [Project Overview](#project-overview)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [New Project Reference](#new-project-reference)
- [Prerequisites](#prerequisites)
- [Installation & Setup](#installation--setup)
- [Configuration](#configuration)
- [Running the Application](#running-the-application)
- [API Endpoints](#api-endpoints)
- [Project Layers](#project-layers)
- [Technologies Used](#technologies-used)

---

## 🎯 Project Overview

The Employee Management System is a RESTful API that provides comprehensive employee data management capabilities. It follows clean architecture principles to ensure maintainability, scalability, and testability.

**Key Features:**
- ✅ Create, Read, Update, Delete (CRUD) operations for employees
- ✅ Separation of concerns using CQRS pattern
- ✅ Dependency Injection for loose coupling
- ✅ AutoMapper for DTO transformations
- ✅ SQL Server database with Entity Framework Core
- ✅ Repository pattern for data access abstraction

---

## 🏗️ Architecture

This project implements a **Clean Architecture** with **CQRS Pattern** separation:

```
┌─────────────────────────────────────────┐
│   EMS-Controller (Presentation Layer)    │
│   REST API Endpoints                     │
└─────────────┬──────────────────────────┘
              │
┌─────────────▼──────────────────────────┐
│   EMS-AppServices (Application Layer)   │
│   Business Logic & Orchestration        │
└─────────────┬──────────────────────────┘
              │
     ┌────────┴─────────┐
     │                  │
┌────▼─────┐    ┌──────▼──────┐
│ COMMANDS  │    │   QUERIES   │
│ (Writes)  │    │   (Reads)   │
└────┬─────┘    └──────┬──────┘
     │                 │
     └────────┬────────┘
              │
┌─────────────▼──────────────────────┐
│   EMS-Infrastructure (Data Layer)   │
│   Database Context, Repositories    │
└──────────────────────────────────┘
```

---

## 📁 Project Structure

### **EMS-Controller** (Presentation Layer)
The ASP.NET Core REST API controller layer that handles HTTP requests and responses.

```
EMS-Controller/
├── Program.cs                    # Dependency Injection & App Configuration
├── appsettings.json             # Configuration settings
├── appsettings.Development.json # Development-specific settings
├── EMS-Controller.http          # HTTP request examples
├── Controllers/
│   └── EmployeeController.cs    # Employee REST endpoints
└── Properties/
    └── launchSettings.json      # Launch profiles
```

### **EMS-AppServices** (Application/Business Logic Layer)
Contains business logic and orchestrates commands and queries.

```
EMS-AppServices/
├── EmployeeService.cs           # Main service orchestrating CQRS operations
├── Mapping/
│   └── EmployeeMappingProfile.cs # AutoMapper configuration
└── EMS-AppServices.csproj
```

### **EMS-Commands** (CQRS Command Side)
Handles all write operations using the Command pattern.

```
EMS-Commands/
├── IEmployeeCommand.cs          # Command interface base
├── Employee/
│   ├── AddEmployeeCommand.cs
│   ├── AddEmployeeCommandHandler.cs
│   ├── UpdateEmployeeCommand.cs
│   ├── UpdateEmployeeCommandHandler.cs
│   ├── DeleteEmployeeCommand.cs
│   └── DeleteEmployeeCommandHandler.cs
└── EMS-Commands.csproj
```

### **EMS-Queries** (CQRS Query Side)
Handles all read operations using the Query pattern.

```
EMS-Queries/
├── IEmployeeQuery.cs            # Query interface base
├── Employee/
│   ├── GetAllEmployeesQuery.cs
│   ├── GetAllEmployeesQueryHandler.cs
│   ├── GetEmployeeByIdQuery.cs
│   └── GetEmployeeByIdQueryHandler.cs
└── EMS-Queries.csproj
```

### **EMS-Infrastructure** (Data Access Layer)
Contains database models, context, and repositories.

```
EMS-Infrastructure/
├── AppDbContext.cs              # Entity Framework DbContext
├── Models/
│   └── EmployeeModel.cs         # Domain model
├── Repositories/
│   ├── IEmployeeRepository.cs   # Repository interface
│   └── EmployeeRepository.cs    # Repository implementation
├── Mapping/
│   └── EmployeeMapping.cs       # EF Core entity mapping
└── EMS-Infrastructure.csproj
```

### **EMS-DTO** (Data Transfer Objects)
Contains DTOs for API contracts.

```
EMS-DTO/
├── EmployeeDto.cs              # Employee data transfer object
└── EMS-DTO.csproj
```

---

## 📚 New Project Reference

For creating a new project with the same layered CQRS architecture and coding standards, use:

- [NEW-PROJECT-REFERENCE.md](./NEW-PROJECT-REFERENCE.md)

---

## 📦 Prerequisites

Before you begin, ensure you have the following installed:

- **.NET 9.0 SDK** or later ([Download](https://dotnet.microsoft.com/download))
- **SQL Server** (LocalDB, Express, or full version)
- **Visual Studio 2022** (recommended) or VS Code
- **Git** (for version control)

---

## 🚀 Installation & Setup

### 1. Clone the Repository
```bash
git clone <repository-url>
cd EMS
```

### 2. Install Dependencies
```bash
dotnet restore
```

### 3. Create Database
The application uses SQL Server. Update the connection string in `appsettings.json`, then run migrations:

```bash
cd EMS-Controller
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 4. Verify Installation
```bash
dotnet build
```

---

## ⚙️ Configuration

### Connection String

Update the `DefaultConnection` in `EMS-Controller/appsettings.json`:

**For Local SQL Server:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=EMS_DB;Trusted_Connection=true;"
  }
}
```

**For SQL Server Express:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=EMS_DB;Trusted_Connection=true;"
  }
}
```

**For LocalDB:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=EMS_DB;Trusted_Connection=true;"
  }
}
```

---

## ▶️ Running the Application

### Start the API Server
```bash
cd EMS-Controller
dotnet run
```

The API will be available at: **https://localhost:5001**

### Using Visual Studio
1. Open `EMS.sln` in Visual Studio 2022
2. Set `EMS-Controller` as the startup project
3. Press `F5` to run with debugging

---

## 🔌 API Endpoints

All endpoints use the base URL: `https://localhost:5001/api/employee`

### Get All Employees
```http
GET /api/employee
```
Returns a list of all employees.

**Response:**
```json
[
  {
    "id": 1,
    "name": "John Doe",
    "email": "john@example.com",
    "department": "Engineering"
  }
]
```

### Get Employee by ID
```http
GET /api/employee/{id}
```
Returns a specific employee by ID.

**Example:**
```http
GET /api/employee/1
```

### Create Employee
```http
POST /api/employee
Content-Type: application/json

{
  "name": "Jane Smith",
  "email": "jane@example.com",
  "department": "HR"
}
```

### Update Employee
```http
PUT /api/employee/{id}
Content-Type: application/json

{
  "name": "Jane Smith",
  "email": "jane.smith@example.com",
  "department": "Management"
}
```

### Delete Employee
```http
DELETE /api/employee/{id}
```

Deletes a specific employee by ID.

---

## 🏛️ Project Layers

### 1. **Presentation Layer** (EMS-Controller)
- Handles HTTP requests and responses
- Routes API calls to appropriate services
- Returns JSON responses

### 2. **Application Layer** (EMS-AppServices)
- Contains business logic
- Orchestrates Commands and Queries
- Handles DTO transformations using AutoMapper

### 3. **Command Layer** (EMS-Commands)
- Implements write operations (Create, Update, Delete)
- Each command has a corresponding handler
- Ensures single responsibility principle

### 4. **Query Layer** (EMS-Queries)
- Implements read operations (Get, GetAll)
- Query handlers process read requests
- Separates read and write concerns

### 5. **Data Layer** (EMS-Infrastructure)
- Entity Framework Core DbContext
- Repository pattern implementation
- Database models and mappings
- Data access abstraction

### 6. **DTO Layer** (EMS-DTO)
- Defines data contracts for API
- Provides data transfer objects
- Decouples domain models from API contracts

---

## 🛠️ Technologies Used

| Technology | Purpose |
|-----------|---------|
| **.NET 9.0** | Application framework |
| **C#** | Programming language |
| **ASP.NET Core** | Web API framework |
| **Entity Framework Core** | ORM for database access |
| **SQL Server** | Database |
| **AutoMapper** | Object mapping |
| **Dependency Injection** | Loose coupling |

---

## 📚 Design Patterns Used

- **CQRS (Command Query Responsibility Segregation)**: Separates read and write operations
- **Repository Pattern**: Abstracts data access logic
- **Dependency Injection**: Manages dependencies
- **DTO Pattern**: Data transfer objects for API contracts
- **Handler Pattern**: Encapsulates command and query processing

---

## 🔄 Request Flow Example

When you call `POST /api/employee`:

1. **EmployeeController** receives the HTTP request
2. Controller calls `EmployeeService.AddEmployee()`
3. **EmployeeService** creates an `AddEmployeeCommand`
4. **AddEmployeeCommandHandler** processes the command
5. Handler uses **IEmployeeRepository** to save data
6. **AppDbContext** persists data to SQL Server
7. Response is returned to the client

---

## 📝 Notes

- Each command and query has a dedicated handler following the Single Responsibility Principle
- AutoMapper handles DTO ↔ Domain Model transformations
- Repository pattern provides a clean abstraction over Entity Framework
- The application uses dependency injection for maximum flexibility and testability

---

## 🤝 Contributing

1. Create a new branch for your feature
2. Make your changes
3. Test thoroughly
4. Submit a pull request

---

## 📞 Support

For issues or questions, please open an issue in the repository.

---

**Version:** 1.0.0  
**Last Updated:** April 2026
