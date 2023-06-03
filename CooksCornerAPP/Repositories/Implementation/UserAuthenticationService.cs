
using CooksCornerAPP.Data;
using CooksCornerAPP.Models.DTO;
using CooksCornerAPP.Repositories.Abstract;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Identity.UI.Services;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Security.Policy;

namespace CooksCornerAPP.Repositories.Implementation
{
    public class UserAuthenticationService : IUserAuthenticationService
    {

        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public UserAuthenticationService(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
        }

        public async Task<Status> RegisterAsync(Registration model)
        {
            var status = new Status();
            var userExists = await userManager.FindByNameAsync(model.Username);
            var emailExists = await userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
            {
                status.StatusCode = 0;
                status.Message = "UserName already exist";
                return status;
            }

            if (emailExists != null)
            {
                status.StatusCode = 0;
                status.Message = "Email already exists";
                return status;
            }

            ApplicationUser user = new ApplicationUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                Name = model.Name,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
            };
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                status.StatusCode = 0;
                status.Message = "User creation failed";
                return status;
            }

            if (result.Succeeded)
            {
                if (user.Email.ToLower() == "admin@hotmail.com")
                {
                    await userManager.AddToRoleAsync(user, "admin");
                }
                else
                {
                    await userManager.AddToRoleAsync(user, "user");
                }
            }

            status.StatusCode = 1;
            status.Message = "You have registered successfully";
            return status;
        }

        public async Task<Status> ConfirmEmail(string token, string email)
        {
            var status = new Status();
            var user = await userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await userManager.ConfirmEmailAsync(user, token);
                if (!result.Succeeded)
                {
                    status.StatusCode = 0;
                    status.Message = "This user doesn't exist";
                    return status;
                }
            }
            status.StatusCode = 1;
            status.Message = "You have registered successfully";
            return status;
        }


        public async Task<Status> LoginAsync(Login model)
        {
            var status = new Status();
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {

                status.StatusCode = -1;
                status.Message = "Invalid email";
                return status;

            }

            if (!await userManager.CheckPasswordAsync(user, model.Password))
            {
                status.StatusCode = -2;
                status.Message = "Invalid Password";
                return status;
            }

            var signInResult = await signInManager.PasswordSignInAsync(user, model.Password, false, true);
            if (signInResult.Succeeded)
            {
                var userRoles = await userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }
                status.StatusCode = 1;
                status.Message = "Logged in succesfully";

            } else if (signInResult.IsLockedOut)
            {
                status.StatusCode = -3;
                status.Message = "User is locked out";
            } else
            {
                status.StatusCode = 0;
                status.Message = "Error on logging in";
            }
                return status;
        }
        public async Task LogoutAsync()
        {
            await signInManager.SignOutAsync();

        }
    }
}
