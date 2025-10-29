using Blog.Api.Data;
using Blog.Api.Dtos;
using Blog.Api.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Api.Controllers
{
    public class PostsController : Controller
    {
        private readonly BlogDbContext _db;
        private readonly IValidator<BlogQuery> _queryValidator;
        private readonly IValidator<BlogPostCreateDto> _createValidator;
        private readonly IValidator<BlogPostUpdateDto> _updateValidator;
        private readonly ILogger<PostsController> _logger;

        public PostsController(BlogDbContext db,
            IValidator<BlogQuery> queryValidator,
            IValidator<BlogPostCreateDto> createValidator,
            IValidator<BlogPostUpdateDto> updateValidator,
            ILogger<PostsController> logger)
        {
            _db = db;
            _queryValidator = queryValidator;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? search = null)
        {
            var query = new BlogQuery { Page = page, PageSize = pageSize, Search = search };
            var v = await _queryValidator.ValidateAsync(query);
            if (!v.IsValid)
            {
                foreach (var e in v.Errors)
                    ModelState.AddModelError(e.PropertyName, e.ErrorMessage);
            }

            IQueryable<BlogPost> q = _db.BlogPosts.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var s = query.Search.ToLower();
                q = q.Where(x => x.Title.ToLower().Contains(s) || x.Content.ToLower().Contains(s));
            }
            q = q.OrderByDescending(x => x.CreatedAt);

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

            var result = new PagedResult<BlogPostDto>
            {
                Page = query.Page,
                PageSize = query.PageSize,
                TotalCount = total,
                Items = items
            };
            ViewBag.Search = search;
            return View(result);
        }

        public async Task<IActionResult> Details(int id)
        {
            var entity = await _db.BlogPosts.FindAsync(id);
            if (entity == null) return NotFound();
            return View(new BlogPostDto
            {
                Id = entity.Id,
                Title = entity.Title,
                Content = entity.Content,
                Author = entity.Author,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            });
        }

        public IActionResult Create()
        {
            return View(new BlogPostCreateDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BlogPostCreateDto dto)
        {
            var v = await _createValidator.ValidateAsync(dto);
            if (!v.IsValid)
            {
                foreach (var e in v.Errors)
                    ModelState.AddModelError(e.PropertyName, e.ErrorMessage);
                return View(dto);
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
            return RedirectToAction(nameof(Details), new { id = entity.Id });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var entity = await _db.BlogPosts.FindAsync(id);
            if (entity == null) return NotFound();
            ViewBag.Id = id;
            return View(new BlogPostUpdateDto
            {
                Title = entity.Title,
                Content = entity.Content,
                Author = entity.Author
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BlogPostUpdateDto dto)
        {
            var v = await _updateValidator.ValidateAsync(dto);
            if (!v.IsValid)
            {
                foreach (var e in v.Errors)
                    ModelState.AddModelError(e.PropertyName, e.ErrorMessage);
                ViewBag.Id = id;
                return View(dto);
            }

            var entity = await _db.BlogPosts.FindAsync(id);
            if (entity == null) return NotFound();

            entity.Title = dto.Title.Trim();
            entity.Author = dto.Author.Trim();
            entity.Content = dto.Content.Trim();
            entity.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _db.BlogPosts.FindAsync(id);
            if (entity == null) return NotFound();
            return View(new BlogPostDto
            {
                Id = entity.Id,
                Title = entity.Title,
                Content = entity.Content,
                Author = entity.Author,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            });
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entity = await _db.BlogPosts.FindAsync(id);
            if (entity != null)
            {
                _db.BlogPosts.Remove(entity);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
