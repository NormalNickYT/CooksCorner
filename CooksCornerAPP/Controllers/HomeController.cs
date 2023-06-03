using CooksCornerAPP.Data;
using CooksCornerAPP.Hubs;
using CooksCornerAPP.Models;
using CooksCornerAPP.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;
using System.Security.Claims;

namespace CooksCornerAPP.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<RecipeHub> _recipeHubContext;


        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> UserManager, IHubContext<RecipeHub> recipehubcontext)
        {
            _recipeHubContext = recipehubcontext;
            _context = context;
            userManager = UserManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult ContactForm()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public async Task<IActionResult> Home()
        {

            var allRecipes = await _context.Recipes.Include(r => r.Owner).Select(r => new AllUserRecipesViewModel
            {
                RecipeId = r.Id,
                RecipeName = r.Name,
                OwnerUserName = r.Owner.Name,
                Image = r.Image,
                Category = r.Category,
                Description = r.Description

            }).ToListAsync();

            return View("Home", allRecipes);


        }

        [Authorize]
        public async Task<IActionResult> FavoriteRecipe(int recipeId)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await userManager.FindByIdAsync(userId);

            var recipe = await _context.Recipes.FindAsync(recipeId);
            if (recipe == null)
            {
                return RedirectToAction("Privacy");
            }

            var favorite = await _context.UserRecipesFavorites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.RecipeId == recipeId);

            if (favorite != null)
            {
                return Json(new { alreadyFavorited = true });
            }

            if (favorite == null)
            {
                favorite = new UserRecipesFavorites
                {
                    UserId = userId,
                    RecipeId = recipeId,
                    Recipe = recipe,
                    User = user
                };
                _context.UserRecipesFavorites.Add(favorite);
                await _context.SaveChangesAsync();

                var recipeOwner = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == recipe.OwnerId);

                if (recipeOwner != null)
                {

                    await _recipeHubContext.Clients.User(recipeOwner.Id).SendAsync("NotifyOwner", recipeId);

                }

            }

            return Json(new { alreadyFavorited = false });

        }
    }
}

