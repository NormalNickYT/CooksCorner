using Microsoft.VisualStudio.TestTools.UnitTesting;
using CooksCornerAPP.Data;
using Microsoft.AspNetCore.Identity;
using Moq;
using CooksCornerAPP.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using CooksCornerAPP.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace CooksCornerAPP.Controllers.Tests
{
    [TestClass()]
    public class ProfileControllerTests
    {
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private Mock<ApplicationDbContext> _mockDbContext;


        [TestInitialize]
        public void Initialize()
        {
            // Set up mock user manager
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            // Set up mock database context
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
             .UseInMemoryDatabase(databaseName: "test_database")
             .Options;
            _mockDbContext = new Mock<ApplicationDbContext>(options);
        }

        private Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            return new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
        }

        [TestMethod]
        public async Task CreateRecipes_ReturnsNotFound_WhenUserNotFound()
        {

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "test_database").Options;
            var context = new ApplicationDbContext(options);
            var userManager = MockUserManager<ApplicationUser>();
            var controller = new ProfileController(userManager.Object, context, null);
            var model = new AddRecipeViewModel();
            var id = "invalid-id";

            context.Users.Add(new ApplicationUser { Id = id });

            var result = await controller.CreateRecipes(id, model, null);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task CreateRecipes_ReturnsRedirect_WhenRecipeIsCreated()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "test_database")
                .Options;
            var context = new ApplicationDbContext(options);
            var controller = new ProfileController(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context), null, null, null, null, null, null, null, null), context, null);
            var id = "valid-id";

            string path = Path.Combine("..","..", "..", "..", "CooksCornerAPP", "wwwroot", "images", "Develop.png");
            string absolutePath = Path.GetFullPath(path);
            var stream = new FileStream(path, FileMode.Open);
            var fileMock = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(path));
            fileMock.Headers = new HeaderDictionary();
            fileMock.ContentType = "image/png";

            using var memoryStream = new MemoryStream();
            await fileMock.CopyToAsync(memoryStream);
            byte[] imageBytes = memoryStream.ToArray();

            var user = new ApplicationUser { 
                
                Name = "User",
                Id = id 
            
            };
            context.Users.Add(user);

            var model = new AddRecipeViewModel
            {
                Recipe = new Recipes
                {
                    Name = "Test",
                    Description = "Test",
                    Category = "Test",
                    PrepTime = TimeSpan.Parse("5:00"),
                    CookTime = TimeSpan.Parse("5:00"),
                    TotalTime = TimeSpan.Parse("5:00"),
                    Servings = 5,
                    Image = imageBytes,
                    OwnerId = id,
                    Owner = user,
                    AdditionalTime = TimeSpan.Parse("5:00"),
                    Ingredients = "Tekst"
                }
            };

            var result = await controller.CreateRecipes(id, model, fileMock);
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            Assert.AreEqual("ManageRecipes", ((RedirectToActionResult)result).ActionName);
            Assert.AreEqual(id, ((RedirectToActionResult)result).RouteValues["id"]);
        }




    }
}