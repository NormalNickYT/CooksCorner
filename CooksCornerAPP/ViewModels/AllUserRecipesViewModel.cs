using CooksCornerAPP.Data;
using CooksCornerAPP.Models;

namespace CooksCornerAPP.ViewModels
{
    public class AllUserRecipesViewModel
    {

        public int RecipeId { get; set; }
        public string RecipeName { get; set; }
        public string OwnerUserName { get; set; }
        public byte[] Image { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public ApplicationUser Owner {get; set;}
    }
}
