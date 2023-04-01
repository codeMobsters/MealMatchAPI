using MealMatchAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MealMatchAPI.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<FavoriteRecipe> FavoriteRecipes { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<Follower> Followers { get; set; }

}