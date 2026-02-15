using CookingRecipes.Data;
using CookingRecipes.Data.Models;
using CookingRecipes.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CookingRecipes.Services.Implementations
{
    public class RecipeService : IRecipeService
    {
        private readonly CookingRecipesDbContext context;

        public RecipeService(CookingRecipesDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Recipe>> GetAllAsync()
        {
            return await context.Recipes
                .Include(r => r.RecipeCategories)
                .ThenInclude(rc => rc.Category)
                .ToListAsync();
        }

        public async Task<Recipe?> GetByIdAsync(int id)
        {
            return await context.Recipes
                .Include(r => r.RecipeCategories)
                .ThenInclude(rc => rc.Category)
                .FirstOrDefaultAsync(r => r.RecipeID == id);
        }

        public async Task CreateAsync(Recipe recipe, List<int> categoryIds)
        {
            recipe.RecipeCategories = categoryIds
                .Select(id => new RecipeCategory
                {
                    CategoryID = id
                }).ToList();

            await context.Recipes.AddAsync(recipe);

            await context.SaveChangesAsync();
        }
    }
}
