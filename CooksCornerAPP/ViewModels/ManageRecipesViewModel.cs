using CooksCornerAPP.Data;
using CooksCornerAPP.Models;

namespace CooksCornerAPP.ViewModels
{
    public class ManageRecipesViewModel
    {

        public string Id { get; set; }
        public ApplicationUser User { get; set; }
        public List<Recipes> CreatedRecipes { get; set; }

    }
}
