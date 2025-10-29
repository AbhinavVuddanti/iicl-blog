
IICL Blog Management Application (.NET 8, Single Project)
Overview
A single-project ASP.NET Core (.NET 8) application that combines:
•	Backend REST API (CRUD for blog posts)
•	MVC Admin UI (List, View, Create, Edit, Delete)
Implements the hiring challenge levels:
•	Level 1: Backend CRUD API
•	Level 2: Frontend Admin UI
•	Level 3: Cloud Deployment (prepared, with Azure instructions)
•	Level 4: Security Enhancements
________________________________________
Tech Stack
•	ASP.NET Core 8 (MVC + API)
•	Entity Framework Core
•	Development: SQLite
•	Production: Azure SQL (SQL Auth or Managed Identity)
•	FluentValidation
•	Swagger / OpenAPI
•	Security middleware: Error handling, Rate limiting, CORS, HTTPS redirection, Security headers
________________________________________
Project Structure
Blog.Api/
 ├── Controllers (API + MVC Posts)
 ├── Views/Posts (Index, Details, Create, Edit, Delete)
 ├── wwwroot/index.html (redirects root → /Posts/Index)
 ├── Data (DbContext), Models, Dtos, Validators
 ├── Middleware (Error handling, Security headers)
 ├── Program.cs (wiring: EF Core, middleware, routes)
 ├── README.md
 ├── .gitignore
 ├── deploy-azure.ps1 (ZipDeploy helper script)
 └── .github/workflows/dotnet.yml (CI build)
________________________________________
Level 1 — Backend API (Blog CRUD)
Entity: BlogPost
Fields:
•	id (auto), title, content, author, createdAt, updatedAt
Endpoints (JSON):
POST    /api/blogs
GET     /api/blogs?page=1&pageSize=10&author=&from=&to=&search=
GET     /api/blogs/{id}
PUT     /api/blogs/{id}
DELETE  /api/blogs/{id}
Validation: FluentValidation on DTOs and query parameters
Errors: Structured JSON via error handling middleware
Bonus: Pagination, filtering (author/date), search, default sorting
Example Create Payload:
{
  "title": "Hello World",
  "content": "My first blog post",
  "author": "IICL"
}
________________________________________
Level 2 — Frontend Admin UI (Integrated)
Pages:
•	List with search + pagination
•	Details
•	Create/Edit (with validation)
•	Delete confirmation
Runs inside the same project (no separate frontend).
Root path serves wwwroot/index.html, which redirects to /Posts/Index.
________________________________________
Level 3 — Deployment (Azure App Service)
Deployment configured and tested locally.
Cloud publish steps included.
Managed Identity DB access is wired; final DNS/app availability depends on cloud configuration.
A live URL can be shared once Azure services are fully accessible.
Option A: SQL Authentication
1.	Configure App Service → Connection Strings → DefaultConnection (type: SQLAzure)
2.	Use the following connection format:
Server=tcp:<server>.database.windows.net,1433;Initial Catalog=<db>;
Persist Security Info=False;User ID=<user>;Password=<password>;
MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
3.	Add App Settings:
ASPNETCORE_ENVIRONMENT = Production
WEBSITE_RUN_FROM_PACKAGE = 1 (optional)
4.	Publish and Zip:
dotnet publish .\Blog.Api -c Release -o .\publish
Compress-Archive -Path .\publish* -DestinationPath .\publish.zip -Force
5.	Upload via Azure ZipDeploy (Kudu):
https://<app>.scm.azurewebsites.net/ZipDeployUI
6.	Restart Web App.
________________________________________
Option B: Managed Identity (Passwordless, Recommended)
1.	Enable Managed Identity on Web App.
2.	Add user to SQL Database using:
CREATE USER [<app_name>] FROM EXTERNAL PROVIDER;
ALTER ROLE db_datareader ADD MEMBER [<app_name>];
ALTER ROLE db_datawriter ADD MEMBER [<app_name>];
3.	Enable “Allow Azure services to access this server” in SQL Networking.
4.	Use this connection string:
Server=tcp:<server>.database.windows.net,1433;
Initial Catalog=<db>;
Encrypt=True;
TrustServerCertificate=False;
Connection Timeout=30;
Authentication=Active Directory Managed Identity;
________________________________________
Troubleshooting (Azure)
•	403 Root Access: Ensure index.html exists in wwwroot, UseDefaultFiles enabled, restart App.
•	500.30 Startup Failure: Usually due to DB connection/auth issues.
•	Check Logs: Use Azure Log Stream → test /swagger and /Posts/Index.
________________________________________
Level 4 — Security & Hardening
•	Validation on all incoming DTOs (FluentValidation)
•	Error handling middleware with consistent JSON responses
•	Rate limiting (20 requests/sec)
•	CORS:
o	Development → Permissive
o	Production → Restricted
•	HTTPS redirection enforced
•	Security headers:
X-Content-Type-Options, X-Frame-Options, Referrer-Policy, basic CSP
________________________________________
How to Run Locally
Requirements:
•	.NET 8 SDK
Run:
dotnet run --project .\Blog.Api
Admin UI → http://localhost:5091/Posts/Index
Swagger → http://localhost:5091/swagger
If another port shows in the console, use that instead.
Reset Dev DB (optional):
Stop app → delete .\Blog.Api\blog.db → run again
________________________________________
CI/CD (GitHub Actions)
•	.github/workflows/dotnet.yml builds on push to main
•	Publishes compiled output as artifact
________________________________________
Deliverables Checklist
✅ Level 1 — CRUD API, Validation, Pagination
✅ Level 2 — Admin UI with full CRUD
✅ Level 3 — Azure App Service deployment path
✅ Level 4 — Rate limiting, CORS, Security Headers
________________________________________
Deployment Note
I successfully deployed this project on Microsoft Azure,
but due to an unexpected Azure service issue,
the hosted URL could not be made publicly accessible before submission.
Please find the deployment screenshot below as proof of successful deployment:
(If images don’t load, refer to /docs/screenshots/ folder in this repository.)
 
 
I can redeploy and share the working live URL once Azure services are fully restored.
________________________________________
Quick Commands
dotnet run --project .\Blog.Api
dotnet publish .\Blog.Api -c Release -o .\publish
Compress-Archive -Path .\publish* -DestinationPath .\publish.zip -Force
Azure ZipDeploy:
•	https://iicl-blog-bgfbehb8grdcb0ax.canadacentral-01.azurewebsites.net
•	https://iicl-blog-bgfbehb8grdcb0ax.canadacentral-01.azurewebsites.net/Posts/Index
•	https://iicl-blog-bgfbehb8grdcb0ax.canadacentral-01.azurewebsites.net/swagger


________________________________________
Contact
Author: Abhinav Vuddanti
📧 abhin6289@gmail.com
💻 Repo includes complete local run instructions and deployment steps.

