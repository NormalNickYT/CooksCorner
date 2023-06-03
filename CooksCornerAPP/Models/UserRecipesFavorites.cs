using CooksCornerAPP.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CooksCornerAPP.Models
{
    public class UserRecipesFavorites
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("UserId")]
        public string? UserId { get; set; }
        public ApplicationUser User { get; set; }

        [ForeignKey("RecipeId")]
        public int? RecipeId { get; set; }
        public Recipes Recipe { get; set; }

    }
}
