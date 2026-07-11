using Microsoft.EntityFrameworkCore;
using RestaurantMenuAPI.Controllers;
using RestaurantMenuAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace RestaurantMenuAPI.Tests
{
    public class CategoriesControllerTests
    {
        private MenuDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<MenuDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new MenuDbContext(options);
        }

        [Fact]
        public async Task GetCategories_Returns_All_Categories_From_Database()
        {
            using var context = GetInMemoryDbContext();
            context.Categories.AddRange(
                new Category { Id = 1, Name = "Main Courses" },
                new Category { Id = 2, Name = "Pizza" }
            );
            await context.SaveChangesAsync();

            var controller = new CategoriesController(context);

            var result = await controller.GetCategories();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var categoryList = Assert.IsAssignableFrom<IEnumerable<Category>>(okResult.Value);
            Assert.Equal(2, categoryList.Count());
        }
    }
}