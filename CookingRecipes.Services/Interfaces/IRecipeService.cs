using CookingRecipes.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CookingRecipes.Services.Interfaces
{
    public interface IRecipeService
    {
        Task<IEnumerable<Recipe>> GetAllAsync();

        Task<Recipe?> GetByIdAsync(int id);

        Task CreateAsync(Recipe recipe, List<int> categoryIds);
    }
}
