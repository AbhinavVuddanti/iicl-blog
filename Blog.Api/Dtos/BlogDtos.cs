using System;

namespace Blog.Api.Dtos
{
    public class BlogPostDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class BlogPostCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
    }

    public class BlogPostUpdateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
    }

    public class PagedResult<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public IReadOnlyCollection<T> Items { get; set; } = Array.Empty<T>();
    }

    public class BlogQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Author { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string? Search { get; set; }
        public string? SortBy { get; set; } // createdAt|title|author
        public string? SortDir { get; set; } // asc|desc
    }
}
