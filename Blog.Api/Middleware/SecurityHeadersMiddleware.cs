namespace Blog.Api.Middleware
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;

        public SecurityHeadersMiddleware(RequestDelegate next, IWebHostEnvironment env)
        {
            _next = next;
            _env = env;
        }

        public async Task Invoke(HttpContext context)
        {
            var headers = context.Response.Headers;
            headers["X-Content-Type-Options"] = "nosniff";
            headers["X-Frame-Options"] = "DENY";
            headers["X-XSS-Protection"] = "1; mode=block";
            headers["Referrer-Policy"] = "no-referrer";

            var csp = "default-src 'self'";
            if (_env.IsDevelopment())
            {
                csp = "default-src 'self'";
            }
            headers["Content-Security-Policy"] = csp;

            await _next(context);
        }
    }
}
