using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CookingRecipes.Data.Models
{
    public class RecipeCategory
    {
        [Key]
        public int RecipeCategoryID { get; set; }

        public  int RecipeID { get; set; }
        public  int CategoryID { get; set; }

        public  Recipe Recipe { get; set; }

        public  Category Category { get; set; }
    }
}
