using App.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

/* Project Database cotext */
namespace App.Context
{
    public class QuestionsContext : IdentityDbContext<User>
    {
        public QuestionsContext(DbContextOptions options) : base(options) { }

        // Tables
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<LikedPost> LikedPosts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Rename identity tables
            builder.Entity<User>(entity => entity.ToTable("Users"));
            builder.Entity<IdentityRole>(entity => entity.ToTable("Roles"));
            builder.Entity<IdentityUserRole<string>>(entity => entity.ToTable("UserRoles"));
            builder.Entity<IdentityUserClaim<string>>(entity => entity.ToTable("UserClaims"));
            builder.Entity<IdentityUserLogin<string>>(entity => entity.ToTable("UserLogins"));
            builder.Entity<IdentityUserToken<string>>(entity => entity.ToTable("UserTokens"));
            builder.Entity<IdentityRoleClaim<string>>(entity => entity.ToTable("RoleClaims"));
        }
    }
}
