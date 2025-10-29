using Blog.Api.Data;
using Blog.Api.Middleware;
using FluentValidation;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Blog.Api.Validators;
using Azure.Core;
using Azure.Identity;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Controllers + MVC Views + FluentValidation
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IValidator<Blog.Api.Dtos.BlogQuery>, BlogQueryValidator>();
builder.Services.AddScoped<IValidator<Blog.Api.Dtos.BlogPostCreateDto>, BlogPostCreateDtoValidator>();
builder.Services.AddScoped<IValidator<Blog.Api.Dtos.BlogPostUpdateDto>, BlogPostUpdateDtoValidator>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// EF Core (SQLite for Dev, SQL Server for Production)
builder.Services.AddDbContext<BlogDbContext>((sp, opt) =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    var env = sp.GetRequiredService<IHostEnvironment>();
    var connStr = cfg.GetConnectionString("DefaultConnection");
    if (env.IsProduction())
    {
        // Let Microsoft.Data.SqlClient handle Managed Identity via connection string:
        // e.g., Authentication=Active Directory Managed Identity; (and optional User Id=<clientId> for user-assigned MI)
        opt.UseSqlServer(connStr);
    }
    else
    {
        opt.UseSqlite(connStr ?? "Data Source=blog.db");
    }
});

// CORS
const string CorsPolicy = "DefaultCors";
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy, policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

// Rate limiting (simple fixed window)
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("global", limiterOptions =>
    {
        limiterOptions.Window = TimeSpan.FromSeconds(1);
        limiterOptions.PermitLimit = 20;
        limiterOptions.QueueLimit = 0;
    });
});

var app = builder.Build();

// Ensure DB ready: Dev => EnsureCreated (SQLite), Prod => Migrate (SQL Server)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BlogDbContext>();
    if (app.Environment.IsProduction())
    {
        db.Database.Migrate();
    }
    else
    {
        db.Database.EnsureCreated();
    }
}

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors(CorsPolicy);
app.UseRateLimiter();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<SecurityHeadersMiddleware>();

app.MapControllers();

// MVC default route for frontend admin UI within same project
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Posts}/{action=Index}/{id?}");

// Redirect root to Posts index (helps Azure default document behavior)
app.MapGet("/", () => Results.Redirect("/Posts/Index"));

app.Run();
