using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Supershop.Data.Entities;
using Supershop.Helpers;


namespace Supershop.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private Random _random;

        public SeedDb(DataContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _random = new Random();
        }

        public async  Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();

            var user = await _userHelper.GetUserByEmailAsync("joaopedropsousa@gmail.com");

            if(user== null)
            {
                user = new User
                {
                    FirstName = "João Pedro",
                    LastName = "Sousa",
                    Email = "joaopedropsousa@gmail.com",
                    UserName = "joaopedropsousa@gmail.com",
                    PhoneNumber = "123456789",
                };

                var result = await _userHelper.AddUserAsync(user, "123456");
                if(result != IdentityResult.Success)
                {
                   throw new InvalidOperationException("Could not create the user in seeder");
                }
            }

            if (!_context.Products.Any())
            {
                AddProduct("IPhone X", user);
                AddProduct("Magic Mouse", user);
                AddProduct("IWatch Series 4", user);
                AddProduct("IPad Mini", user);
                await _context.SaveChangesAsync();
            }
        }

        private void AddProduct(string name, User user)
        {
            _context.Products.Add(new Product
            {
                Name = name,
                Price = _random.Next(1000),              
                IsAvailable = true,
                Stock = _random.Next(100),
                User = user
            });
        }
    }
}
