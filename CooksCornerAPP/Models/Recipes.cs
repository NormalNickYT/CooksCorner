using CooksCornerAPP.Data;
using System.ComponentModel.DataAnnotations;

namespace CooksCornerAPP.Models
{
    public class Recipes
    {


        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Category { get; set; }

        public float Rating { get; set; }

        [Required]
        public string OwnerId { get; set; }
        public ApplicationUser Owner { get; set; }

        public ICollection<UserRecipesFavorites> FavoritedBy { get; set; }

        [Required]
        public byte[]? Image { get; set; }

        [Required]
        public TimeSpan PrepTime { get; set; }

        [Required]
        public TimeSpan CookTime { get; set; }

        public TimeSpan AdditionalTime { get; set; }

        [Required]
        public TimeSpan TotalTime { get; set; }

        [Required]
        public int Servings { get; set; }

        [Required]
        public string Ingredients { get; set; }

    }
}
