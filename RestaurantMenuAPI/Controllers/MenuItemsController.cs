using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantMenuAPI.Models;

namespace RestaurantMenuAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuItemsController : ControllerBase
    {
        private readonly MenuDbContext _context;

        public MenuItemsController(MenuDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetMenuItems()
        {
            var menuItems = await _context.MenuItems.ToListAsync();
            return Ok(menuItems);
        }

        [HttpPost]
        public async Task<IActionResult> AddMenuItem(MenuItem menuItem)
        {
            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();

            return Ok(menuItem);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMenuItem(int id, MenuItem updatedItem)
        {
            if (id != updatedItem.Id)
            {
                return BadRequest("The ID in the URL and the ID in the data do not match.");
            }

            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
            {
                return NotFound("The menu item you want to update does not exist.");
            }

            menuItem.Name = updatedItem.Name;
            menuItem.Description = updatedItem.Description;
            menuItem.Price = updatedItem.Price;
            menuItem.CategoryId = updatedItem.CategoryId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
            {
                return NotFound("The menu item you want to delete does not exist.");
            }

            _context.MenuItems.Remove(menuItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}