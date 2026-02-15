using CookingRecipes.Data;
using CookingRecipes.Data.Models;
using CookingRecipes.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CookingRecipes.Services.Implementations
{
    public class CategoriesService : ICategoryService
    {
        private readonly CookingRecipesDbContext context;

        public CategoriesService(CookingRecipesDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await context.Categories.ToListAsync();
        }

        public async Task CreateAsync(Category category)
        {
            await context.Categories.AddAsync(category);
            await context.SaveChangesAsync();
        }
    }
}
