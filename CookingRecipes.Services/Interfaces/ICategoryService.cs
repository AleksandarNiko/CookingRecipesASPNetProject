using CookingRecipes.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CookingRecipes.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllAsync();

        Task CreateAsync(Category category);
    }
}
