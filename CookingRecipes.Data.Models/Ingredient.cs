using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CookingRecipes.Data.Models
{
    public class Ingredient
    {
        [Key]
        public  int IngredientID { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(4)]
        public string Unit { get; set; } = null!;

        public IEnumerable<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
    }
}
