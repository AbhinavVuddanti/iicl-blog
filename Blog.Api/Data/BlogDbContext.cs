using Blog.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog.Api.Data
{
    public class BlogDbContext : DbContext
    {
        public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options)
        {
        }

        public DbSet<BlogPost> BlogPosts => Set<BlogPost>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BlogPost>(e =>
            {
                e.ToTable("BlogPosts");
                e.HasKey(x => x.Id);
                e.Property(x => x.Title).HasMaxLength(200).IsRequired();
                e.Property(x => x.Author).HasMaxLength(100).IsRequired();
                e.Property(x => x.Content).IsRequired();
                e.Property(x => x.CreatedAt).IsRequired();
                e.Property(x => x.UpdatedAt).IsRequired();
                e.HasIndex(x => x.Author);
                e.HasIndex(x => x.CreatedAt);
            });
        }
    }
}
