using CooksCornerAPP.Data;
using CooksCornerAPP.Models;
using CooksCornerAPP.Models.DTO;
using CooksCornerAPP.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System.Data;
using System.Drawing;
using System.Net;
using System.Runtime.Intrinsics.X86;
using System.Security.Claims;

namespace CooksCornerAPP.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {

        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext _context;
        private IPasswordHasher<ApplicationUser> passwordHasher;


        public ProfileController(UserManager<ApplicationUser> usrMgr, ApplicationDbContext context, IPasswordHasher<ApplicationUser> passwordHasher)
        {
            userManager = usrMgr;
            _context = context;
            this.passwordHasher = passwordHasher;
        }

        public IActionResult Profile()
        {
            string userId = userManager.GetUserId(HttpContext.User);
            ApplicationUser user = userManager.FindByIdAsync(userId).Result;

            if (user == null)
            {
                return RedirectToAction("Home", "Home");

            }

            return View(user);
        }

        // Redirects me to the correct page
        [HttpGet]
        public async Task<IActionResult> EditProfile(string id)
        {
            ApplicationUser user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return RedirectToAction("Profile");
            }
            
            var viewModel = new EditProfileViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name
            };

            return View(viewModel);

        }

        // Changes the profile page data where users gives specific parameters with the form details
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(string id, string name, string email)
        {
            ApplicationUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                if (!string.IsNullOrEmpty(email))
                    user.Email = email;
                else
                    TempData["msg"] = "Email cannot be empty";
                    ModelState.AddModelError("", "Email cannot be empty");

                if (!string.IsNullOrEmpty(name))
                    user.Name = name;
                else
                    TempData["msg"] = "Name cannot be empty";
                    ModelState.AddModelError("", "Name cannot be empty");


                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(name))
                {
                    IdentityResult result = await userManager.UpdateAsync(user);
                    if (result.Succeeded)
                        return RedirectToAction("Profile");
                    else
                        TempData["msg"] = "Something went wrong";
                        Errors(result);
                }
            }
            else
            {
                ModelState.AddModelError("", "User Not Found");
                return RedirectToAction("EditProfile");
            }


            return RedirectToAction("Profile");
        }

        private void Errors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }

        [HttpPost]
        public async Task<IActionResult> AddProfilePicture(string id, IFormFile file)
        {
            ApplicationUser user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            if (user != null)
            {
                if (file != null && file.Length > 0)
                {
                    if (!file.ContentType.StartsWith("image/"))
                    {
                        ModelState.AddModelError("", "Please upload an image file.");
                        return View("Profile", "Profile");
                    }
                    // Read the contents of the uploaded file into a byte array
                    using (var ms = new MemoryStream())
                    {
                        await file.CopyToAsync(ms);
                        user.Image = ms.ToArray();
                    }
                    await userManager.UpdateAsync(user);
                }
                else
                {
                    ModelState.AddModelError("", "Please select an image to upload.");
                    return View("Profile", "Profile");
                }
            }
            return RedirectToAction("Profile", "Profile");
        }


        public async Task<IActionResult> ManageRecipes(string id)
        {

            ApplicationUser user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return RedirectToAction("Profile");
            }

            var createdRecipes = _context.Recipes.Where(r => r.OwnerId == id).ToList();

            var viewModel = new ManageRecipesViewModel
            {
                Id = user.Id,
                User = user,
                CreatedRecipes = createdRecipes
            };

            return View(viewModel);
        }

        public IActionResult AddRecipes(string id)
        {

            var user = userManager.FindByIdAsync(id).Result;
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var viewModel = new AddRecipeViewModel
            {
                User = user
            };

            return View(viewModel);

        }


        [HttpPost]
        public async Task<IActionResult> CreateRecipes(string id, AddRecipeViewModel model, IFormFile file)
        {
            var status = new Status();

            // Get the user by their ID
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                
                return NotFound();
            }

            var prepTimeSpan = TimeSpan.Zero;
            var cookTimeSpan = TimeSpan.Zero;
            var totalTimeSpan = TimeSpan.Zero;
            var additionalTimeSpan = TimeSpan.Zero;

            if (model.Recipe != null) {

                TimeSpan.TryParse(model.Recipe.PrepTime.ToString(), out prepTimeSpan);
                TimeSpan.TryParse(model.Recipe.CookTime.ToString(), out cookTimeSpan);
                TimeSpan.TryParse(model.Recipe.TotalTime.ToString(), out totalTimeSpan);
                TimeSpan.TryParse(model.Recipe.AdditionalTime.ToString(), out additionalTimeSpan);
            }

            byte[] image = null;
            if (file != null && file.Length > 0)
            {
                if (!file.ContentType.StartsWith("image/"))
                {
                    status.Message = "Please upload an image file.";
                    TempData["msg"] = status.Message;
                    ModelState.AddModelError("", "Please upload an image file.");

                    return RedirectToAction("Profile");
                }
                // Read the contents of the uploaded file into a byte array
                using (var ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms);
                    image = ms.ToArray();
                }
            }

            // Create the new recipe object
            var recipe = new Recipes
            {
                Name = model.Recipe.Name,
                Description = model.Recipe.Description,
                Category = model.Recipe.Category,
                PrepTime = prepTimeSpan,
                CookTime = cookTimeSpan,
                TotalTime = totalTimeSpan,
                Servings = model.Recipe.Servings,
                Image = image,
                OwnerId = user.Id,
                Owner = user,
                AdditionalTime = additionalTimeSpan,
                Ingredients = model.Recipe.Ingredients,
            };

            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();

            return RedirectToAction("ManageRecipes", new { id = user.Id});

        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword(string id)
        {
            ApplicationUser user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return RedirectToAction("Profile");
            }

            return View(user);

        }

        [HttpPost]
        public async Task<IActionResult> UpdatePassword(string id, string password)
        {
            ApplicationUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                if (!string.IsNullOrEmpty(password))
                {
                    user.PasswordHash = passwordHasher.HashPassword(user, password);
                    IdentityResult result = await userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        TempData["msg"] = "Succeeded";
                        return RedirectToAction("ChangePassword", new { id = user.Id });
                    }
                    else
                        TempData["msg"] = "Password is leeg";
                }
            }
            else
            {
                ModelState.AddModelError("", "User Not Found");
                return RedirectToAction("Profile");
            }

            return RedirectToAction("Profile");

        }

        public async Task<IActionResult> FavoriteRecipes(string id)
        {

            ApplicationUser user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return RedirectToAction("Profile");
            }

            var favoriteRecipes = _context.UserRecipesFavorites
            .Include(f => f.Recipe)
            .Where(r => r.UserId == id)
            .ToList();

            var viewModel = new FavoriteRecipesViewModel
            {
                Id = user.Id,
                User = user,
                FavoriteRecipes = favoriteRecipes
            };

            return View(viewModel);
        }

        public async Task<IActionResult> RemoveFavoriteRecipe(int recipeId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var favorite = await _context.UserRecipesFavorites
             .FirstOrDefaultAsync(f => f.UserId == userId && f.RecipeId == recipeId);

            if (favorite == null)
            {
                return RedirectToAction("Profile");
            }

            _context.UserRecipesFavorites.Remove(favorite);
            await _context.SaveChangesAsync();

            return RedirectToAction("FavoriteRecipes", new { id = userId });
        }

        public async Task<IActionResult> RemoveRecipe(int recipeId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var recipe = await _context.Recipes.FirstOrDefaultAsync(f => f.OwnerId == userId && f.Id == recipeId);

            if (recipe == null)
            {
                return RedirectToAction("Profile");
            }
            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();
            return RedirectToAction("ManageRecipes", new { id = userId });
        }

    }
}
