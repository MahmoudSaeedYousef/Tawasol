# 🏛️ Clean Architecture Rules — Tawasol Backend (.NET Core Web API)
**Version:** 1.1
**Stack:** .NET Core Web API · Entity Framework Core · Clean Architecture
**Author:** Mahmoud Saeed Yousef

---

## ⚠️ CRITICAL: AI Agent Instructions

You are assisting in developing the **Tawasol** backend — a social solidarity platform for rural Egyptian communities.

**Your primary duty is to enforce Clean Architecture at all times.**
- If a request violates any rule below → **refuse, explain why, and suggest the correct approach.**
- If you are unsure which layer a piece of code belongs to → **ask before writing.**
- Never sacrifice architecture for convenience or speed.

---

## 📐 Layer Structure

The solution is organized into **4 projects (layers)**. Each layer has strict boundaries.

```
Tawasol.sln
├── Tawasol.Domain          # Layer 1 — The Core (innermost)
├── Tawasol.Application     # Layer 2 — Business Logic
├── Tawasol.Infrastructure  # Layer 3 — External Concerns
└── Tawasol.API             # Layer 4 — Entry Point (outermost)
```

### Dependency Rule (THE most important rule)
Dependencies flow **inward only**:
```
API → Application → Domain
Infrastructure → Application → Domain
```
- `Domain` depends on **nothing**.
- `Application` depends only on `Domain`.
- `Infrastructure` depends on `Application` and `Domain`.
- `API` depends on `Application` and `Infrastructure` (only for DI registration).

**NEVER allow an inner layer to reference an outer layer.**

---

## 🗂️ Layer 1: Domain

**Purpose:** The heart of the system. Contains pure business logic with zero external dependencies.

**Allowed in this layer:**
- Entities
- Value Objects
- Enums
- Domain Exceptions
- Repository Interfaces
- Domain Events (if needed)

**Folder Structure:**
```
Tawasol.Domain/
├── Entities/
│   ├── User.cs
│   ├── Case.cs
│   ├── Transaction.cs
│   └── Notification.cs
├── Enums/
│   ├── UserRole.cs
│   ├── CaseStatus.cs
│   └── TransactionStatus.cs
├── Interfaces/
│   ├── Repositories/
│   │   ├── IUserRepository.cs
│   │   ├── ICaseRepository.cs
│   │   └── ITransactionRepository.cs
│   └── IUnitOfWork.cs
└── Exceptions/
    ├── DomainException.cs
    └── NotFoundException.cs
```

### ✅ Domain Rules

**DO:**
- Keep entities as rich domain objects with validation logic inside them.
- Define repository interfaces here (`IUserRepository`, `ICaseRepository`, etc.)
- Use private setters on entity properties to protect invariants.
- Throw `DomainException` for business rule violations.

---

## 🗂️ Layer 2: Application

**Purpose:** Orchestrates use cases. Tells the system *what* to do, not *how* to do it.

**Allowed in this layer:**
- Use Cases / Commands / Queries (CQRS pattern using MediatR)
- DTOs (Request and Response models)
- Application Service Interfaces
- Mapping Profiles (AutoMapper)
- Validators (FluentValidation)
- Custom Application Exceptions
- MediatR Pipeline Behaviors (Validation, Logging, etc.)

**Folder Structure:**
```
Tawasol.Application/
├── Features/
│   ├── Cases/
│   │   ├── Commands/
│   │   │   ├── CreateCase/
│   │   │   │   ├── CreateCaseCommand.cs
│   │   │   │   ├── CreateCaseCommandHandler.cs
│   │   │   │   └── CreateCaseCommandValidator.cs
│   ├── ...
├── DTOs/
├── Mappings/
├── Common/
│   ├── Models/
│   │   └── Result.cs
│   ├── Exceptions/
│   │   ├── ValidationException.cs
│   │   └── ForbiddenException.cs
│   └── Behaviors/
│       └── ValidationBehavior.cs
├── Interfaces/
└── DependencyInjection.cs
```

### ✅ Application Rules

**DO:**
- Use **CQRS** with MediatR: every action is either a `Command` (write) or a `Query` (read).
- Use the **Result Pattern** (`Result<T>`) for all handler responses.
- Use **Pipeline Behaviors** for cross-cutting concerns like validation.
- Return DTOs, never domain entities.

---

