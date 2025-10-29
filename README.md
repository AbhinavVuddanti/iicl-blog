IICL Blog Management Application (.NET 8, Single Project) :


Overview
This project is a single-project ASP.NET Core (.NET 8) application that I developed as part of a technical challenge.
It combines both backend and frontend functionalities within a single solution — designed for simplicity, scalability, and clean architecture.
It implements four levels of functionality:
•	Level 1: Backend CRUD API
•	Level 2: Frontend Admin UI
•	Level 3: Cloud Deployment (Azure-ready)
•	Level 4: Security Enhancements

Tech Stack
•	ASP.NET Core 8 (MVC + API)
•	Entity Framework Core
•	Database (Development): SQLite
•	Database (Production): Azure SQL (SQL Auth / Managed Identity)
•	FluentValidation
•	Swagger / OpenAPI
•	Security Middleware: Error handling, Rate limiting, HTTPS, CORS, Security headers



Project Structure
Blog.Api/
 ├── Controllers (API + MVC Posts)
 ├── Views/Posts (Index, Details, Create, Edit, Delete)
 ├── wwwroot/index.html (redirects root → /Posts/Index)
 ├── Data (DbContext), Models, Dtos, Validators
 ├── Middleware (Error handling, Security headers)
 ├── Program.cs (middleware, EF Core setup, routing)
 ├── deploy-azure.ps1 (ZipDeploy helper script)
 ├── .github/workflows/dotnet.yml (CI build)
 └── README.md

Level 1 — Backend API (Blog CRUD)
I built a BlogPost entity that supports full CRUD operations via REST API.
Endpoints:
POST    /api/blogs
GET     /api/blogs?page=1&pageSize=10&author=&search=
GET     /api/blogs/{id}
PUT     /api/blogs/{id}
DELETE  /api/blogs/{id}
All endpoints are validated using FluentValidation, and structured error handling middleware ensures consistent JSON responses.

Level 2 — Frontend Admin UI
I implemented an integrated MVC-based Admin UI to manage blog posts:
•	List view with search and pagination
•	Create / Edit forms with validation
•	Delete confirmation
•	Details view
This frontend is served from the same project — no separate frontend framework needed.



Level 3 — Deployment (Azure App Service)
I configured deployment for Microsoft Azure App Service and tested it locally before deployment.
Option A: SQL Authentication
Used a connection string through Azure portal settings.
Option B: Managed Identity (Passwordless)
Configured Azure Managed Identity and added required roles in SQL Database for secure passwordless connection.

Troubleshooting
During deployment, I faced issues with Azure connection timeout (SQL connection).
I confirmed this was a temporary Azure region issue and verified it through the deployment logs.
Once services are fully operational, I can redeploy and share a working URL.

Level 4 — Security Enhancements
•	Input validation using FluentValidation
•	Global exception middleware for structured error output
•	Rate limiting (20 requests/sec)
•	HTTPS redirection in production
•	Secure headers (X-Content-Type-Options, X-Frame-Options, etc.)
•	Configurable CORS policy for restricted environments

How to Run Locally
Requirements:
•	.NET 8 SDK
Run the application:
dotnet run --project .\Blog.Api
•	Admin UI → http://localhost:5091/Posts/Index
•	Swagger → http://localhost:5091/swagger
If the port differs, use the one displayed in the console.
To reset local DB:
Delete blog.db in Blog.Api folder and rerun the app.


Deployment Note
I successfully deployed this project on Microsoft Azure,
but due to an unexpected Azure service issue,
the hosted URL could not be made publicly accessible before submission.
However, the app was fully published and verified from the Kudu deployment panel.
Below are the deployment screenshots captured from Azure:
(If images don’t load in GitHub preview, they are also available in the /docs/screenshots/ folder.)
Once Azure resolves the service-level issue, I can provide the live URL immediately.

Azure URLs (for reference)
•	Web App: https://iicl-blog-bgfbehb8grdcb0ax.canadacentral-01.azurewebsites.net
•	Admin UI: https://iicl-blog-bgfbehb8grdcb0ax.canadacentral-01.azurewebsites.net/Posts/Index
•	Swagger: https://iicl-blog-bgfbehb8grdcb0ax.canadacentral-01.azurewebsites.net/swagger

CI/CD (GitHub Actions)
•	Workflow configured in .github/workflows/dotnet.yml
•	Builds automatically on every push to main
•	Publishes compiled artifacts for testing

Personal Contribution Summary
I independently designed, developed, and deployed this entire project:
•	Set up the full-stack architecture (API + MVC UI)
•	Implemented validation and error-handling middleware
•	Integrated Azure SQL and deployment automation
•	Configured CI/CD and security features
•	Troubleshot Azure connectivity and deployment logs manually
This project reflects my hands-on experience with .NET 8, EF Core, Azure, and secure API development — directly aligned with full-stack .NET developer roles.

Contact Author: Abhinav Vuddanti
📧 abhin6289@gmail.com
💻 The repository includes full setup instructions and deployment steps for reviewers.

