using Microsoft.EntityFrameworkCore;
using RestaurantMenuAPI.Controllers;
using RestaurantMenuAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace RestaurantMenuAPI.Tests
{
    public class AuthUnitTests
    {
        private MenuDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<MenuDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new MenuDbContext(options);
        }

        private IConfiguration GetFakeConfiguration()
        {
            var inMemorySettings = new Dictionary<string, string> {
                {"Jwt:Key", "njxkuxiuuuvHBnjLNjNjlkKJnKLLKKLLKMK"},
                {"Jwt:Issuer", "RestaurantAPI"},
                {"Jwt:Audience", "RestaurantClient"}
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
        }

        [Fact]
        public async Task Register_User_Hashes_Password_Correctly()
        {
            using var context = GetInMemoryDbContext();
            var config = GetFakeConfiguration();
            var controller = new AuthController(context, config);

            var newUser = new User
            {
                Username = "test_kuvar",
                Password = "MojaLozinka123",
                Role = "User"
            };

            var result = await controller.Register(newUser);

            var savedUser = await context.Users.FirstOrDefaultAsync(u => u.Username == "test_kuvar");

            Assert.NotNull(savedUser);
            Assert.NotEqual("MojaLozinka123", savedUser.Password);
            Assert.StartsWith("$2", savedUser.Password);
        }

        [Fact]
        public async Task Login_Return_JWT_Token()
        {
            using var context = GetInMemoryDbContext();
            var config = GetFakeConfiguration();

            var existingUser = new User { Username = "kuvar_marko", Password = BCrypt.Net.BCrypt.HashPassword("Sifra123!"), Role = "Admin" };
            context.Users.Add(existingUser);
            await context.SaveChangesAsync();

            var controller = new AuthController(context, config);
            var loginData = new User { Username = "kuvar_marko", Password = "Sifra123!" };

            var result = await controller.Login(loginData);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseValue = okResult.Value;
            Assert.NotNull(responseValue);

            string tokenValue = "";

            if (responseValue is string tokenString)
            {
                tokenValue = tokenString;
            }
            else
            {
                var property = responseValue.GetType().GetProperty("token") ?? responseValue.GetType().GetProperty("Token");
                if (property != null)
                {
                    tokenValue = property.GetValue(responseValue) as string;
                }
            }

            Assert.False(string.IsNullOrEmpty(tokenValue), "Token was not found in the response object.");
        }

        [Fact]
        public async Task Username_Already_Exists()
        {
            using var context = GetInMemoryDbContext();
            var config = GetFakeConfiguration();

            context.Users.Add(new User { Username = "pero", Password = "NekaSifra1!", Role = "User" });
            await context.SaveChangesAsync();

            var controller = new AuthController(context, config);

            var duplicate = new User { Username = "pero", Password = "DrugaSifra123!", Role = "User" };

            var result = await controller.Register(duplicate);

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}