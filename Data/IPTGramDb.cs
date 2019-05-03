using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IPTGram.Data
{
    public class IPTGramDb : IdentityDbContext<User>
    {
        public IPTGramDb(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }

        public DbSet<PostLike> PostLikes { get; set; }

        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<PostLike>()
                .HasKey(x => new { x.PostId, x.UserId });

            builder.Entity<Post>()
                .HasOne(p => p.User)
                .WithMany(u => u.Posts)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PostLike>()
                .HasOne(l => l.User)
                .WithMany(u => u.Likes)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(builder);
        }
    }
}