using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CookingRecipes.Data.Models
{
    public class Comment
    {
        [Key]
        public  int CommentID { get; set; }

        public  int RecipeID { get; set; }

        public  int UserID { get; set; }

        [Required]
        [MaxLength(255)]
        public  string Content { get; set; }= null!;

        public  DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public  Recipe Recipe { get; set; }

        public User User { get; set; }
    }
}
