 using CooksCornerAPP.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace CooksCornerAPP.Data
{
    public class ApplicationUser : IdentityUser
    {

        public string Name { get; set; }

        public byte[]? Image { get; set; }

        public ICollection<UserRecipesFavorites> FavoriteRecipes { get; set; }

        public ICollection<Recipes> CreatedRecipes { get; set; }

        public string? ConnectionId { get; set; }

    }
}
