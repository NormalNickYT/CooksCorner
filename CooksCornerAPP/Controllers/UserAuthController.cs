using CooksCornerAPP.Models.DTO;
using CooksCornerAPP.Repositories.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Org.BouncyCastle.Crypto;
using EmailService;
using CooksCornerAPP.Data;
using Microsoft.AspNetCore.Http.Extensions;
using CooksCornerAPP.Services;

namespace CooksCornerAPP.Controllers
{
    public class UserAuthController : Controller
    {
        private IUserAuthenticationService _authService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IEmailSender _emailService;
        private readonly GoogleCaptchaService _captchaService;

        public UserAuthController(IUserAuthenticationService AuthService, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, IEmailSender emailService, GoogleCaptchaService captchaService)
        {

            this._authService = AuthService;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
            this._emailService = emailService;
            this._captchaService = captchaService;
        }

        public IActionResult Register()
        {

            if (User.Identity != null)
            {
                if (User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Home", "Home");
                }
            }

            return View();
        }

        public IActionResult Login()
        {
            if (User.Identity != null)
            {
                if (User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Home", "Home");
                }
            }
           
            return View();
           
        }


        [HttpPost]
        public async Task<IActionResult> Register(Registration model)
        {
            model.Role = "user";
            var result = await _authService.RegisterAsync(model);
            TempData["msg"] = result.Message;
            return RedirectToAction(nameof(Register));
        }

 
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPassword forgotPasswordModel)
        {
            if (!ModelState.IsValid)
                return View(forgotPasswordModel);

            var user = await userManager.FindByEmailAsync(forgotPasswordModel.Email);
            if (user == null)
            {
                TempData["msg"] = "Email doesn't exist";
                return RedirectToAction(nameof(ForgotPassword));
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var tokenExpirationTime = TimeSpan.FromHours(1);
            var confirmationLink = Url.Action(nameof(ResetPassword), "UserAuth", new { token, email = user.Email }, Request.Scheme);
            var message = new Message(new string[] { user.Email! }, "Confirmation email link", confirmationLink!);
            _emailService.SendEmail(message);


            return RedirectToAction(nameof(ForgotPasswordConfirmation));
        }

        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            var model = new ResetPassword { Token = token, Email = email };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPassword resetPasswordModel)
        {
            if (!ModelState.IsValid)
                return View(resetPasswordModel);

            var user = await userManager.FindByEmailAsync(resetPasswordModel.Email);
            if (user == null)
                RedirectToAction(nameof(ResetPasswordConfirmation));

            var resetPassResult = await userManager.ResetPasswordAsync(user, resetPasswordModel.Token, resetPasswordModel.Password);
            if (!resetPassResult.Succeeded)
            {
                foreach (var error in resetPassResult.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }

                return View();
            }

            return RedirectToAction(nameof(ResetPasswordConfirmation));
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(Login model)
        {

            var captchaResult = await _captchaService.VerifyToken(model.Token);

            if (!captchaResult)
            {
                TempData["msg"] = "You might be a bot :)";
                return RedirectToAction(nameof(Login));
            }

            if (!ModelState.IsValid)
                return View(model);

            var result = await _authService.LoginAsync(model);
            if (result.StatusCode == -1)
            {
                TempData["msg"] = "Invalid Email";
                return RedirectToAction(nameof(Login));
            }
            if (result.StatusCode == -2)
            {
                TempData["msg"] = "Invalid Password";
                return RedirectToAction(nameof(Login));
            }
            if (result.StatusCode == 1)
                return RedirectToAction("Home", "Home");
            else
            {
                TempData["msg"] = "Could not logged in..";
                return RedirectToAction(nameof(Login));
            }
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return RedirectToAction(nameof(Login));
        }




    }
}
