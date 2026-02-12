using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CookingRecipes.Data.Models
{
    public class Category
    {
        [Key]
        public  int  CategoryID { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = null!;

        public IEnumerable<RecipeCategory> RecipeCategories { get; set; } = new List<RecipeCategory>();
    }
}
