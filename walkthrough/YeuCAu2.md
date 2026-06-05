````md
# AI_ENGINEERING_GUIDE.md

# AutoHub AI Engineering Guide

## 1. Project Overview

### Project Name
AutoHub

### Project Description
AutoHub is a scalable vehicle ecommerce and garage management platform.

The system supports:
- Vehicle sales
- Spare parts management
- Vehicle service management
- Brand management
- Customer management
- Orders and transactions
- Vehicle accessories and upgrade products

Future expansion:
- AI integration
- Mobile application
- Real-time analytics
- Map/location services
- Multi-branch garage support
- API-first architecture

---

## 2. Current Technical Stack

### Backend
- ASP.NET Core MVC
- C#
- Entity Framework Core
- SQL Server

### Frontend
- Razor Views
- Bootstrap 5
- JavaScript
- AJAX

### Development Tools
- VS Code
- Git & GitHub
- Docker (future integration)

---

## 3. Current Project Phase

Current focus:
- Admin dashboard development
- CRUD modules
- Database architecture
- UI refactoring
- Business logic refinement

Completed modules:
- Vehicles
- Brands
- Services
- Database schema
- Sidebar navigation
- Admin dashboard layout

In progress:
- Shared layouts
- Partial views
- AJAX CRUD
- Validation improvements
- Role-based architecture preparation

Planned:
- Authentication & Authorization
- Repository-Service pattern
- Clean Architecture migration
- API integration
- Mobile support
- Real-time notifications
- Reporting & analytics

---

## 4. Core Development Philosophy

The project must follow:
- Clean Code
- SOLID Principles
- Maintainable Architecture
- Enterprise-level structure
- Real-world engineering practices

Code should prioritize:
- readability
- scalability
- maintainability
- modularity
- defensive programming

Avoid tutorial-style code.

Generated code should resemble production-ready enterprise code.

---

## 5. SOLID Principles Enforcement

### Single Responsibility Principle (SRP)
- Every class must have only one responsibility.
- Avoid God classes and God methods.

### Open/Closed Principle (OCP)
- Prefer extensible architecture.
- Use polymorphism, abstraction, interfaces, and strategy patterns when applicable.
- Avoid excessive switch-case and if-else chains.

### Liskov Substitution Principle (LSP)
- Child classes must properly replace parent classes without breaking behavior.

### Interface Segregation Principle (ISP)
- Keep interfaces small and focused.
- Do not force classes to implement unused methods.

### Dependency Inversion Principle (DIP)
- Depend on abstractions instead of concrete implementations.
- Use Dependency Injection properly.

---

## 6. Coding Standards

### Naming Conventions
- Use descriptive and meaningful names.
- Avoid abbreviations.
- Follow official C# naming conventions.
- Use PascalCase for classes and methods.
- Use camelCase for local variables and parameters.

Bad Examples:
- d
- data1
- obj
- temp

Good Examples:
- vehicleOrder
- serviceRequest
- customerAddress
- brandRepository

---

## 7. Code Style Requirements

### Required
- Self-documenting code
- Small focused methods
- Clear separation of concerns
- Reusable components
- Proper validation
- Proper exception handling
- Consistent folder structure

### Forbidden
- Massive controllers
- Massive services
- Deep nested logic
- Duplicated Razor layouts
- Hardcoded business rules
- Static helper abuse
- Repeated database queries
- Tight coupling
- Spaghetti code

---

## 8. Error Handling & Validation

Always apply defensive programming.

### Required Validation
- null checks
- empty string checks
- invalid format checks
- duplicate data checks
- invalid business logic checks

### Exception Handling
- Use explicit try-catch blocks when necessary.
- Never swallow exceptions silently.
- Throw meaningful exceptions.
- Return consistent validation responses.

### Edge Cases
Always consider:
- zero quantity
- negative values
- duplicate records
- invalid relationships
- missing foreign keys
- concurrency conflicts

---

## 9. Database Design Philosophy

Database design must prioritize:
- normalization
- scalability
- maintainability
- reporting capability
- future extensibility

### Database Rules
- Use soft delete
- Avoid duplicated data
- Prefer normalized relationships
- Use proper foreign keys
- Use indexes where appropriate
- Design for future API migration

### Current Database Modules
- Countries
- Brands
- Vehicles
- VehicleColors
- SpareParts
- Services
- Users
- Orders
- OrderDetails

---

## 10. UI/UX Philosophy

The UI should resemble a modern enterprise admin dashboard.

### UI Requirements
- Responsive layout
- Reusable modal components
- Reusable partial views
- Clean spacing
- Consistent typography
- Professional admin dashboard appearance

### UX Requirements
- Minimize page reloads
- Prefer AJAX for CRUD interactions
- Clear validation feedback
- Smooth user flow
- Easy navigation

---

## 11. Architecture Direction

Current architecture:
- Traditional ASP.NET Core MVC

Future architecture direction:
- Repository-Service Pattern
- Clean Architecture
- Modular architecture
- API-first structure
- Reusable UI components
- Scalable backend services

AI should improve architecture gradually without rewriting the entire project unnecessarily.

Avoid overengineering.

---

## 12. Folder Structure Expectations

Preferred structure:

```text
AutoHub/
│
├── Controllers/
├── Models/
├── Views/
│   ├── Shared/
│   ├── Vehicle/
│   ├── Brand/
│   └── Service/
│
├── Services/
├── Repositories/
├── Data/
├── DTOs/
├── Validators/
├── Middleware/
├── Interfaces/
├── Configurations/
├── Utilities/
├── wwwroot/
│
└── AI_ENGINEERING_GUIDE.md
```

---

## 13. AI Response Instructions

When generating code:
- prioritize maintainability
- prioritize scalability
- prioritize readability
- avoid unnecessary complexity
- avoid rewriting unrelated modules
- preserve current architecture direction
- improve code incrementally

When suggesting improvements:
- explain architectural tradeoffs briefly
- mention scalability concerns
- mention performance considerations
- mention security concerns
- mention maintainability concerns

---

## 14. Output Requirements

### Required
- Pure code blocks only
- Clean formatting
- Clear structure
- Production-style implementation

### Forbidden
- No code comments
- No tutorial explanations
- No unnecessary markdown explanations
- No pseudo code

If new folders or files are created:
- provide folder tree structure

---

## 15. Long-Term Engineering Goals

The project aims to:
- simulate real-world enterprise software
- improve software engineering skills
- demonstrate scalable architecture
- support future business expansion
- become portfolio-quality software

The codebase should evolve gradually toward enterprise-level architecture without unnecessary complexity.
````
