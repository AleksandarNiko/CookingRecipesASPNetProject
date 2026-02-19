using CookingRecipes.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;

namespace CookingRecipes.Data
{
    public class CookingRecipesDbContext : DbContext
    {
        public CookingRecipesDbContext(DbContextOptions<CookingRecipesDbContext> options)
        : base(options)
        {
        }

        public  DbSet<Category> Categories { get; set; }

        public  DbSet<Comment> Comments { get; set; }

        public  DbSet<Ingredient> Ingredients { get; set; }

        public  DbSet<Recipe> Recipes { get; set; }

        public  DbSet<RecipeCategory> RecipeCategories { get; set; }

        public  DbSet<RecipeIngredient> RecipeIngredients { get; set; }

        public  DbSet<User> Users { get; set; }
    }
}
