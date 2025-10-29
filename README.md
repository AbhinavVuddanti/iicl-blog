# IICL Blog Management Application (.NET 8, Single Project)

Single-project ASP.NET Core app with:
- REST API (Level 1)
- MVC Admin UI (Level 2)
- Production-ready deployment path (Level 3)
- Security middleware: validation, rate limiting, CORS, HTTPS, security headers (Level 4)

## Tech
- ASP.NET Core (.NET 8)
- EF Core (SQLite for Dev, SQL Server for Prod)
- FluentValidation
- Polly (Admin-only previously; now unified in a single app)

## Run locally
- Requirements: .NET 8 SDK
- Start
  - `dotnet run --project .\Blog.Api`
  - App: http://localhost:5091
  - Swagger: http://localhost:5091/swagger
- DB: SQLite file `blog.db` auto-created in Blog.Api folder.

## API Endpoints (summary)
- POST /api/blogs
- GET /api/blogs?page=1&pageSize=10&author=&from=&to=&search=&sortBy=&sortDir=
- GET /api/blogs/{id}
- PUT /api/blogs/{id}
- DELETE /api/blogs/{id}

## Security
- Input validation (FluentValidation)
- Error handling middleware (JSON payloads)
- Fixed-window rate limiting (20 req/sec)
- HTTPS redirection (use HTTPS in production)
- Security headers (CSP, X-Frame-Options, X-Content-Type-Options, etc.)
- CORS: permissive in Development; restrict in Production

## Deployment to Azure App Service (Option A: Publish Profile)
We will deploy the single project (Blog.Api) to Azure App Service. For production, use Azure SQL.

### 1) Azure resources
- Create App Service (Linux) runtime: .NET 8 LTS
- Create Azure SQL server + database (e.g., `iiclblogdb`)
- In Web App > Configuration:
  - Add ConnectionStrings: `DefaultConnection = Server=tcp:<server>.database.windows.net,1433;Initial Catalog=<db>;Persist Security Info=False;User ID=<user>;Password=<password>;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;`
  - Add `ASPNETCORE_ENVIRONMENT = Production`
  - Optional: add `WEBSITE_RUN_FROM_PACKAGE = 1` (recommended for zip deploy)

### 2) Production configuration in code
- `Blog.Api/appsettings.Production.json` (placeholders provided). App reads `DefaultConnection` in Production and uses SQL Server.
- EF Core uses `Database.Migrate()` in Production to apply migrations automatically on startup.

### 3) Create EF Core migration (one-time)
- Install tools if needed: `dotnet tool install --global dotnet-ef`
- From solution root:
  - `dotnet ef migrations add InitialCreate --project .\Blog.Api --startup-project .\Blog.Api`
  - Commit the generated `Migrations/` folder

### 4) Deploy using Publish Profile (Zip Deploy via Kudu)
- Download publish profile from Azure Web App: Overview > Get publish profile (XML)
- Save as `C:\path\to\your\webapp.PublishSettings` (do NOT commit)
- Run the script (PowerShell):
  - `./deploy-azure.ps1 -PublishProfilePath C:\path\to\your\webapp.PublishSettings -ProjectPath .\Blog.Api -Configuration Release`
- The script will:
  - `dotnet publish` to `./publish`
  - Zip the output
  - POST to Kudu ZipDeploy with credentials from the publish profile

### 5) Verify
- Open `https://<your-app>.azurewebsites.net`
- Swagger at `/swagger`

### 6) GitHub
- Create a new repo and push:
  - `git init`
  - `git add .`
  - `git commit -m "IICL Blog challenge: single-project API+UI with security"`
  - `git branch -M main`
  - `git remote add origin <your-repo-url>`
  - `git push -u origin main`
- Ensure you do NOT commit `*.PublishSettings` or secrets.

## Repo hygiene
- `blog.db` is dev-only; safe to commit or add to .gitignore.
- Never commit publish profiles or passwords.

## Notes
- In Production, tighten CORS to your frontend origin (same app origin if unified).
- HTTPS enforced by Azure App Service. Ensure custom domains and certificates if needed.
