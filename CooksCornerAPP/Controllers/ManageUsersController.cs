using CooksCornerAPP.Data;
using CooksCornerAPP.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace CooksCornerAPP.Controllers
{
    [Authorize(Roles = "admin, moderator")]
    public class ManageUsersController : Controller
    {

        private readonly UserManager<ApplicationUser> userManager;
        private IPasswordHasher<ApplicationUser> passwordHasher;
        private readonly RoleManager<IdentityRole> roleManager;


        public ManageUsersController(UserManager<ApplicationUser> usrMgr, IPasswordHasher<ApplicationUser> pswhasher, RoleManager<IdentityRole> roleManager)
        {
            userManager = usrMgr;
            passwordHasher = pswhasher;
            this.roleManager = roleManager;
        }

        // Redirect naar de pagina van de juiste userId
        public async Task<IActionResult> EditUser(string id)
        {
            ApplicationUser user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return RedirectToAction("ListUsers");
            }

            var roles = roleManager.Roles.ToList();
            var viewModel = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Roles = roles,
                Password = user.PasswordHash
                
            };

            var userRole = await userManager.GetRolesAsync(user);
            viewModel.Role = userRole.FirstOrDefault();

            return View(viewModel); 

        }

        // Update de User zijn informatie

        [HttpPost]
        public async Task<IActionResult> UpdateUser(string id, string email, string password, string role)
        {
            ApplicationUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                if (!string.IsNullOrEmpty(email))
                    user.Email = email;
                else
                    ModelState.AddModelError("", "Email cannot be empty");

                if (!string.IsNullOrEmpty(password))
                    user.PasswordHash = passwordHasher.HashPassword(user, password);
                else
                    ModelState.AddModelError("", "Password cannot be empty");

                if (!string.IsNullOrEmpty(role))
                {
                    var currentRole = await userManager.GetRolesAsync(user);
                    await userManager.RemoveFromRoleAsync(user, currentRole.FirstOrDefault());

                    var result = await userManager.AddToRoleAsync(user, role);
                    if (!result.Succeeded)
                        ModelState.AddModelError("", "Failed to update role");
                }

                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
                {
                    IdentityResult result = await userManager.UpdateAsync(user);
                    if (result.Succeeded)
                        return RedirectToAction("ListUsers");
                    else
                        Errors(result);
                }


            }
            else
            {
                ModelState.AddModelError("", "User Not Found");
                return RedirectToAction("ListUsers");
            }

            var roles = roleManager.Roles.ToList();
            if (roles != null)
            {
                var viewModel = new EditUserViewModel
                {
                      Id = user.Id,
                      Email = user.Email,
                      Roles = roles
                };

                    return View(viewModel);
                }
                else
                {
                    ModelState.AddModelError("", "No roles found");
                }

            return RedirectToAction("ListUsers");

        }

        public async Task<IActionResult> ListUsers()
        {

            var users = userManager.Users;
            var viewModelList = new List<ManageUsersViewModel>();

            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault();

                var viewModel = new ManageUsersViewModel
                {
                    User = user,
                    Role = role
                };

                viewModelList.Add(viewModel);
            }


            return View(viewModelList);
        }

        private void Errors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            ApplicationUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                    return RedirectToAction("ListUsers");
                else
                    Errors(result);
            }
            else
                ModelState.AddModelError("", "User Not Found");
            return View("ListUsers", userManager.Users);
        }

    }
}
