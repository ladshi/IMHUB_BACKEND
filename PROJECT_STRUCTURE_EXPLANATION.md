# IMHub Backend - Project Structure Explanation (Tamil/English)

## 📁 Project Folders - எதற்கு Use பண்றாங்க?

இந்த project **Clean Architecture** pattern-ஐ follow பண்றது. ஒவ்வொரு folder-um ஒரு specific responsibility-க்கு use ஆகுது.

---

## 1. 🎯 **IMHub.Domain** (Core Business Entities)

**எதற்கு Use பண்றாங்க:**
- **Business Entities** (Database tables-க்கு corresponding classes)
- **Enums** (Status types, Field types, etc.)
- **Base Classes** (Common properties like Id, CreatedAt, etc.)

**என்ன Contains பண்றது:**
- `Entities/` - User, Organization, Template, Content, Sendout, etc.
- `Enums/` - AssignmentStatus, FieldType, PlanType, etc.
- `Common/` - BaseEntity class

**Example:**
```csharp
// IMHub.Domain/Entities/Identity/User.cs
public class User : BaseEntity
{
    public string Name { get; set; }
    public string Email { get; set; }
    // ...
}
```

**Important:** 
- இது **pure C# classes** மட்டும்
- Database, API, External services-க்கு dependency இல்லை
- Business logic-க்கு core entities

---

## 2. 🔧 **ApplicationLayer** (Business Logic & Use Cases)

**எதற்கு Use பண்றாங்க:**
- **Business Logic** (What the application does)
- **Use Cases** (Login, Register, Create Template, etc.)
- **Commands & Queries** (CQRS pattern)
- **Validation Rules** (FluentValidation)
- **DTOs** (Data Transfer Objects)

**என்ன Contains பண்றது:**
- `Features/` - Auth, Organizations, SuperAdmin features
  - `Auth/Commands/` - LoginCommand, RegisterCommand
  - `Auth/Queries/` - GetCurrentUserQuery
  - `Organizations/Templates/` - Template management
  - `Organizations/Contents/` - Content management
- `Common/Interfaces/` - Service interfaces
- `Common/Models/` - DTOs, PagedResult, etc.

**Example:**
```csharp
// ApplicationLayer/Features/Auth/Commands/LoginCommandHandler.cs
public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    // Business logic for login
    // Validates credentials
    // Generates JWT token
}
```

**Important:**
- Domain-ஐ reference பண்றது (entities use பண்றது)
- Infrastructure-க்கு dependency இல்லை (interfaces மட்டும் use பண்றது)
- Business rules, validation, workflows இங்கே

---

## 3. 🏗️ **Infrastructure Layer** (External Services & Data Access)

**எதற்கு Use பண்றாங்க:**
- **Database Access** (Entity Framework, Repositories)
- **External Services** (Email, File Storage, JWT)
- **Data Configuration** (Entity mappings, Migrations)
- **Repository Implementations**

**என்ன Contains பண்றது:**
- `Data/` - DbContext, Entity Configurations, Migrations
- `Repositories/` - UserRepository, TemplateRepository, etc.
- `Services/` - LocalFileStorageService
- `Service_external/` - SendGridEmailService
- `Authendication/` - JwtTokenGenerator
- `Data/DbInitializers_Seeds/` - Database seeding (SuperAdmin, Roles)

**Example:**
```csharp
// Infrastructure Layer/Repositories/UserRepository.cs
public class UserRepository : GenericRepository<User>, IUserRepository
{
    // Database operations for User entity
    // Implements IUserRepository interface
}
```

**Important:**
- ApplicationLayer-க்கு implementations provide பண்றது
- Database, File System, External APIs-க்கு access
- Infrastructure details (SQL Server, File Storage, etc.)

---

## 4. 🌐 **IMHub.API** (Web API - Entry Point)

**எதற்கு Use பண்றாங்க:**
- **REST API Endpoints** (HTTP requests handle பண்றது)
- **Controllers** (API routes)
- **Middleware** (Exception handling, CORS, Authentication)
- **Configuration** (appsettings.json, Program.cs)

**என்ன Contains பண்றது:**
- `Controllers/` - AuthController, ContentsController, etc.
- `Middleware/` - ExceptionMiddleware, CorrelationIdMiddleware
- `Extensions/` - AuthenticationServiceExtensions
- `Services/` - CurrentUserService
- `Program.cs` - Application startup, dependency injection

