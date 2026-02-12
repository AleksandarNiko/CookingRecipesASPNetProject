using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace CookingRecipes.Data.Models
{
    public class User
    {
        [Key]
        public  int UserID { get; set; }

        [Required]
        [MaxLength(255)]
        public string Username { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        public string Email { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        public string Role { get; set; } = null!;

        public  DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public IEnumerable<Recipe> Recipes { get; set; } = new List<Recipe>();

        public IEnumerable<Comment> Comments { get; set; } = new List<Comment>();
    }
}
