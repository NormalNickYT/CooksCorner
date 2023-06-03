using CooksCornerAPP.Data;
using CooksCornerAPP.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CooksCornerAPP.Controllers
{
    public class RecipesController : Controller
    {


        private readonly UserManager<ApplicationUser> userManager;
        private readonly IPasswordHasher<ApplicationUser> passwordHasher;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ApplicationDbContext _context;


        public RecipesController(UserManager<ApplicationUser> usrMgr, IPasswordHasher<ApplicationUser> pswhasher, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            userManager = usrMgr;
            passwordHasher = pswhasher;
            this.roleManager = roleManager;
            _context = context;
        }


        public async Task<IActionResult> RecipeDetails(int recipeId)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return RedirectToAction("Home");
            }

            var recipe = await _context.Recipes.Include(r => r.Owner).FirstOrDefaultAsync(r => r.Id == recipeId);

            if (recipe == null)
            {
                return RedirectToAction("Home");
            }

            TimeSpan.TryParse(recipe.PrepTime.ToString(), out TimeSpan prepTimeSpan);
            TimeSpan.TryParse(recipe.CookTime.ToString(), out TimeSpan cookTimeSpan);
            TimeSpan.TryParse(recipe.TotalTime.ToString(), out TimeSpan totalTimeSpan);
            TimeSpan.TryParse(recipe.AdditionalTime.ToString(), out TimeSpan additionalTimeSpan);

            var viewModel = new RecipeDetailViewModel
            {
                RecipeId = recipe.Id,
                Image = recipe.Image, 
                Category = recipe.Category, 
                Ingredients = recipe.Ingredients,
                Servings = recipe.Servings,
                Description = recipe.Description, 
                TotalTime = totalTimeSpan, 
                AdditionalTime = additionalTimeSpan,
                PrepTime = prepTimeSpan, 
                OwnerUserName = recipe.Owner.Name,
                CookTime = cookTimeSpan
            };

            return View(viewModel);
        }

    }
}