**Example:**
```csharp
// IMHub.API/Controllers/AuthController.cs
[Route("api/auth")]
public class AuthController : BaseController
{
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginCommand command)
    {
        // Calls ApplicationLayer handler
        return await HandleRequestAsync(command);
    }
}
```

**Important:**
- Frontend-க்கு API endpoints provide பண்றது
- HTTP requests receive பண்றது, ApplicationLayer-க்கு forward பண்றது
- Authentication, CORS, Error handling manage பண்றது

---

## 5. 🧪 **IMHub.Application.UnitTests** (Unit Tests)

**எதற்கு Use பண்றாங்க:**
- **Unit Tests** for ApplicationLayer
- **Handler Tests** (LoginCommandHandler, etc.)
- **Business Logic Testing**

**என்ன Contains பண்றது:**
- Test files for ApplicationLayer features
- Mock objects for testing
- Test data and scenarios

**Example:**
```csharp
// IMHub.Application.UnitTests/Features/Auth/LoginCommandHandlerTests.cs
[Test]
public async Task Login_WithValidCredentials_ReturnsToken()
{
    // Test login handler logic
}
```

---

## 6. 🔗 **IMHub.Infrastructure.IntegrationTests** (Integration Tests)

**எதற்கு Use பண்றாங்க:**
- **Integration Tests** for Infrastructure Layer
- **Database Tests** (Repository tests with real database)
- **End-to-End Tests**

**என்ன Contains பண்றது:**
- Tests for repositories
- Database integration tests
- External service tests

---

## 📊 Architecture Flow (எப்படி Work பண்றது)

```
Frontend (React)
    ↓ HTTP Request
IMHub.API (Controllers)
    ↓ MediatR
ApplicationLayer (Handlers - Business Logic)
    ↓ Interfaces
Infrastructure Layer (Repositories - Data Access)
    ↓ Entity Framework
IMHub.Domain (Entities)
    ↓
Database (SQL Server)
```

### Example Flow: Login Request

1. **Frontend** → `POST /api/auth/login` with email & password
2. **IMHub.API/AuthController** → Receives request
3. **ApplicationLayer/LoginCommandHandler** → Validates, processes business logic
4. **Infrastructure Layer/UserRepository** → Gets user from database
5. **Infrastructure Layer/JwtTokenGenerator** → Creates JWT token
6. **Response** → Returns token to frontend

---

## 🎯 Key Points

### Dependency Direction:
```
IMHub.API
    ↓ depends on
ApplicationLayer
    ↓ depends on
IMHub.Domain
    ↑ depends on
Infrastructure Layer
```

### Rules:
1. **Domain** - எதையும் depend பண்ணாது (Pure business entities)
2. **ApplicationLayer** - Domain மட்டும் depend பண்றது
3. **Infrastructure** - Domain & ApplicationLayer depend பண்றது (implementations)
4. **API** - ApplicationLayer & Infrastructure depend பண்றது (entry point)

---

## 📝 Summary Table

| Project | Purpose | Contains | Dependencies |
|---------|---------|----------|--------------|
| **IMHub.Domain** | Business Entities | Entities, Enums, Base Classes | None (Pure C#) |
| **ApplicationLayer** | Business Logic | Commands, Queries, Handlers, DTOs | Domain only |
| **Infrastructure Layer** | Data Access & External Services | Repositories, DbContext, Services | Domain, ApplicationLayer |
| **IMHub.API** | Web API Entry Point | Controllers, Middleware, Configuration | ApplicationLayer, Infrastructure |
| **IMHub.Application.UnitTests** | Unit Tests | Test files for ApplicationLayer | ApplicationLayer |
| **IMHub.Infrastructure.IntegrationTests** | Integration Tests | Test files for Infrastructure | Infrastructure |

---

## 🔍 Real Example: Create Template Flow

1. **Frontend** calls: `POST /api/organizations/templates`
2. **IMHub.API/OrganizationTemplatesController** receives request
3. **ApplicationLayer/CreateTemplateCommandHandler** processes:
   - Validates request
   - Checks permissions
   - Creates Template entity
4. **Infrastructure Layer/TemplateRepository** saves to database
5. **Response** returns to frontend

---

**இது Clean Architecture pattern - separation of concerns, testability, maintainability-க்கு best practice!** ✅

