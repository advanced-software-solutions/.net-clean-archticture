# AGENTS.md - .NET Clean Architecture

Guidance for agentic coding agents working in this repository.

## Project Overview

.NET 10 Clean Architecture with four layers: CleanBase (entities/DTOs), CleanOperation (data access), CleanBusiness (services/actors), CleanAPI (REST API).

## Build, Test, and Run Commands

### Build
```bash
dotnet build CleanArchitecture.sln
dotnet build CleanArchitecture.sln --configuration Release
dotnet clean && dotnet build
```

### Run Tests
```bash
dotnet test
dotnet test --filter "FullyQualifiedName~UsersTests"
dotnet test --filter "FullyQualifiedName~UsersTests.CheckLogin"
dotnet test --filter "FullyQualifiedName~UserRolesTests.Add"
```

### Run Application
```bash
dotnet run --project CleanAPI/CleanAPI.csproj
dotnet run --project CleanAPI/CleanAPI.csproj --environment Development
```

## Solution Structure

| Project | Purpose |
|---------|---------|
| CleanBase | Entities, DTOs, validators, enums, extensions |
| CleanOperation | EF Core DbContext, repositories, low-level operations |
| CleanBusiness | Services, Akka actors, business logic |
| CleanAPI | ASP.NET Core Web API, controllers, middleware |
| CeanArchitecture.Tests | xUnit integration tests |

## Code Style Guidelines

### File Organization & Formatting
- One class per file, file name matches class name
- Namespace matches folder structure
- System imports first, then third-party, then project namespaces
- ImplicitUsings enabled, alphabetize imports
- 4 spaces indentation (not tabs), opening brace on same line

### Naming Conventions

| Type | Convention | Example |
|------|------------|---------|
| Classes | PascalCase | `TodoList`, `UserAccount` |
| Interfaces | IPascalCase | `IRepository`, `IRootService` |
| Methods | PascalCase | `GetAsync`, `InsertList` |
| Properties | PascalCase | `Title`, `DueDate` |
| Private fields | _camelCase | `_dataContext`, `_repository` |
| Parameters | camelCase | `entity`, `includedNavigations` |
| Constants | PascalCase | `AdminRoleId` |
| Entity tables | Plural PascalCase | `TodoLists`, `UserAccounts` |

### Entity Guidelines

All entities must inherit from `EntityRoot` (provides `Guid Id` and `byte[]? Rowversion`). NO data annotations on properties - use FluentAPI instead. Navigation properties should be nullable.

```csharp
public partial class TodoList : EntityRoot
{
    public string Title { get; set; }
    public DateTime DueDate { get; set; }
    public IList<TodoItem>? TodoItems { get; set; }
}
```

### Repository Pattern

Use `IRepository<T>` for data access:
- `Get(Guid id)` / `GetAsync(Guid id)` - Single entity retrieval
- `Get(Expression<Func<T, bool>> query)` - Query by expression
- `Insert(T entity)` / `InsertAsync(T entity)` - Add entity
- `Update(T entity)` - Update entity
- `Delete(T entity)` / `Delete(Guid id)` - Remove entity
- `Query()` - IQueryable access

### Validation

Use FluentValidation in `CleanBase/Validator/`:

```csharp
public class TodoListValidation : AbstractValidator<TodoList>
{
    public TodoListValidation()
    {
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.DueDate).GreaterThanOrEqualTo(DateTime.Now);
    }
}
```

### Error Handling

1. Use `Ardalis.GuardClauses` for argument validation: `Guard.Against.Null(entity);`
2. Use `EntityResult<T>` for operation results (Data, Message, IsSuccess)
3. Wrap database operations in aspect methods (transactions handled automatically)

### Async Patterns

- Prefer async methods for I/O operations
- Append `Async` suffix: `GetAsync`, `InsertAsync`
- Use `Task<T>` for async returns, `Task` for void returns

### Dependency Injection

```csharp
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

public class RootService<T> : IRootService<T>
{
    public readonly IRepository<T> _repository;
    public RootService(IRepository<T> repository) => _repository = repository;
}
```

### Logging

Use Serilog: `Log.Information("Data: {@0}", entity);`

### Controllers

Inherit from `BaseController<T>` and use `[EnableQuery]` for OData endpoints.

### EF Core Configuration

Use FluentAPI in `AppDataContext.OnModelCreating`:
```csharp
modelBuilder.Entity<TodoList>().ToTable("TodoLists", "Core");
```
Do NOT use data annotations on entity classes.

### Actors (Akka)

Implement `ICleanActor` and inherit from `UntypedActor`.

## Key Libraries

| Library | Purpose |
|---------|---------|
| FluentValidation | Input validation |
| Ardalis.GuardClauses | Null/argument checks |
| Serilog | Structured logging |
| Entity Framework Core | ORM |
| FastEndpoints | API endpoints |
| OData | Query capabilities |
| Akka.NET | Actor model |
| xUnit | Testing |
| Bogus | Test data generation |
| Refit | API client |
| Riok.Mapperly | Object mapping |

## Testing Guidelines

- Tests in `CeanArchitecture.Tests` project
- Use xUnit with `[Fact]` attribute
- Use Bogus for test data generation
- Integration tests use Refit to call API endpoints
- Test naming: `{MethodName}_{Scenario}`

## Pre-commit Checklist

1. `dotnet build` succeeds with no errors
2. `dotnet test` passes
3. Follow naming conventions
4. Use GuardClauses for null checks
5. Entities inherit from EntityRoot
6. Validation uses FluentValidation
7. Async methods have `Async` suffix
