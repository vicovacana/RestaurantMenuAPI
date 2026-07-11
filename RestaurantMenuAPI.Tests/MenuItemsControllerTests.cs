using Microsoft.EntityFrameworkCore;
using RestaurantMenuAPI.Controllers;
using RestaurantMenuAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace RestaurantMenuAPI.Tests
{
    public class MenuItemsControllerTests
    {
        private MenuDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<MenuDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new MenuDbContext(options);
        }

        [Fact]
        public async Task GetMenuItems_Returns_Empty_List_When_Menu_Is_Empty()
        {
            using var context = GetInMemoryDbContext();
            var controller = new MenuItemsController(context);

            var result = await controller.GetMenuItems();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var menuItemList = Assert.IsAssignableFrom<IEnumerable<MenuItem>>(okResult.Value);
            Assert.Empty(menuItemList);
        }
    }
}