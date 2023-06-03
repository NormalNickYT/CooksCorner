using CooksCornerAPP.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CooksCornerAPP.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            SeedRoles(modelBuilder);

        }

        private void SeedRoles(ModelBuilder builder)
        {
            builder.Entity<IdentityRole>().HasData(

                new IdentityRole() { Name = "admin", ConcurrencyStamp = "1", NormalizedName = "ADMIN" },
                new IdentityRole() { Name = "moderator", ConcurrencyStamp = "2", NormalizedName = "MODERATOR" },
                new IdentityRole() { Name = "user", ConcurrencyStamp = "3", NormalizedName = "USER" }

           );
        }

        public DbSet<Recipes> Recipes { get; set; }
        public DbSet<UserRecipesFavorites> UserRecipesFavorites { get; set; }
        public DbSet<Notification> Notifications { get; set; }

    }
}