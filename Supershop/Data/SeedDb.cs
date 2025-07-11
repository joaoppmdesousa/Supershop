﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
            await _context.Database.MigrateAsync();

            await _userHelper.CheckRoleAsync("Admin");
            await _userHelper.CheckRoleAsync("Customer");

            if (!_context.Countries.Any())
            {
                var cities = new List<City>
                {
                    new City { Name = "Setúbal" },
                    new City { Name = "Porto" },
                    new City { Name = "Lisboa" }
                };

                _context.Countries.Add(new Country
                {
                    Name = "Portugal",
                    Cities = cities
                });

                await _context.SaveChangesAsync();
            }


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
                    Address = "Avenida José Mourinho 45",
                    CityId = _context.Countries.FirstOrDefault().Id,
                    City = _context.Countries.FirstOrDefault().Cities.FirstOrDefault()
                };

                var result = await _userHelper.AddUserAsync(user, "123456");
                if(result != IdentityResult.Success)
                {
                   throw new InvalidOperationException("Could not create the user in seeder");
                }

                await _userHelper.AddUserToRoleAsync(user, "Admin");
                var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                await _userHelper.ConfirmEmailAsync(user, token);
            }

            var isInRole = await _userHelper.IsUserInRoleAsync(user, "Admin");
            if (!isInRole)
            {
                await _userHelper.AddUserToRoleAsync(user, "Admin");
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
