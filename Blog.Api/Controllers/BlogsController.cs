using Blog.Api.Data;
using Blog.Api.Dtos;
using Blog.Api.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlogsController : ControllerBase
    {
        private readonly BlogDbContext _db;
        private readonly IValidator<BlogQuery> _queryValidator;
        private readonly IValidator<BlogPostCreateDto> _createValidator;
        private readonly IValidator<BlogPostUpdateDto> _updateValidator;
        private readonly ILogger<BlogsController> _logger;

        public BlogsController(BlogDbContext db,
            IValidator<BlogQuery> queryValidator,
            IValidator<BlogPostCreateDto> createValidator,
            IValidator<BlogPostUpdateDto> updateValidator,
            ILogger<BlogsController> logger)
        {
            _db = db;
            _queryValidator = queryValidator;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<BlogPostDto>>> GetAll([FromQuery] BlogQuery query)
        {
            var v = await _queryValidator.ValidateAsync(query);
            if (!v.IsValid)
            {
                return BadRequest(new { errors = v.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }) });
            }

            IQueryable<BlogPost> q = _db.BlogPosts.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(query.Author))
            {
                q = q.Where(x => x.Author == query.Author);
            }
            if (query.From.HasValue)
            {
                q = q.Where(x => x.CreatedAt >= query.From.Value);
            }
            if (query.To.HasValue)
            {
                q = q.Where(x => x.CreatedAt <= query.To.Value);
            }
            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var s = query.Search.ToLower();
                q = q.Where(x => x.Title.ToLower().Contains(s) || x.Content.ToLower().Contains(s));
            }

            q = (query.SortBy, query.SortDir?.ToLower()) switch
            {
                ("title", "asc") => q.OrderBy(x => x.Title),
                ("title", _) => q.OrderByDescending(x => x.Title),
                ("author", "asc") => q.OrderBy(x => x.Author),
                ("author", _) => q.OrderByDescending(x => x.Author),
                ("createdAt", "asc") => q.OrderBy(x => x.CreatedAt),
                _ => q.OrderByDescending(x => x.CreatedAt)
            };

            var total = await q.CountAsync();
            var items = await q.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize)
                .Select(x => new BlogPostDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Content = x.Content,
                    Author = x.Author,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                })
                .ToListAsync();

            return Ok(new PagedResult<BlogPostDto>
            {
                Page = query.Page,
                PageSize = query.PageSize,
                TotalCount = total,
                Items = items
            });
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<BlogPostDto>> GetById(int id)
        {
            var entity = await _db.BlogPosts.FindAsync(id);
            if (entity == null) return NotFound(new { error = "Not Found" });
            return Ok(MapToDto(entity));
        }

        [HttpPost]
        public async Task<ActionResult<BlogPostDto>> Create([FromBody] BlogPostCreateDto dto)
        {
            var v = await _createValidator.ValidateAsync(dto);
            if (!v.IsValid)
            {
                return BadRequest(new { errors = v.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }) });
            }

            var now = DateTime.UtcNow;
            var entity = new BlogPost
            {
                Title = dto.Title.Trim(),
                Author = dto.Author.Trim(),
                Content = dto.Content.Trim(),
                CreatedAt = now,
                UpdatedAt = now
            };
            _db.BlogPosts.Add(entity);
            await _db.SaveChangesAsync();

            var result = MapToDto(entity);
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, result);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<BlogPostDto>> Update(int id, [FromBody] BlogPostUpdateDto dto)
        {
            var v = await _updateValidator.ValidateAsync(dto);
            if (!v.IsValid)
            {
                return BadRequest(new { errors = v.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }) });
            }

            var entity = await _db.BlogPosts.FindAsync(id);
            if (entity == null) return NotFound(new { error = "Not Found" });

            entity.Title = dto.Title.Trim();
            entity.Author = dto.Author.Trim();
            entity.Content = dto.Content.Trim();
            entity.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return Ok(MapToDto(entity));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _db.BlogPosts.FindAsync(id);
            if (entity == null) return NotFound(new { error = "Not Found" });
            _db.BlogPosts.Remove(entity);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        private static BlogPostDto MapToDto(BlogPost x) => new BlogPostDto
        {
            Id = x.Id,
            Title = x.Title,
            Content = x.Content,
            Author = x.Author,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        };
    }
}
