using CooksCornerAPP.Data;
using CooksCornerAPP.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CooksCornerAPP.ViewModels
{
    public class FavoriteRecipesViewModel
    {

        public string Id { get; set; }
        public ApplicationUser User { get; set; }
        public List<UserRecipesFavorites> FavoriteRecipes { get; set; }

    }
}
