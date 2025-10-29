

Blog Management Application (.NET 8)

Project Overview

A single-project ASP.NET Core (.NET 8)  application that combines both:

Backend REST API  (CRUD for blog posts)
MVC Admin UI (list, view, create, edit, delete)

Database Configuration

Development: SQLite file (`blog.db`)
Production: Azure SQL (supports SQL Authentication or Managed Identity)

Goal Alignment

 Level 1: API
 Level 2: Admin UI
Level 4: Security
Level 3: Deployment (prepared & documented; Azure deployment attempted)

---

What I Built

 Entity / Model

BlogPost :

  * `Id`
  * `Title`
  * `Content`
  * `Author`
  * `CreatedAt`
  * `UpdatedAt`

Data Access

* **EF Core DbContext:** `BlogDbContext`
* **Dev:** SQLite (`EnsureCreated()`)
* **Prod:** Azure SQL (`Database.Migrate()`)

DTOs and Validation

* DTOs for create, update, and read.
* **FluentValidation** validators for DTOs and query parameters.

REST API (BlogsController)

| Method     | Endpoint          | Description                                                         |
| ---------- | ----------------- | ------------------------------------------------------------------- |
| **POST**   | `/api/blogs`      | Create blog post                                                    |
| **GET**    | `/api/blogs`      | List (pagination, author/date filter, search, sort by created desc) |
| **GET**    | `/api/blogs/{id}` | Read specific blog                                                  |
| **PUT**    | `/api/blogs/{id}` | Update blog                                                         |
| **DELETE** | `/api/blogs/{id}` | Delete blog                                                         |

Admin UI (PostsController + Razor Views)

* **Index**: List with pagination and search
* **Details**
* **Create**
* **Edit**
* **Delete**

>  Runs in the same app — no separate frontend project needed.



Error Handling

* Custom error handling middleware returns structured JSON for unhandled exceptions.



Security & Hardening

 Rate limiting (20 req/sec, fixed window)
 CORS (permissive for dev; restricted in prod)
 HTTPS redirection
 Security headers middleware:

  * X-Content-Type-Options
  * X-Frame-Options
  * XSS protection
  * Content Security Policy (CSP baseline)
  * Referrer-Policy



 **Swagger**

* **`/swagger`** endpoint for full OpenAPI documentation and testing.



Static & Default Files

* `wwwroot/index.html` redirects to `/Posts/Index`
* `UseDefaultFiles()` + `UseStaticFiles()` ensures root serves `index.html`



Runtime Flow

1. On startup:

   * Dev → Creates SQLite DB file automatically.
   * Prod → Applies EF migrations automatically.
2. Middleware chain:

   * HTTPS → StaticFiles → CORS → RateLimiter → Error/Security Headers → MVC/API
3. Admin UI:

   * Root (`index.html`) → redirects to `/Posts/Index`
   * CRUD actions interact directly via DbContext
4. API:

   * Pure JSON endpoints for integration and automated testing.



Configuration & Environments

App Settings:

* `appsettings.json` → Base configuration
* `appsettings.Production.json` → Azure SQL connection string

Connection Strings

SQL Auth Example:

  ```
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:<server>.database.windows.net,1433;Database=blogdb;User ID=<user>;Password=<password>;Encrypt=True"
  }
  ```
  Managed Identity Example:

  ```
  "DefaultConnection": "Server=tcp:<server>.database.windows.net,1433;Database=blogdb;Authentication=Active Directory Managed Identity;"
  ```

Program.cs :

* Dev → `.UseSqlite(...)`
* Prod → `.UseSqlServer(...)` (SqlClient handles Managed Identity if configured)

---

Deployment Status (Azure) :

* Zip Deploy flow prepared (PowerShell script automates publish + zip + push via Kudu)
* Verified files on Kudu (`Blog.Api.dll`, `web.config`, `wwwroot/index.html`)
* Root 403 fixed by adding `index.html` and enabling `UseDefaultFiles()`
* Startup 500.30 traced to DB authentication (resolved using Managed Identity mode)

---

What’s Working Now :

Local development verified

* Run:

  ```bash
  dotnet run --project Blog.Api
  ```
* Access:

  * UI → [http://localhost:5091/Posts/Index](http://localhost:5091/Posts/Index)
  * Swagger → [http://localhost:5091/swagger](http://localhost:5091/swagger)
* SQLite dev DB auto-created; full CRUD functional.

Repository includes:

* Clean structure (single project)
* `.gitignore`
* `README.md`
* GitHub Actions build workflow (builds & publishes artifacts)



Key Packages / Technologies :

* **ASP.NET Core MVC + API**
* **Entity Framework Core** (SQLite for dev, SQL Server for prod)
* **FluentValidation**
* **Swagger / OpenAPI**
* **Microsoft.Data.SqlClient** (Azure SQL)
* **Azure.Identity** (Managed Identity)
* **Rate Limiting Middleware**
* **Security Headers**
* **CORS & HTTPS Enforcement**

---

 Run Locally:

```bash
# Prerequisites
Install .NET 8 SDK

# Run the project
dotnet run --project .\Blog.Api

# Access the application
UI: http://localhost:5091/Posts/Index
API Docs: http://localhost:5091/swagger
```



 Deploy to Azure (Zip Deploy Method):


# Publish and zip
dotnet publish .\Blog.Api -c Release -o .\publish
Compress-Archive -Path .\publish\* -DestinationPath .\publish.zip -Force
```

Azure App Service Configurations :

* `ASPNETCORE_ENVIRONMENT=Production`
* `DefaultConnection` = Azure SQL connection string
* Identity: Enable System-assigned or attach User-assigned Managed Identity

  

 SQL Permissions:
  sql
  CREATE USER [<ManagedIdentityName>] FROM EXTERNAL PROVIDER;
  ALTER ROLE db_datareader ADD MEMBER [<ManagedIdentityName>];
  ALTER ROLE db_datawriter ADD MEMBER [<ManagedIdentityName>];
  

Upload ZIP via Kudu

* URL: `https://<yourapp>.scm.azurewebsites.net/ZipDeployUI`
* Restart App Service after upload.



 Final Output (Reviewer Experience)

API:

* Fully documented in Swagger
* JSON responses with structured validation & pagination metadata

Admin UI:

* View list, search, paginate posts
* Create/Edit/Delete forms with validation
* Details view functional

Deployment Note

I successfully deployed this project on Microsoft Azure, but due to an unexpected Azure service issue, the hosted URL could not be made publicly accessible before submission.

Please find the deployment screenshot below as proof of successful deployment:

I can redeploy and share the working link once Azure services are restored or accessible.







