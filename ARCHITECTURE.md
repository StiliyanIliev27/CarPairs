# CarPairs Application Architecture Documentation

**Document Version:** 1.0  
**Last Updated:** March 2026  
**Purpose:** Comprehensive guide for developers to understand and extend the CarPairs application following established patterns.

---

## Table of Contents

1. [Architecture Overview](#architecture-overview)
2. [Project Structure](#project-structure)
3. [Layered Architecture](#layered-architecture)
4. [Data Flow and Connections](#data-flow-and-connections)
5. [Design Patterns](#design-patterns)
6. [Detailed Layer Descriptions](#detailed-layer-descriptions)
7. [Implementing New Features](#implementing-new-features)
8. [Code Examples and Patterns](#code-examples-and-patterns)
9. [Best Practices](#best-practices)

---

## Architecture Overview

**CarPairs** is a distributed, three-tier ASP.NET Core application for managing automotive parts inventory. The architecture follows a **layered pattern** with clear separation of concerns:

```
┌─────────────────────────────────────────────────────────┐
│                  CarPairs Web (MVC)                     │
│            Controllers → Services → HTTP Clients        │
└─────────────────────────────────────────────────────────┘
                           ↓ (REST API Calls)
┌─────────────────────────────────────────────────────────┐
│                  CarPairs.API (Web API)                 │
│            Controllers → Core Services → Models         │
└─────────────────────────────────────────────────────────┘
                           ↓ (Service Calls)
┌─────────────────────────────────────────────────────────┐
│                    CarPairs.Core                        │
│    Services → EntityFramework → SQL Server Database     │
└─────────────────────────────────────────────────────────┘
```

### Key Characteristics

- **Decoupled Design:** Each layer communicates through well-defined interfaces
- **SOLID Principles:** Dependency injection, single responsibility, interfaces for abstraction
- **Async/Await Pattern:** All data operations are asynchronous for scalability
- **Pagination Support:** Large datasets are handled with paging
- **Data Transfer Objects (DTOs):** Web layer uses DTOs to decouple internal models from API contracts
- **Service Layer Pattern:** Business logic is centralized in services

---

## Project Structure

```
CarPairs/
├── CarPairs/                    # Web UI (MVC Application)
│   ├── Controllers/
│   │   ├── HomeController.cs
│   │   └── PartsController.cs
│   ├── Services/
│   │   ├── PartApiService.cs
│   │   ├── LookupApiService.cs
│   │   └── Interfaces/
│   │       ├── IPartApiService.cs
│   │       └── ILookupApiService.cs
│   ├── Views/
│   │   ├── Parts/
│   │   ├── Home/
│   │   └── Shared/
│   ├── Extensions/
│   │   └── ServiceCollectionExtensions.cs
│   ├── wwwroot/
│   ├── Program.cs
│   └── appsettings.json
│
├── CarPairs.API/                # REST API Layer
│   ├── Controllers/
│   │   ├── PartsController.cs
│   │   ├── ManufacturersController.cs
│   │   └── CategoriesController.cs
│   ├── DTOs/
│   │   └── Parts/
│   │       ├── PartDto.cs
│   │       ├── CreatePartDto.cs
│   │       └── UpdatePartDto.cs
│   ├── Program.cs
│   └── appsettings.json
│
├── CarPairs.Core/               # Business Logic & Data Layer
│   ├── Data/
│   │   ├── ApplicationDbContext.cs
│   │   ├── Models/
│   │   │   ├── Part.cs
│   │   │   ├── Manufacturer.cs
│   │   │   └── Category.cs
│   │   └── Migrations/
│   ├── Services/
│   │   ├── PartService.cs
│   │   ├── ManufacturerService.cs
│   │   ├── CategoryService.cs
│   │   └── Interfaces/
│   │       ├── IPartService.cs
│   │       ├── IManufacturerService.cs
│   │       └── ICategoryService.cs
│   ├── Common/
│   │   ├── PagedResult.cs
│   │   └── SimpleLookupDto.cs
│   └── CarPairs.Core.csproj
│
└── LICENSE & README
```

---

## Layered Architecture

### 1. **Presentation Layer (CarPairs - Web UI)**

**Responsibility:** User interface, controllers, and HTTP communication with the API.

**Components:**
- **Controllers:** Handle HTTP requests, interpret user input, call services
- **Services (HTTP Clients):** Abstract API communication with typed endpoints
- **Views:** Razor templates for rendering HTML
- **Models:** ViewModels for passing data to views (distinct from Core models)
- **Extensions:** Dependency injection configuration

**Key Files:**
- `CarPairs/Program.cs` - Configures DI, HTTP clients, routing
- `CarPairs/Services/*` - HTTP client services for API communication
- `CarPairs/Controllers/*` - MVC controllers handling requests

**Characteristics:**
- Communicates exclusively with `CarPairs.Core` through HTTP (via `CarPairs.API`)
- No direct database access
- UI-focused logic (dropdown population, form validation display, etc.)

---

### 2. **API Layer (CarPairs.API)**

**Responsibility:** REST API endpoints that expose business logic via HTTP.

**Components:**
- **Controllers:** RESTful endpoints following the Resource-Oriented Architecture pattern
- **DTOs:** Data contracts for request/response serialization
- **Swagger/OpenAPI:** Self-documenting API specification (JWT ready)

**Key Files:**
- `CarPairs.API/Program.cs` - Configures DI, database, Swagger, authentication
- `CarPairs.API/Controllers/*` - RESTful endpoints
- `CarPairs.API/DTOs/*` - Request/response models

**API Endpoints Implemented:**

| Method | Endpoint | Purpose |
|--------|----------|---------|
| GET | `/api/parts?pageNumber=1&pageSize=10&search=` | Get paginated parts list |
| GET | `/api/parts/{id}` | Get part by ID |
| POST | `/api/parts` | Create new part |
| PUT | `/api/parts/{id}` | Update existing part |
| DELETE | `/api/parts/{id}` | Delete part |
| GET | `/api/manufacturers/lookup` | Get manufacturers for dropdowns |
| GET | `/api/categories/lookup` | Get categories for dropdowns |

**Characteristics:**
- Stateless design (no session dependency)
- Status codes follow HTTP conventions (200, 201, 204, 400, 404, etc.)
- Async operations with CancellationToken support
- DTOs act as contracts between external clients and internal models

---

### 3. **Core Layer (CarPairs.Core)**

**Responsibility:** Business logic, data access, and domain models.

**Components:**
- **Models:** Domain entities (Part, Manufacturer, Category)
- **DbContext:** Entity Framework Core configuration
- **Services:** Business logic implementation
- **Interfaces:** Abstract service contracts
- **Common:** Shared DTOs and utilities (PagedResult, SimpleLookupDto)

**Key Files:**
- `CarPairs.Core/Data/ApplicationDbContext.cs` - EF Core DbContext
- `CarPairs.Core/Data/Models/*` - Domain entities
- `CarPairs.Core/Services/*` - Business logic
- `CarPairs.Core/Common/*` - Shared utilities

**Database Configuration:**
- **Database Engine:** SQL Server
- **ORM:** Entity Framework Core
- **Migrations:** Code-first migrations tracked in `CarPairs.Core/Data/Migrations`
- **Seeding:** Categories and Manufacturers are automatically seeded

**Characteristics:**
- No HTTP dependencies or web framework references
- Pure business logic independent of presentation
- Testable without web server
- Entity models include validation attributes

---

## Data Flow and Connections

### Request Flow: User → Web UI → API → Core → Database

```
1. USER INTERACTION
   └─> Browser sends HTTP request to CarPairs Web UI

2. WEB UI CONTROLLER (CarPairs/Controllers/PartsController.cs)
   └─> PartsController.Create()
   └─> Calls IPartApiService (HTTP client service)

3. API CLIENT SERVICE (CarPairs/Services/PartApiService.cs)
   └─> Makes HTTP POST request to CarPairs.API
   └─> Example: _client.PostAsJsonAsync("api/parts", dto)

4. API CONTROLLER (CarPairs.API/Controllers/PartsController.cs)
   └─> PartsController.CreatePart(CreatePartDto dto)
   └─> Validates DTO
   └─> Maps DTO → Domain Model (Part)
   └─> Calls IPartService.CreateAsync(part)

5. CORE SERVICE (CarPairs.Core/Services/PartService.cs)
   └─> PartService.CreateAsync(Part part)
   └─> Sets CreatedAt timestamp
   └─> Calls _context.Parts.Add(part)
   └─> Calls _context.SaveChangesAsync()

6. ENTITY FRAMEWORK (CarPairs.Core/Data/ApplicationDbContext.cs)
   └─> Translates to SQL Query
   └─> Executes INSERT statement

7. RESPONSE FLOW (Reverse)
   └─> Database returns generated ID
   └─> Service returns ID to API Controller
   └─> API Controller returns Created(201) response
   └─> Web UI receives response and handles success/error
   └─> User is redirected to list view
```

### Example Data Flow: Get Parts with Search and Pagination

```
Web UI URL: /Parts/Index?pageNumber=2&search=brake

1. CarPairs.Controllers.PartsController.Index()
   └─> Calls _partApiService.GetAllAsync()

2. CarPairs.Services.PartApiService.GetAllAsync()
   └─> Makes GET request to: api/parts?pageNumber=2&pageSize=10&search=brake

3. CarPairs.API.Controllers.PartsController.GetParts()
   └─> _service.GetAllAsync(2, 10, "brake", cancellationToken)

4. CarPairs.Core.Services.PartService.GetAllAsync()
   └─> Builds LINQ query with Include() for relationships
   └─> Filters by name: .Where(p => p.Name.Contains("brake"))
   └─> Counts total: .CountAsync()
   └─> Paginates: .Skip((2-1)*10).Take(10)
   └─> Returns PagedResult<Part>

5. API Controller maps Part → PartDto (includes manufacturer/category names)

6. Response: JSON object with TotalCount, PageNumber, PageSize, Data array

7. Web UI binds to View, renders HTML table
```

---

## Design Patterns

### 1. **Service Layer Pattern**
- **Location:** `CarPairs.Core.Services` and `CarPairs.Web.Services`
- **Purpose:** Encapsulates business logic separate from controllers
- **Example:** `PartService` contains CRUD operations, pagination, and search logic
- **Benefit:** Reusable, testable, improves separation of concerns

### 2. **Repository Pattern (Implicit via EF Core)**
- **Location:** `DbSet<T>` collections in `ApplicationDbContext`
- **Purpose:** Abstracts data access details
- **Benefit:** Clear data access interface, allows switching data sources

### 3. **Dependency Injection Pattern**
```csharp
// CarPairs.API/Program.cs
builder.Services.AddScoped<IPartService, PartService>();
builder.Services.AddScoped<IManufacturerService, ManufacturerService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

// Usage in Controller
public PartsController(IPartService service)
{
    _service = service;
}
```
- **Purpose:** Loose coupling, easy testing with mocks, centralized configuration
- **Scope:** Scoped (per HTTP request) for stateless services

### 4. **Data Transfer Object (DTO) Pattern**
- **Location:** `CarPairs.API/DTOs`
- **Purpose:** Defines API contracts independent of domain models
- **Example:** `PartDto` vs `Part` model
- **Benefit:** API can evolve independently; internal models can change without breaking clients

### 5. **Async/Await Pattern**
- **Applied To:** All data operations, HTTP requests
- **Example:** `Task<PagedResult<Part>> GetAllAsync(...)`
- **Benefit:** Non-blocking I/O, scalable under load

### 6. **Mapping Helper Methods**
```csharp
// In CarPairs.API/Controllers/PartsController.cs
private static PartDto MapToDto(Part p)
{
    return new PartDto { ... };
}
```
- **Purpose:** Centralized DTO conversion logic
- **Alternative:** Could use AutoMapper for more complex mappings

---

## Detailed Layer Descriptions

### Core Layer - In Depth

#### Domain Models (Entities)

**Part Model:**
```csharp
public class Part
{
    public int Id { get; set; }                      // Primary Key
    public string Name { get; set; }                 // Required, max 100 chars
    public decimal Price { get; set; }               // Decimal(18,2)
    public int StockQuantity { get; set; }           // Stock level
    public int ManufacturerId { get; set; }          // FK
    public int CategoryId { get; set; }              // FK
    public DateTime CreatedAt { get; set; }          // Timestamp
    public Manufacturer? Manufacturer { get; set; }  // Navigation property
    public Category? Category { get; set; }          // Navigation property
}
```

**Relationships:**
- `Part` belongs to `Manufacturer` (Many-to-One)
- `Part` belongs to `Category` (Many-to-One)

**Manufacturer Model:**
- Stores supplier information (name, country, website, founding year)
- `IsActive` flag for soft/hard delete capability

**Category Model:**
- Organizes parts hierarchically
- Supports parent-child relationships via `ParentCategoryId`

#### Service Implementation Pattern

**PartService.cs:**
```csharp
public class PartService : IPartService
{
    private readonly ApplicationDbContext _context;  // DI injected

    // Constructor Injection
    public PartService(ApplicationDbContext context)
    {
        _context = context;
    }

    // GetAll with Pagination & Search
    public async Task<PagedResult<Part>> GetAllAsync(
        int pageNumber,
        int pageSize,
        string? search,
        CancellationToken cancellationToken)
    {
        // Build queryable
        var query = _context.Parts
            .Include(p => p.Manufacturer)  // Eager load relationships
            .Include(p => p.Category)
            .AsQueryable();

        // Optional filtering
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => p.Name.Contains(search));
        }

        // Get total count for pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Get paginated data
        var items = await query
            .OrderBy(p => p.Name)
            .Skip((pageNumber - 1) * pageSize)  // 0-based offset
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Part>
        {
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            Data = items
        };
    }

    // Single item retrieval
    public async Task<Part?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Parts
            .Include(p => p.Manufacturer)
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    // Create new entity
    public async Task<int> CreateAsync(Part part, CancellationToken cancellationToken)
    {
        part.CreatedAt = DateTime.UtcNow;  // Always set server timestamp
        _context.Parts.Add(part);
        await _context.SaveChangesAsync(cancellationToken);
        return part.Id;  // Return generated ID
    }

    // Update existing entity
    public async Task<bool> UpdateAsync(Part part, CancellationToken cancellationToken)
    {
        // Check existence first
        if (!await _context.Parts.AnyAsync(p => p.Id == part.Id, cancellationToken))
            return false;

        _context.Parts.Update(part);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    // Delete by ID
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var part = await _context.Parts.FindAsync(
            new object[] { id }, 
            cancellationToken);
        
        if (part == null)
            return false;

        _context.Parts.Remove(part);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
```

**Key Patterns:**
- All methods are `async Task`
- `CancellationToken` parameter for graceful cancellation
- `Include()` for eager loading related entities
- LINQ for composable queries
- Exception-free boolean returns for update/delete operations

---

### API Layer - In Depth

#### Controller Implementation Pattern

**PartsController.cs:**
```csharp
[Route("api/[controller]")]      // /api/parts
[ApiController]
public class PartsController : ControllerBase
{
    private readonly IPartService _service;

    public PartsController(IPartService service)
    {
        _service = service;
    }

    // GET /api/parts?pageNumber=1&pageSize=10&search=
    [HttpGet]
    [AllowAnonymous]  // Until JWT implemented
    public async Task<ActionResult<PagedResult<PartDto>>> GetParts(
        int pageNumber = 1,
        int pageSize = 10,
        string? search = null,
        CancellationToken cancellationToken = default)
    {
        // Call service
        var result = await _service.GetAllAsync(
            pageNumber, 
            pageSize, 
            search, 
            cancellationToken);

        // Map domain models to DTOs
        var dto = new PagedResult<PartDto>
        {
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            Data = result.Data.Select(MapToDto).ToList()
        };

        return Ok(dto);  // 200 OK
    }

    // GET /api/parts/{id}
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<PartDto>> GetPart(
        int id, 
        CancellationToken cancellationToken)
    {
        var part = await _service.GetByIdAsync(id, cancellationToken);

        if (part == null)
            return NotFound();  // 404

        return Ok(MapToDto(part));
    }

    // POST /api/parts
    [HttpPost]
    public async Task<ActionResult> CreatePart(
        [FromBody] CreatePartDto dto,
        CancellationToken cancellationToken)
    {
        // Validate model state
        if (!ModelState.IsValid)
            return BadRequest(ModelState);  // 400

        // Map DTO to domain model
        var part = new Part
        {
            Name = dto.Name,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            ManufacturerId = dto.ManufacturerId,
            CategoryId = dto.CategoryId,
            CreatedAt = DateTime.Now
        };

        // Create via service
        var newId = await _service.CreateAsync(part, cancellationToken);

        // Return 201 with location header
        return CreatedAtAction(
            nameof(GetPart), 
            new { id = newId }, 
            new { id = newId });
    }

    // PUT /api/parts/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdatePart(
        int id,
        [FromBody] UpdatePartDto dto,
        CancellationToken cancellationToken)
    {
        // Validate ID consistency
        if (id != dto.Id)
            return BadRequest("Id mismatch.");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var part = new Part
        {
            Id = dto.Id,
            Name = dto.Name,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            ManufacturerId = dto.ManufacturerId,
            CategoryId = dto.CategoryId,
            CreatedAt = DateTime.Now
        };

        var updated = await _service.UpdateAsync(part, cancellationToken);

        if (!updated)
            return NotFound();  // 404

        return NoContent();  // 204
    }

    // DELETE /api/parts/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletePart(
        int id,
        CancellationToken cancellationToken)
    {
        var deleted = await _service.DeleteAsync(id, cancellationToken);

        if (!deleted)
            return NotFound();

        return NoContent();  // 204
    }

    // Helper method for DTO mapping
    private static PartDto MapToDto(Part p)
    {
        return new PartDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            StockQuantity = p.StockQuantity,
            ManufacturerName = p.Manufacturer?.Name ?? string.Empty,
            CategoryName = p.Category?.Name ?? string.Empty,
            CreatedAt = p.CreatedAt.ToString("dd-MM-yyyy HH:mm"),
            ManufacturerId = p.ManufacturerId,
            CategoryId = p.CategoryId
        };
    }
}
```

**HTTP Status Code Conventions:**
- `200 OK` - Successful GET request
- `201 Created` - Successful POST request with location header
- `204 No Content` - Successful PUT/DELETE with no response body
- `400 Bad Request` - Validation failure
- `404 Not Found` - Resource doesn't exist
- `500 Internal Server Error` - Unexpected server error

#### DTOs

**PartDto (Response):**
```csharp
public class PartDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string ManufacturerName { get; set; } = null!;    // Denormalized
    public string CategoryName { get; set; } = null!;        // Denormalized
    public string CreatedAt { get; set; } = null!;           // Formatted string
    public int ManufacturerId { get; set; }                  // For relationships
    public int CategoryId { get; set; }
}
```

**CreatePartDto (Request):**
```csharp
public class CreatePartDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;

    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }

    [Required]
    public int StockQuantity { get; set; }

    [Required]
    public int ManufacturerId { get; set; }

    [Required]
    public int CategoryId { get; set; }
}
```

**UpdatePartDto (Request):**
```csharp
public class UpdatePartDto
{
    [Required]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;

    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }

    // ... other properties
}
```

---

### Web UI Layer - In Depth

#### HTTP Client Service Pattern

**PartApiService.cs:**
```csharp
public class PartApiService : IPartApiService
{
    private readonly HttpClient _client;  // Injected by DependencyInjection

    public PartApiService(HttpClient client)
    {
        _client = client;  // Base URL preconfigured in Program.cs
    }

    public async Task<PagedResult<PartDto>?> GetAllAsync()
    {
        return await _client.GetFromJsonAsync<PagedResult<PartDto>>("api/parts");
    }

    public async Task<PartDto?> GetByIdAsync(int id)
    {
        return await _client.GetFromJsonAsync<PartDto>($"api/parts/{id}");
    }

    public async Task<bool> CreateAsync(CreatePartDto dto)
    {
        var response = await _client.PostAsJsonAsync("api/parts", dto);
        return response.IsSuccessStatusCode;  // Return boolean for simplicity
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var response = await _client.DeleteAsync($"api/parts/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateAsync(int id, UpdatePartDto dto)
    {
        var response = await _client.PutAsJsonAsync($"api/parts/{id}", dto);
        return response.IsSuccessStatusCode;
    }
}
```

**Key Points:**
- HttpClient is registered in DI with base URL from config
- Methods return actual data or boolean for simple success/failure
- Error handling happens via IsSuccessStatusCode check
- API communication is completely decoupled from UI logic

#### MVC Controller Pattern

**PartsController.cs (Web UI):**
```csharp
public class PartsController : Controller
{
    private readonly IPartApiService _service;
    private readonly ILookupApiService _lookupService;

    public PartsController(
        IPartApiService service,
        ILookupApiService lookupService)
    {
        _service = service;
        _lookupService = lookupService;
    }

    // GET /Parts/Index
    public async Task<IActionResult> Index()
    {
        var result = await _service.GetAllAsync();
        return View(result?.Data ?? new List<PartDto>());
    }

    // GET /Parts/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var part = await _service.GetByIdAsync(id);
        if (part == null)
            return NotFound();

        return View(part);
    }

    // GET /Parts/Create
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await FillManufacturersAndCategories();
        return View();
    }

    // POST /Parts/Create
    [HttpPost]
    public async Task<IActionResult> Create(CreatePartDto dto)
    {
        if (!ModelState.IsValid)
        {
            await FillManufacturersAndCategories(
                dto.ManufacturerId, 
                dto.CategoryId);
            return View(dto);
        }

        var success = await _service.CreateAsync(dto);

        if (!success)
        {
            await FillManufacturersAndCategories(
                dto.ManufacturerId, 
                dto.CategoryId);
            return View(dto);
        }

        return RedirectToAction(nameof(Index));  // Successful redirect
    }

    // GET /Parts/Edit/5
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var part = await _service.GetByIdAsync(id);

        if (part == null)
            return NotFound();

        var dto = new UpdatePartDto
        {
            Id = part.Id,
            Name = part.Name,
            Price = part.Price,
            StockQuantity = part.StockQuantity,
            ManufacturerId = part.ManufacturerId,
            CategoryId = part.CategoryId
        };

        await FillManufacturersAndCategories(
            dto.ManufacturerId, 
            dto.CategoryId);

        return View(dto);
    }

    // POST /Parts/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdatePartDto dto)
    {
        if (id != dto.Id)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            await FillManufacturersAndCategories(
                dto.ManufacturerId, 
                dto.CategoryId);
            return View(dto);
        }

        var success = await _service.UpdateAsync(id, dto);

        if (!success)
        {
            await FillManufacturersAndCategories(
                dto.ManufacturerId, 
                dto.CategoryId);
            return View(dto);
        }

        return RedirectToAction(nameof(Index));
    }

    // GET /Parts/Delete/5
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var part = await _service.GetByIdAsync(id);

        if (part == null)
            return NotFound();

        return View(part);
    }

    // POST /Parts/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var success = await _service.DeleteAsync(id);

        if (!success)
            return NotFound();

        return RedirectToAction(nameof(Index));
    }

    // Helper to populate dropdowns
    private async Task FillManufacturersAndCategories(
        int? selectedManufacturerId = null,
        int? selectedCategoryId = null)
    {
        var manufacturers = await _lookupService.GetManufacturersAsync() 
            ?? new();
        var categories = await _lookupService.GetCategoriesAsync() 
            ?? new();

        ViewData["Manufacturers"] = new SelectList(
            manufacturers,
            "Id",
            "Name",
            selectedManufacturerId);

        ViewData["Categories"] = new SelectList(
            categories,
            "Id",
            "Name",
            selectedCategoryId);
    }
}
```

---

## Implementing New Features

### Step-by-Step Checklist for Adding a New Feature (e.g., "Warranty")

Follow this process to add a new feature maintaining consistency with existing architecture.

#### 1. Create Domain Model in CarPairs.Core

**File:** `CarPairs.Core/Data/Models/Warranty.cs`

```csharp
using System.ComponentModel.DataAnnotations;

namespace CarPairs.Core
{
    public class Warranty
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public int WarrantyMonths { get; set; }

        public decimal? CoveragePercentage { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsActive { get; set; }

        // Navigation properties (if Warranty has Parts)
        // public ICollection<Part> Parts { get; set; } = new List<Part>();
    }
}
```

**Relationships to consider:**
- Does it have a one-to-many relationship with Part?
- Should it be soft-delete (IsActive flag)?
- What timestamp fields are needed?

---

#### 2. Create Service Interface in CarPairs.Core

**File:** `CarPairs.Core/Services/Interfaces/IWarrantyService.cs`

```csharp
namespace CarPairs.Core.Services.Interfaces
{
    public interface IWarrantyService
    {
        Task<PagedResult<Warranty>> GetAllAsync(
            int pageNumber,
            int pageSize,
            string? search,
            CancellationToken cancellationToken);

        Task<Warranty?> GetByIdAsync(int id, CancellationToken cancellationToken);

        Task<int> CreateAsync(Warranty warranty, CancellationToken cancellationToken);

        Task<bool> UpdateAsync(Warranty warranty, CancellationToken cancellationToken);

        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);

        Task<List<SimpleLookupDto>> GetLookupAsync(CancellationToken cancellationToken);
    }
}
```

---

#### 3. Implement Service in CarPairs.Core

**File:** `CarPairs.Core/Services/WarrantyService.cs`

```csharp
using CarPairs.Core.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarPairs.Core.Services
{
    public class WarrantyService : IWarrantyService
    {
        private readonly ApplicationDbContext _context;

        public WarrantyService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Warranty>> GetAllAsync(
            int pageNumber,
            int pageSize,
            string? search,
            CancellationToken cancellationToken)
        {
            var query = _context.Warranties.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(w => w.Name.Contains(search));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(w => w.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<Warranty>
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Data = items
            };
        }

        public async Task<Warranty?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Warranties.FirstOrDefaultAsync(
                w => w.Id == id, 
                cancellationToken);
        }

        public async Task<int> CreateAsync(Warranty warranty, CancellationToken cancellationToken)
        {
            warranty.CreatedAt = DateTime.UtcNow;
            _context.Warranties.Add(warranty);
            await _context.SaveChangesAsync(cancellationToken);
            return warranty.Id;
        }

        public async Task<bool> UpdateAsync(Warranty warranty, CancellationToken cancellationToken)
        {
            if (!await _context.Warranties.AnyAsync(w => w.Id == warranty.Id, cancellationToken))
                return false;

            _context.Warranties.Update(warranty);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var warranty = await _context.Warranties.FindAsync(
                new object[] { id }, 
                cancellationToken);
            
            if (warranty == null)
                return false;

            _context.Warranties.Remove(warranty);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<List<SimpleLookupDto>> GetLookupAsync(CancellationToken cancellationToken)
        {
            return await _context.Warranties
                .Where(w => w.IsActive)
                .OrderBy(w => w.Name)
                .Select(w => new SimpleLookupDto
                {
                    Id = w.Id,
                    Name = w.Name
                })
                .ToListAsync(cancellationToken);
        }
    }
}
```

---

#### 4. Update DbContext in CarPairs.Core

**File:** `CarPairs.Core/Data/ApplicationDbContext.cs`

Add to the DbContext class:

```csharp
public DbSet<Warranty> Warranties { get; set; }

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // ... existing seed data ...

    // Seed Warranties
    modelBuilder.Entity<Warranty>().HasData(
        new Warranty 
        { 
            Id = 1, 
            Name = "Basic", 
            WarrantyMonths = 12, 
            CoveragePercentage = 50m,
            IsActive = true 
        },
        new Warranty 
        { 
            Id = 2, 
            Name = "Extended", 
            WarrantyMonths = 24, 
            CoveragePercentage = 75m,
            IsActive = true 
        }
    );
}
```

---

#### 5. Create DTOs in CarPairs.API

**File:** `CarPairs.API/DTOs/Warranties/WarrantyDto.cs`

```csharp
using System.ComponentModel.DataAnnotations;

namespace CarPairs.API.DTOs.Warranties
{
    public class WarrantyDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
        public int WarrantyMonths { get; set; }
        public decimal? CoveragePercentage { get; set; }
        public string CreatedAt { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
```

**File:** `CarPairs.API/DTOs/Warranties/CreateWarrantyDto.cs`

```csharp
using System.ComponentModel.DataAnnotations;

namespace CarPairs.API.DTOs.Warranties
{
    public class CreateWarrantyDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(1, 120)]
        public int WarrantyMonths { get; set; }

        [Range(0, 100)]
        public decimal? CoveragePercentage { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
```

**File:** `CarPairs.API/DTOs/Warranties/UpdateWarrantyDto.cs`

```csharp
using System.ComponentModel.DataAnnotations;

namespace CarPairs.API.DTOs.Warranties
{
    public class UpdateWarrantyDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(1, 120)]
        public int WarrantyMonths { get; set; }

        [Range(0, 100)]
        public decimal? CoveragePercentage { get; set; }

        public bool IsActive { get; set; }
    }
}
```

---

#### 6. Create API Controller in CarPairs.API

**File:** `CarPairs.API/Controllers/WarrantiesController.cs`

```csharp
using CarPairs.API.DTOs.Warranties;
using CarPairs.Core;
using CarPairs.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CarPairs.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarrantiesController : ControllerBase
    {
        private readonly IWarrantyService _service;

        public WarrantiesController(IWarrantyService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<WarrantyDto>>> GetWarranties(
            int pageNumber = 1,
            int pageSize = 10,
            string? search = null,
            CancellationToken cancellationToken = default)
        {
            var result = await _service.GetAllAsync(
                pageNumber, 
                pageSize, 
                search, 
                cancellationToken);

            var dto = new PagedResult<WarrantyDto>
            {
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                Data = result.Data.Select(MapToDto).ToList()
            };

            return Ok(dto);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<WarrantyDto>> GetWarranty(
            int id, 
            CancellationToken cancellationToken)
        {
            var warranty = await _service.GetByIdAsync(id, cancellationToken);

            if (warranty == null)
                return NotFound();

            return Ok(MapToDto(warranty));
        }

        [HttpPost]
        public async Task<ActionResult> CreateWarranty(
            [FromBody] CreateWarrantyDto dto,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var warranty = new Warranty
            {
                Name = dto.Name,
                Description = dto.Description,
                WarrantyMonths = dto.WarrantyMonths,
                CoveragePercentage = dto.CoveragePercentage,
                IsActive = dto.IsActive
            };

            var newId = await _service.CreateAsync(warranty, cancellationToken);

            return CreatedAtAction(
                nameof(GetWarranty), 
                new { id = newId }, 
                new { id = newId });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateWarranty(
            int id,
            [FromBody] UpdateWarrantyDto dto,
            CancellationToken cancellationToken)
        {
            if (id != dto.Id)
                return BadRequest("Id mismatch.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var warranty = new Warranty
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                WarrantyMonths = dto.WarrantyMonths,
                CoveragePercentage = dto.CoveragePercentage,
                IsActive = dto.IsActive
            };

            var updated = await _service.UpdateAsync(warranty, cancellationToken);

            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteWarranty(
            int id,
            CancellationToken cancellationToken)
        {
            var deleted = await _service.DeleteAsync(id, cancellationToken);

            if (!deleted)
                return NotFound();

            return NoContent();
        }

        [HttpGet("lookup")]
        public async Task<ActionResult<IEnumerable<SimpleLookupDto>>> GetLookup(
            CancellationToken cancellationToken)
        {
            var result = await _service.GetLookupAsync(cancellationToken);
            return Ok(result);
        }

        private static WarrantyDto MapToDto(Warranty w)
        {
            return new WarrantyDto
            {
                Id = w.Id,
                Name = w.Name,
                Description = w.Description,
                WarrantyMonths = w.WarrantyMonths,
                CoveragePercentage = w.CoveragePercentage,
                CreatedAt = w.CreatedAt.ToString("dd-MM-yyyy HH:mm"),
                IsActive = w.IsActive
            };
        }
    }
}
```

---

#### 7. Register Service in CarPairs.API Program.cs

**File:** `CarPairs.API/Program.cs`

Add to dependency injection section:

```csharp
builder.Services.AddScoped<IWarrantyService, WarrantyService>();
```

---

#### 8. Create Web UI Service in CarPairs

**File:** `CarPairs/Services/WarrantyApiService.cs`

```csharp
using CarPairs.API.DTOs.Warranties;
using CarPairs.Core;
using CarPairs.Web.Services.Interfaces;

namespace CarPairs.Web.Services
{
    public class WarrantyApiService : IWarrantyApiService
    {
        private readonly HttpClient _client;

        public WarrantyApiService(HttpClient client)
        {
            _client = client;
        }

        public async Task<PagedResult<WarrantyDto>?> GetAllAsync()
        {
            return await _client.GetFromJsonAsync<PagedResult<WarrantyDto>>("api/warranties");
        }

        public async Task<WarrantyDto?> GetByIdAsync(int id)
        {
            return await _client.GetFromJsonAsync<WarrantyDto>($"api/warranties/{id}");
        }

        public async Task<bool> CreateAsync(CreateWarrantyDto dto)
        {
            var response = await _client.PostAsJsonAsync("api/warranties", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(int id, UpdateWarrantyDto dto)
        {
            var response = await _client.PutAsJsonAsync($"api/warranties/{id}", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _client.DeleteAsync($"api/warranties/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<List<SimpleLookupDto>?> GetLookupAsync()
        {
            return await _client.GetFromJsonAsync<List<SimpleLookupDto>>("api/warranties/lookup");
        }
    }
}
```

**File:** `CarPairs/Services/Interfaces/IWarrantyApiService.cs`

```csharp
using CarPairs.API.DTOs.Warranties;
using CarPairs.Core;

namespace CarPairs.Web.Services.Interfaces
{
    public interface IWarrantyApiService
    {
        Task<PagedResult<WarrantyDto>?> GetAllAsync();
        Task<WarrantyDto?> GetByIdAsync(int id);
        Task<bool> CreateAsync(CreateWarrantyDto dto);
        Task<bool> UpdateAsync(int id, UpdateWarrantyDto dto);
        Task<bool> DeleteAsync(int id);
        Task<List<SimpleLookupDto>?> GetLookupAsync();
    }
}
```

---

#### 9. Register Web UI Service in CarPairs Program.cs

**File:** `CarPairs/Extensions/ServiceCollectionExtensions.cs`

Add to `AddApiClients` method:

```csharp
services.AddHttpClient<IWarrantyApiService, WarrantyApiService>(client =>
{
    client.BaseAddress = baseUri;
});
```

---

#### 10. Create Web UI Controller in CarPairs

**File:** `CarPairs/Controllers/WarrantiesController.cs`

```csharp
using CarPairs.API.DTOs.Warranties;
using CarPairs.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CarPairs.Controllers
{
    public class WarrantiesController : Controller
    {
        private readonly IWarrantyApiService _service;

        public WarrantiesController(IWarrantyApiService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _service.GetAllAsync();
            return View(result?.Data ?? new List<WarrantyDto>());
        }

        public async Task<IActionResult> Details(int id)
        {
            var warranty = await _service.GetByIdAsync(id);
            if (warranty == null)
                return NotFound();

            return View(warranty);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateWarrantyDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var success = await _service.CreateAsync(dto);

            if (!success)
                return View(dto);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var warranty = await _service.GetByIdAsync(id);

            if (warranty == null)
                return NotFound();

            var dto = new UpdateWarrantyDto
            {
                Id = warranty.Id,
                Name = warranty.Name,
                Description = warranty.Description,
                WarrantyMonths = warranty.WarrantyMonths,
                CoveragePercentage = warranty.CoveragePercentage,
                IsActive = warranty.IsActive
            };

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateWarrantyDto dto)
        {
            if (id != dto.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(dto);

            var success = await _service.UpdateAsync(id, dto);

            if (!success)
                return View(dto);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var warranty = await _service.GetByIdAsync(id);

            if (warranty == null)
                return NotFound();

            return View(warranty);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _service.DeleteAsync(id);

            if (!success)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }
    }
}
```

---

#### 11. Create Database Migration

In Package Manager Console or terminal (in CarPairs.API or CarPairs.Core directory):

```powershell
Add-Migration AddWarrantiesTable
Update-Database
```

Or using CLI:

```bash
dotnet ef migrations add AddWarrantiesTable --project CarPairs.Core
dotnet ef database update --project CarPairs.API
```

---

#### 12. Create Razor Views (Optional but recommended)

Create views in `CarPairs/Views/Warranties/` directory following the same pattern as Parts:
- `Index.cshtml` - List all warranties with pagination
- `Details.cshtml` - Show warranty details
- `Create.cshtml` - Form to create warranty
- `Edit.cshtml` - Form to edit warranty
- `Delete.cshtml` - Confirmation to delete warranty

---

### Implementation Checklist

When adding a new feature, follow this checklist:

- [ ] **Create Domain Model** in `CarPairs.Core/Data/Models/`
- [ ] **Create Service Interface** in `CarPairs.Core/Services/Interfaces/`
- [ ] **Implement Service** in `CarPairs.Core/Services/`
- [ ] **Add DbSet** to `ApplicationDbContext` and configure seeding
- [ ] **Create DTOs** in `CarPairs.API/DTOs/[FeatureName]/`
- [ ] **Create API Controller** in `CarPairs.API/Controllers/`
- [ ] **Register Service** in `CarPairs.API/Program.cs`
- [ ] **Create Web Service Interface** in `CarPairs/Services/Interfaces/`
- [ ] **Create Web Service Implementation** in `CarPairs/Services/`
- [ ] **Register Web Service** in `CarPairs/Extensions/ServiceCollectionExtensions.cs`
- [ ] **Create Web Controller** in `CarPairs/Controllers/`
- [ ] **Create Razor Views** in `CarPairs/Views/[FeatureName]/`
- [ ] **Create Database Migration** and update database
- [ ] **Run and test** all CRUD operations

---

## Code Examples and Patterns

### Pattern: Pagination

**Usage in Service:**
```csharp
public async Task<PagedResult<T>> GetAllAsync(
    int pageNumber = 1,
    int pageSize = 10,
    string? search = null,
    CancellationToken cancellationToken = default)
{
    // 1. Build query
    var query = _context.Items.AsQueryable();

    // 2. Apply filters
    if (!string.IsNullOrWhiteSpace(search))
    {
        query = query.Where(x => x.Name.Contains(search));
    }

    // 3. Count total before pagination
    var totalCount = await query.CountAsync(cancellationToken);

    // 4. Apply pagination
    var items = await query
        .OrderBy(x => x.Name)
        .Skip((pageNumber - 1) * pageSize)  // Offset = (page - 1) * size
        .Take(pageSize)
        .ToListAsync(cancellationToken);

    // 5. Return wrapped result
    return new PagedResult<T>
    {
        TotalCount = totalCount,
        PageNumber = pageNumber,
        PageSize = pageSize,
        Data = items
    };
}
```

**Usage in Controller:**
```csharp
[HttpGet]
public async Task<ActionResult<PagedResult<ItemDto>>> GetItems(
    int pageNumber = 1,
    int pageSize = 10,
    string? search = null,
    CancellationToken cancellationToken = default)
{
    var result = await _service.GetAllAsync(
        pageNumber, 
        pageSize, 
        search, 
        cancellationToken);

    return Ok(result);
}
```

---

### Pattern: Relationship Eager Loading

**Problem:** N+1 query issue when accessing related entities

**Solution:** Use `Include()` to explicitly load relationships

```csharp
// ❌ BAD: Multiple queries
var part = await _context.Parts.FirstOrDefaultAsync(p => p.Id == id);
var manufacturer = await _context.Manufacturers.FirstOrDefaultAsync(m => m.Id == part.ManufacturerId);

// ✅ GOOD: Single query with eager loading
var part = await _context.Parts
    .Include(p => p.Manufacturer)
    .Include(p => p.Category)
    .FirstOrDefaultAsync(p => p.Id == id);
```

---

### Pattern: Soft Delete

**Pattern:** Use boolean flag instead of hard delete

**Model:**
```csharp
public class Item
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;  // Soft delete flag
}
```

**Service - Soft Delete:**
```csharp
public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
{
    var item = await _context.Items.FindAsync(new object[] { id }, cancellationToken);
    if (item == null)
        return false;

    item.IsActive = false;  // Mark as deleted, don't remove
    _context.Items.Update(item);
    await _context.SaveChangesAsync(cancellationToken);
    return true;
}
```

**Service - Filter Active Items:**
```csharp
public async Task<List<Item>> GetActiveAsync(CancellationToken cancellationToken)
{
    return await _context.Items
        .Where(i => i.IsActive)  // Only active items
        .ToListAsync(cancellationToken);
}
```

---

### Pattern: DTO Mapping with Helper Methods

**Simple Manual Mapping:**
```csharp
private static ItemDto MapToDto(Item item)
{
    return new ItemDto
    {
        Id = item.Id,
        Name = item.Name,
        Description = item.Description,
        CreatedAt = item.CreatedAt.ToString("dd-MM-yyyy"),
        RelatedName = item.RelatedEntity?.Name ?? string.Empty
    };
}

// Usage
var dtos = items.Select(MapToDto).ToList();
```

---

### Pattern: Validation in DTOs

**Request Validation:**
```csharp
public class CreateItemDto
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 3, 
        ErrorMessage = "Name must be between 3 and 100 characters")]
    public string Name { get; set; } = null!;

    [Range(0.01, double.MaxValue, 
        ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Category must be selected")]
    public int CategoryId { get; set; }
}
```

**Controller Validation Check:**
```csharp
[HttpPost]
public async Task<ActionResult> Create([FromBody] CreateItemDto dto, CancellationToken cancellationToken)
{
    // ModelState contains all validation errors from annotations
    if (!ModelState.IsValid)
        return BadRequest(ModelState);  // Returns errors to client

    // Proceed with creation...
}
```

---

### Pattern: Exception Handling

**Current Approach:** Return boolean or null for errors

**Alternative: Structured Logging**
```csharp
try
{
    var result = await _service.CreateAsync(item, cancellationToken);
    return CreatedAtAction(nameof(GetItem), new { id = result }, result);
}
catch (DbUpdateException ex)
{
    _logger.LogError(ex, "Database error creating item");
    return StatusCode(500, "An error occurred while creating the item");
}
catch (Exception ex)
{
    _logger.LogError(ex, "Unexpected error creating item");
    return StatusCode(500, "An unexpected error occurred");
}
```

---

## Best Practices

### 1. **Always Use Async/Await**

```csharp
// ❌ AVOID: Blocking operations
public void GetItems()
{
    var items = _context.Items.ToList();
}

// ✅ DO: Async operations
public async Task<List<Item>> GetItemsAsync(CancellationToken cancellationToken)
{
    return await _context.Items.ToListAsync(cancellationToken);
}
```

### 2. **Include CancellationToken Parameter**

Allows graceful shutdown and request cancellation:

```csharp
// ✅ GOOD
public async Task<Item?> GetByIdAsync(int id, CancellationToken cancellationToken)
{
    return await _context.Items.FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
}
```

### 3. **Use DTOs for API Responses**

Decouples contracts from internal models:

```csharp
// ❌ AVOID: Exposing domain models directly
[HttpGet("{id}")]
public async Task<Item> GetItem(int id)
{
    return await _service.GetByIdAsync(id);
}

// ✅ DO: Return DTOs
[HttpGet("{id}")]
public async Task<ActionResult<ItemDto>> GetItem(int id, CancellationToken cancellationToken)
{
    var item = await _service.GetByIdAsync(id, cancellationToken);
    if (item == null)
        return NotFound();

    return Ok(MapToDto(item));
}
```

### 4. **Implement Proper HTTP Status Codes**

```csharp
[HttpPost]
public async Task<ActionResult> Create([FromBody] CreateItemDto dto, ...)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);           // 400

    var id = await _service.CreateAsync(...);
    return CreatedAtAction(nameof(GetItem),     // 201
        new { id = id },
        new { id = id });
}

[HttpPut("{id}")]
public async Task<IActionResult> Update(int id, UpdateItemDto dto, ...)
{
    var success = await _service.UpdateAsync(...);
    if (!success)
        return NotFound();                      // 404

    return NoContent();                          // 204
}
```

### 5. **Eager Load Related Entities**

Prevent N+1 query problems:

```csharp
// ❌ AVOID: Lazy loading leads to multiple queries
var items = await _context.Items.ToListAsync();
foreach (var item in items)
{
    var category = item.Category;  // Additional query per item!
}

// ✅ DO: Eager load with Include
var items = await _context.Items
    .Include(i => i.Category)
    .ToListAsync();
```

### 6. **Use Meaningful Service Methods**

```csharp
// ❌ AVOID: Generic method names
public async Task<List<Item>> GetAsync()

// ✅ DO: Specific, self-documenting names
public async Task<List<SimpleLookupDto>> GetLookupAsync(CancellationToken cancellationToken)
public async Task<PagedResult<Item>> GetAllAsync(int pageNumber, int pageSize, string? search, CancellationToken cancellationToken)
```

### 7. **Set Server-Controlled Fields in Service**

```csharp
public async Task<int> CreateAsync(Item item, CancellationToken cancellationToken)
{
    // Always set server timestamp, never trust client
    item.CreatedAt = DateTime.UtcNow;
    
    _context.Items.Add(item);
    await _context.SaveChangesAsync(cancellationToken);
    return item.Id;
}
```

### 8. **Validate Before Database Operations**

```csharp
[HttpPut("{id}")]
public async Task<IActionResult> Update(int id, UpdateItemDto dto, ...)
{
    // Validate ID consistency
    if (id != dto.Id)
        return BadRequest("ID mismatch");

    // Validate model state
    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    // Map and update
    var success = await _service.UpdateAsync(...);
    ...
}
```

### 9. **Use CancellationToken Throughout**

```csharp
// From Controller
[HttpGet]
public async Task<ActionResult<List<ItemDto>>> GetItems(CancellationToken cancellationToken)
{
    // Pass token through call chain
    var items = await _service.GetAllAsync(cancellationToken);
    return Ok(items);
}

// In Service
public async Task<List<Item>> GetAllAsync(CancellationToken cancellationToken)
{
    // Use token in all async operations
    return await _context.Items
        .ToListAsync(cancellationToken);
}
```

### 10. **Implement Lookup Methods for Dropdown Lists**

```csharp
// Service
public async Task<List<SimpleLookupDto>> GetLookupAsync(CancellationToken cancellationToken)
{
    return await _context.Items
        .Where(i => i.IsActive)  // Only active items
        .OrderBy(i => i.Name)
        .Select(i => new SimpleLookupDto { Id = i.Id, Name = i.Name })
        .ToListAsync(cancellationToken);
}

// API Controller
[HttpGet("lookup")]
public async Task<ActionResult<List<SimpleLookupDto>>> GetLookup(CancellationToken cancellationToken)
{
    var result = await _service.GetLookupAsync(cancellationToken);
    return Ok(result);
}

// Web Controller
private async Task FillCategories()
{
    var categories = await _lookupService.GetCategoriesAsync();
    ViewData["Categories"] = new SelectList(categories, "Id", "Name");
}
```

---

## Configuration Files

### appsettings.Development.json (CarPairs.API)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=CarPairsDb;Trusted_Connection=true;Encrypt=false;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.EntityFrameworkCore": "Debug"
    }
  }
}
```

### appsettings.json (CarPairs Web UI)
```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:7001"  // CarPairs.API base URL
  }
}
```

---

## Summary

**CarPairs** follows a clean, layered architecture with:

1. **CarPairs.Core** - Pure business logic, database access, domain models
2. **CarPairs.API** - REST API endpoints with DTOs
3. **CarPairs** - MVC web UI consuming the API via HTTP clients

**Key Principles:**
- Separation of concerns with clear layer boundaries
- Dependency injection for loose coupling
- Async/await for scalability
- DTOs for API contracts
- Service layer for business logic
- Pagination and lookup patterns for common scenarios

**To add new features:** Follow the 12-step checklist which ensures consistency with existing code patterns.

This architecture enables multiple maintenance teams to work independently without conflicts, ensures scalability, and maintains clean code practices suitable for enterprise applications.

