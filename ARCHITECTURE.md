# Tawasol Solution Architecture

This document describes the architectural patterns and project structure for the Tawasol backend system.

## 🏛️ Clean Architecture

The solution follows **Clean Architecture** principles, ensuring a separation of concerns and a dependency flow that always points inward toward the Domain.

```
Tawasol.sln
├── Tawasol.Domain          # Layer 1: Core Domain Entities & Logic
├── Tawasol.Application     # Layer 2: Business Use Cases (CQRS)
├── Tawasol.Infrastructure  # Layer 3: Persistence & External Services
└── Tawasol.API             # Layer 4: Presentation (Minimal API)
```

---

## 🗂️ Layer Details

### 1. Tawasol.Domain
The innermost layer. It contains the business heart of the application and has **zero dependencies** on other layers or external frameworks.

- **Entities**: Rich domain objects (e.g., `Case`, `User`, `Transaction`).
- **Enums**: Domain-specific constants (e.g., `CaseStatus`).
- **Interfaces**: Repository contracts and Unit of Work definition.
- **Exceptions**: Custom domain-specific exceptions.

### 2. Tawasol.Application
Contains the application's business logic and orchestrates the flow of data. It depends only on the **Domain** layer.

- **Features (CQRS)**: Organized by entity, containing Commands (Writes) and Queries (Reads).
- **MediatR**: Used for decoupling the request from the handler.
- **DTOs**: Data Transfer Objects for API responses.
- **AutoMapper**: Maps Domain Entities to DTOs.
- **FluentValidation**: Validates incoming commands/requests.

### 3. Tawasol.Infrastructure
Implements the interfaces defined in the Domain and Application layers. It handles technical concerns like database access.

- **Persistence**: EF Core `AppDbContext`.
- **Configurations**: Fluent API configurations for database mapping.
- **Repositories**: Concrete implementations of the repository interfaces.
- **UnitOfWork**: Manages database transactions.

### 4. Tawasol.API
The entry point of the application. It handles HTTP requests and returns responses.

- **Minimal APIs**: Lightweight endpoint definitions.
- **Dependency Injection**: Orchestrates the registration of all layers.
- **Swagger**: API documentation and testing interface.

---

## 🛠️ Technology Stack

- **Framework**: .NET 10.0
- **ORM**: Entity Framework Core 10 (SQL Server)
- **Mediator Pattern**: MediatR
- **Object Mapping**: AutoMapper
- **Validation**: FluentValidation
- **API Style**: Minimal APIs

---

## 🔄 Request Flow

1. **Client** sends an HTTP request to an endpoint in **Tawasol.API**.
2. The endpoint dispatches a **Command** or **Query** via **MediatR**.
3. A **Handler** in **Tawasol.Application** receives the request.
4. The Handler interacts with **Tawasol.Domain** (Entities) and **Tawasol.Infrastructure** (Repositories).
5. Data is persisted via the **Unit of Work**.
6. The Handler maps the result to a **DTO** and returns it to the API.
7. **Tawasol.API** returns the final HTTP response.
