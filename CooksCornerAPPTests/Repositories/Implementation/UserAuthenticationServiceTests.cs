using Microsoft.VisualStudio.TestTools.UnitTesting;
using CooksCornerAPP.Repositories.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Microsoft.AspNetCore.Identity;
using CooksCornerAPP.Data;
using CooksCornerAPP.Repositories.Abstract;
using Microsoft.AspNetCore.Http;
using CooksCornerAPP.Models.DTO;

namespace CooksCornerAPP.Repositories.Implementation.Tests
{
    [TestClass()]
    public class UserAuthenticationServiceTests
    {
        private Mock<UserManager<ApplicationUser>> userManagerMock;
        private Mock<RoleManager<IdentityRole>> roleManagerMock;
        private Mock<SignInManager<ApplicationUser>> signInManagerMock;
        private IUserAuthenticationService userAuthenticationService;

        [TestInitialize]
        public void TestInitialize()
        {
            userManagerMock = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
            roleManagerMock = new Mock<RoleManager<IdentityRole>>(Mock.Of<IRoleStore<IdentityRole>>(), null, null, null, null);
            signInManagerMock = new Mock<SignInManager<ApplicationUser>>(userManagerMock.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(), null, null, null, null);
            userAuthenticationService = new UserAuthenticationService(userManagerMock.Object, signInManagerMock.Object, roleManagerMock.Object);
        }

        [TestMethod()]
        public async Task RegisterAsync_WithValidModel_ReturnsSuccessStatus()
        {
            // Arrange
            var model = new Registration
            {
                Username = "testusers",
                Email = "testuser@gmail.com",
                Name = "Test User",
                Password = "TestPassword1!"
            };

            userManagerMock.Setup(x => x.FindByNameAsync(model.Username)).ReturnsAsync((ApplicationUser)null);
            userManagerMock.Setup(x => x.FindByEmailAsync(model.Email)).ReturnsAsync((ApplicationUser)null);
            userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), model.Password)).ReturnsAsync(IdentityResult.Success);
            userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new[] { "user" });

            // Act
            var result = await userAuthenticationService.RegisterAsync(model);

            // Assert
            Assert.AreEqual(1, result.StatusCode);
            Assert.AreEqual("You have registered successfully", result.Message);

            // Verifieren dat alle methodes wel gecalled zijn van het object
            userManagerMock.Verify(x => x.FindByNameAsync(model.Username), Times.Once);
            userManagerMock.Verify(x => x.FindByEmailAsync(model.Email), Times.Once);
            userManagerMock.Verify(x => x.CreateAsync(It.IsAny<ApplicationUser>(), model.Password), Times.Once);
            userManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), "user"), Times.Once);
        }

        [TestMethod()]
        public async Task LoginAsync_ReturnsSuccessStatusWhenCredentialsAreValid()
        {
            // Arrange
            var expectedStatus = new Status { StatusCode = 1, Message = "Logged in succesfully" };
            var loginModel = new Login { Email = "john.doe@example.com", Password = "Ditiseenwachtwoord123!" };
            var user = new ApplicationUser { UserName = "JohnDoe", Email = loginModel.Email };
            userManagerMock.Setup(u => u.FindByEmailAsync(loginModel.Email)).ReturnsAsync(user);
            userManagerMock.Setup(u => u.CheckPasswordAsync(user, loginModel.Password)).ReturnsAsync(true);
            userManagerMock.Setup(u => u.GetRolesAsync(user)).ReturnsAsync(new List<string>());
            signInManagerMock.Setup(s => s.PasswordSignInAsync(user, loginModel.Password, false, true)).ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            // Act
            var result = await userAuthenticationService.LoginAsync(loginModel);

            // Assert
            Assert.AreEqual(expectedStatus.StatusCode, result.StatusCode);
            Assert.AreEqual(expectedStatus.Message, result.Message);
        }
    }
}