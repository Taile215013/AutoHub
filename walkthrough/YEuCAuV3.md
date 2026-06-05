# AI_ENGINEERING_GUIDE.md

# AutoHub AI Guide

## 1. Project Overview

### Project Name
AutoHub

### Description
AutoHub is a vehicle management and ecommerce web application.

Main features:
- Vehicle management
- Brand management
- Vehicle services
- Spare parts management
- Customer management
- Order management

This project is mainly built for:
- learning ASP.NET Core MVC
- practicing backend development
- improving database design skills
- improving clean coding skills

---

## 2. Current Technology Stack

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

### Tools
- VS Code
- GitHub
- SSMS

---

## 3. Current Project Goal

The project should:
- look clean and organized
- have good coding structure
- use understandable logic
- avoid duplicated code
- be easy to explain during presentation

Avoid overly complex enterprise architecture.

---

## 4. Coding Style

### Required
- Clean and readable code
- Clear naming
- Small focused methods
- Organized folder structure
- Reusable components when reasonable
- Easy-to-understand logic

### Avoid
- Overengineering
- Complex abstraction
- Advanced enterprise patterns
- Unnecessary interfaces
- Deep nested logic
- Massive controllers
- Repeated code

---

## 5. Naming Rules

Use meaningful names.

### Good Examples
- vehicleService
- brandRepository
- customerOrder
- vehiclePrice

### Bad Examples
- data1
- temp
- obj
- d

Follow standard C# naming conventions:
- PascalCase for classes and methods
- camelCase for local variables and parameters

---

## 6. Logic & Validation

Always validate:
- null values
- empty strings
- duplicate data
- invalid input
- negative values
- invalid relationships

Business logic should:
- be simple
- be easy to explain
- avoid unnecessary complexity

---

## 7. Error Handling

Use proper try-catch blocks when necessary.

Do not:
- ignore exceptions
- leave empty catch blocks

Return meaningful validation messages.

---

## 8. Database Design Philosophy

Database should:
- avoid duplicated data
- use proper relationships
- use foreign keys correctly
- support future improvements

Current main tables:
- Countries
- Brands
- Vehicles
- Services
- SpareParts
- Orders
- Users

Use soft delete when appropriate.

---

## 9. UI/UX Direction

The admin dashboard should:
- look clean
- be responsive
- be easy to navigate
- no duplicated UI code

-CSS should be clear, concise, simple, and easy to maintain, avoiding overlapping elements.

Prefer:
- reusable partial views
- reusable modals
- AJAX for simple CRUD actions



## 11. AI Instructions

When generating code:
- prioritize readability
- prioritize maintainability
- keep logic simple and understandable
- avoid unnecessary complexity
- preserve current project structure
- improve code gradually

Generated code should:
- look realistic for a student project
- avoid overly advanced architecture
- remain easy to explain during presentation

---

## 12. Output Requirements

### Required
- Pure code blocks
- Clean formatting
- Clear structure

### Forbidden
- have code comments
- No unnecessary explanations
- No overly advanced architecture