using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CookingRecipes.Data.Models
{
    public class Recipe
    {
        [Key]
        public  int RecipeID { get; set; }

        [Required]
        [MaxLength(255)]
        public  string Title { get; set; }

        [MaxLength(255)]
        public  string? Description { get; set; }

        public  int PreparationTime { get; set; }

        public  int CookingTime { get; set; }

        public  int Servings { get; set; }

        [ForeignKey("User")]
        public  int CreatedBy { get; set; }

        public  DateTime CreatedAt { get; set; }

        public  User? User { get; set; }

        public IEnumerable<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();

        public IEnumerable<RecipeCategory> RecipeCategories { get; set; } = new List<RecipeCategory>();

        public IEnumerable<Comment> Comments { get; set; } = new List<Comment>();
    }
}
