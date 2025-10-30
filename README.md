
# 🧩 IICL Software Engineer Hiring Challenge

## 📘 Overview
Single-project **ASP.NET Core (.NET 8)** Blog Management Application that combines:

- **Backend REST API (CRUD)**
- **MVC Admin UI** (list, view, create, edit, delete)

### Implemented Challenge Levels
| Level | Description |
|--------|--------------|
| 1 | Backend CRUD API |
| 2 | Frontend Admin UI |
| 3 | Cloud Deployment (live URL + instructions) |
| 4 | Security, Performance & Reliability Hardening |

---

## 🌐 Live Demo

- **App:** [https://iicl-blog.onrender.com/Posts/Index](https://iicl-blog.onrender.com/Posts/Index)  
- **Swagger:** [https://iicl-blog.onrender.com/swagger](https://iicl-blog.onrender.com/swagger)  
- **Health:** [https://iicl-blog.onrender.com/health](https://iicl-blog.onrender.com/health)

**Repository:** [https://github.com/AbhinavVuddanti/iicl-blog](https://github.com/AbhinavVuddanti/iicl-blog)

---

## 🧱 Level 1: Backend API – Blog CRUD

### Entity: `BlogPost`
| Field | Type | Description |
|--------|------|-------------|
| id | int | Auto-generated primary key |
| title | string | Blog title |
| content | string | Blog body |
| author | string | Author name |
| createdAt | datetime | Auto timestamp |
| updatedAt | datetime | Auto timestamp |

### Endpoints
```

POST    /api/blogs
GET     /api/blogs?page=1&pageSize=10&author=&from=&to=&search=
GET     /api/blogs/{id}
PUT     /api/blogs/{id}
DELETE  /api/blogs/{id}

````

### Validation
- **FluentValidation** on DTOs and query parameters  
- **Central middleware** for consistent JSON error responses  
- **Pagination**, **filtering**, and **search**

### Example Payload
```json
{
  "title": "Hello World",
  "content": "My first blog post",
  "author": "IICL"
}
````

---

## 🖥️ Level 2: Frontend UI – Blog Management

### Pages

* List (search + pagination)
* Details
* Create / Edit (server-side validation)
* Delete confirmation

### Structure

Single project → API + MVC hosted together
Root: `wwwroot/index.html` → redirects to `/Posts/Index`

---

## ☁️ Level 3: Cloud Deployment

### 🟢 Public Demo (Render)

**Demo:** [https://iicl-blog.onrender.com](https://iicl-blog.onrender.com)
Deployed via **Dockerfile (multi-stage build)** using **Render cloud** with SQLite for demo persistence.

### 🧰 Azure Deployment Documentation

#### Option A: SQL Authentication

**Connection string:**

```
Server=tcp:<server>.database.windows.net,1433;
Initial Catalog=<db>;
Persist Security Info=False;
User ID=<user>;
Password=<password>;
MultipleActiveResultSets=False;
Encrypt=True;
TrustServerCertificate=False;
Connection Timeout=30;
```

**App settings:**

```
ASPNETCORE_ENVIRONMENT=Production
```

**Zip Deploy via Kudu:**

```bash
dotnet publish .\Blog.Api -c Release -o .\publish
Compress-Archive -Path .\publish\* -DestinationPath .\publish.zip -Force
```

Upload to:
👉 https://<app>.scm.azurewebsites.net/ZipDeployUI
Restart App Service after upload.

#### Option B: Managed Identity (Passwordless, Recommended)

1. Enable **System-assigned Identity** in Azure Web App.
2. Run on SQL (AAD Admin):

   ```sql
   CREATE USER [<managed-identity-display-name>] FROM EXTERNAL PROVIDER;
   ALTER ROLE db_datareader ADD MEMBER [<managed-identity-display-name>];
   ALTER ROLE db_datawriter ADD MEMBER [<managed-identity-display-name>];
   ```
3. Enable “Allow Azure services…” in SQL Networking.
4. Use connection string with:

   ```
   Authentication=Active Directory Managed Identity;
   ```

---

## 🔒 Level 4: Application Security & Hardening

### Secure Coding

* EF Core (parameterized queries → prevents SQL injection)
* Razor HTML encoding (XSS prevention)
* FluentValidation (server-side input validation)

### API Protection

* Fixed-window **rate limiting (20 req/sec)**
* Centralized **JSON error middleware**
* **Logging** at Information level

### HTTPS & Security Headers

* HTTPS redirection, HSTS, and proxy-aware headers
* Security headers: `X-Content-Type-Options`, `X-Frame-Options`, `Referrer-Policy`, and basic CSP

### CORS

* Development: permissive
* Production: configurable via `Cors:AllowedOrigins`

### Health

`/health` → readiness/liveness check (`200 OK`)

---

## 🧪 Local Development

### Requirements

* .NET 8 SDK

### Run Locally

```bash
dotnet run --project .\Blog.Api
```

UI: [http://localhost:5091/Posts/Index](http://localhost:5091/Posts/Index)
Swagger: [http://localhost:5091/swagger](http://localhost:5091/swagger)

To reset dev DB:

```bash
# Stop app, then:
del .\Blog.Api\blog.db
dotnet run --project .\Blog.Api
```

---

## ⚙️ Tech Stack

* ASP.NET Core 8 (MVC + API)
* Entity Framework Core
* SQLite (dev) / Azure SQL (prod)
* FluentValidation
* Swagger / OpenAPI
* Middleware: error handling, security headers, rate limiting, CORS

---

## 📂 Project Structure

```
Blog.Api/
 ┣ Controllers/ (API + MVC Posts)
 ┣ Views/Posts/ (Index, Details, Create, Edit, Delete)
 ┣ wwwroot/index.html
 ┣ Data/, Models/, Dtos/, Validators/
 ┣ Middleware/ (ErrorHandling, SecurityHeaders)
 ┣ Program.cs
 ┣ Dockerfile
 ┣ .dockerignore
 ┣ .github/workflows/dotnet.yml
 ┗ README.md
```

---

## ✅ Deliverables Checklist

✔ Level 1 — CRUD API, validation, pagination/filtering
✔ Level 2 — Admin UI with full CRUD
✔ Level 3 — Cloud deployment (live demo + deploy steps)
✔ Level 4 — Rate limiting, CORS, HTTPS, headers, health endpoint

---

## ⚠️ Deployment Note

> I successfully deployed this project on **Microsoft Azure**, but due to an **Azure service issue**, the hosted URL could not be made publicly accessible before submission.

Deployment screenshots are included in the repository as proof of successful deployment.
I can redeploy and share the working link once Azure services are restored.

---

## 🖼️ Suggested Screenshots (Optional)

Place screenshots under `docs/screenshots/` for clarity:

* Swagger (all endpoints visible)
* Admin UI list view (after creating posts)
* Create/Edit/Delete screens

---

## 📎 Quick Commands

Run locally:

```bash
dotnet run --project .\Blog.Api
```

Publish & zip (for Azure):

```bash
dotnet publish .\Blog.Api -c Release -o .\publish
Compress-Archive -Path .\publish\* -DestinationPath .\publish.zip -Force
```

Kudu ZipDeploy:

```
https://<your-app>.scm.azurewebsites.net/ZipDeployUI
```

---

## 👤 Author

**Abhinav Vuddanti**
📧 [abhin6289@gmail.com](mailto:abhin6289@gmail.com)

---

### 🧾 Notes

* Render demo runs with SQLite (data resets on restart/redeploy)
* For persistence in cloud, attach a managed DB or persistent disk and update connection string accordingly

---

✅ **Summary:**
This repository delivers **Levels 1–4** with:

* Working public demo
* Clean single-project architecture
* Documented Azure + Render deployment paths
* Security, performance, and reliability best practices






