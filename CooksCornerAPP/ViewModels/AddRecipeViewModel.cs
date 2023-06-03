using CooksCornerAPP.Data;
using CooksCornerAPP.Models;

namespace CooksCornerAPP.ViewModels
{
    public class AddRecipeViewModel
    {
       public ApplicationUser User { get; set; }
       public Recipes Recipe { get; set; }
    }

}
