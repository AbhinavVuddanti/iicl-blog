using Blog.Api.Dtos;
using FluentValidation;

namespace Blog.Api.Validators
{
    public class BlogPostCreateDtoValidator : AbstractValidator<BlogPostCreateDto>
    {
        public BlogPostCreateDtoValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Author).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Content).NotEmpty().MinimumLength(10);
        }
    }

    public class BlogPostUpdateDtoValidator : AbstractValidator<BlogPostUpdateDto>
    {
        public BlogPostUpdateDtoValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Author).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Content).NotEmpty().MinimumLength(10);
        }
    }

    public class BlogQueryValidator : AbstractValidator<BlogQuery>
    {
        public BlogQueryValidator()
        {
            RuleFor(x => x.Page).GreaterThan(0);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
            RuleFor(x => x.SortBy).Must(v => v == null || new[] { "createdAt", "title", "author" }.Contains(v))
                .WithMessage("SortBy must be one of: createdAt, title, author");
            RuleFor(x => x.SortDir).Must(v => v == null || v == "asc" || v == "desc")
                .WithMessage("SortDir must be asc or desc");
        }
    }
}
