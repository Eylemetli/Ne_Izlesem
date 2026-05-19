using Microsoft.EntityFrameworkCore;
using MovieRecommendation.API.Models;

namespace MovieRecommendation.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<UserRating> UserRatings { get; set; }
        public DbSet<MovieLensRating> MovieLensRatings { get; set; }
        public DbSet<WatchlistItem> WatchlistItems { get; set; }
    }
}