## 🗂️ Layer 3: Infrastructure

**Purpose:** Implements all external concerns — database, storage, notifications, external APIs.

**Allowed in this layer:**
- EF Core DbContext and Configurations
- Repository Implementations
- Unit of Work Implementation
- Migrations
- External Service Implementations

**Folder Structure:**
```
Tawasol.Infrastructure/
├── Persistence/
│   ├── AppDbContext.cs
│   ├── Configurations/
│   ├── Repositories/
│   ├── UnitOfWork.cs
│   ├── DatabaseInitializer.cs
│   └── Migrations/
├── Services/
└── DependencyInjection.cs
```

---

## 🗂️ Layer 4: API (Presentation)

**Purpose:** Entry point. Handles HTTP requests and routes them to Application layer using **Minimal APIs**.

**Allowed in this layer:**
- Endpoints (Organized in classes)
- Middleware (Global Exception Handling)
- Program.cs / DI registration

**Folder Structure:**
```
Tawasol.API/
├── Endpoints/
│   ├── CaseEndpoints.cs
│   └── ...
├── Middleware/
│   └── ExceptionHandlingMiddleware.cs
├── Properties/
│   └── launchSettings.json
├── appsettings.json
└── Program.cs
```

### ✅ API Rules

**DO:**
- Use **Minimal APIs** for endpoint definitions.
- Keep `Program.cs` lean by using extension methods for endpoint mapping (e.g., `app.MapCaseEndpoints()`).
- Use `ISender` (MediatR) to dispatch commands and queries.
- Use **Global Exception Middleware** to catch all exceptions and return a standardized `Result` structure.
- Return standardized response wrappers (`Result<T>`).

**NEVER:**
- Use Controllers (unless absolutely necessary for legacy reasons).
- Inject repositories or DbContext directly into endpoints.
- Return raw entity objects — always use the `Result` wrapper with DTOs.

**Example — Correct Minimal API Endpoint:**
```csharp
// ✅ CORRECT
public static void MapCaseEndpoints(this IEndpointRouteBuilder app)
{
    var group = app.MapGroup("/api/cases");

    group.MapPost("/", async (CreateCaseCommand command, ISender mediator) =>
    {
        var result = await mediator.Send(command);
        return Results.Ok(result);
    });
}
```

---

## 📛 Naming Conventions

| Artifact | Convention | Example |
|---|---|---|
| Command | `{Action}{Entity}Command` | `CreateCaseCommand` |
| Query | `Get{Entity}By{Criteria}Query` | `GetCaseByIdQuery` |
| Handler | `{Command/Query}Handler` | `CreateCaseCommandHandler` |
| Repository Interface | `I{Entity}Repository` | `ICaseRepository` |
| Repository Implementation | `{Entity}Repository` | `CaseRepository` |
| Endpoint Class | `{Entity}Endpoints` | `CaseEndpoints` |

---

## 🔄 Correct Request Flow

```
HTTP Request
    ↓
[API] Minimal API Endpoint
    ↓ (sends Command/Query via ISender)
[Application] ValidationBehavior (Pipeline)
    ↓ (if valid)
[Application] Handler
    ↓ (calls via Interface)
[Domain] Entity / Repository Interface
    ↑ (implemented by)
[Infrastructure] Repository / DbContext
    ↓
Database
    ↑
[Infrastructure] Returns Entity
    ↑
[Application] Maps to DTO & wraps in Result<T>
    ↑
[API] Returns HTTP Response (JSON Result)
```

## 💼 Core Business & Donation Models (Updated)

**1. Donation Types:**
* **Monetary Donations:** Direct financial transfers to Cases, Emergency Fund, or Platform Operations Fund.
* **In-Kind Donations (تبرع عيني):** Donors can pledge to buy specific physical items (e.g., a fridge for a marriage case) instead of paying money.

**2. Fund & Wallet Structure:**
* **Case Wallets:** Specific funds tied to a published Case.
* **General Wallets:** Invisible in the "Cases List" but available as donation options:
    * `EmergencyFund`: For urgent community needs.
    * `PlatformOperationsFund`: To support app hosting and admin costs.

**3. Case Items (Needs):**
* Cases can have multiple `CaseItems`.
* Each `CaseItem` has a `Type` (Monetary Need vs. Physical Item).
* If a physical item is pledged by a donor, it is locked so others cannot pledge for the same item.