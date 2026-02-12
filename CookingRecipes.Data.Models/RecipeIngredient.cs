using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CookingRecipes.Data.Models
{
    public class RecipeIngredient
    {
        [Key]
        public  int RecipeIngredientID { get; set; }

        public  int RecipeID { get; set; }

        public  int IngredientID { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public  decimal Quantity { get; set; }

        public  Recipe Recipe { get; set; }

        public  Ingredient Igredient { get; set; }
    }
}