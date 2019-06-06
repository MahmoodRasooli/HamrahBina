using HamrahBina.Models.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HamrahBina.Data
{
    public class DataSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public DataSeeder(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task SeedAsync()
        {
            try
            {
                _context.Database.EnsureCreated();
                if (!_context.Roles.Any(p => p.Name.ToLower() == "Admin".ToLower()))
                {
                    var adminRole = new ApplicationRole
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Admin",
                        NormalizedName = "ADMIN"
                    };
                    var createAdminRoleResult = await _roleManager.CreateAsync(adminRole);
                }

                if (!_context.Roles.Any(p => p.Name.ToLower() == "User".ToLower()))
                {
                    var userRole = new ApplicationRole
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "User",
                        NormalizedName = "USER"
                    };
                    var createUserRoleResult = await _roleManager.CreateAsync(userRole);
                }

                if (!_context.Users.Any(p => p.UserName.ToLower() == "admin@HamrahBina.com".ToLower()))
                {
                    var user = new ApplicationUser
                    {
                        FirstName = "مدیر",
                        LastName = "سیستم",
                        PhoneNumber = "----",
                        Email = "admin@HamrahBina.com",
                        NationalId = "0000000000",
                        UserName = "admin@HamrahBina.com",
                        CreateDate = DateTime.Now,
                        BrithDate = DateTime.Now,
                        PhoneNumberConfirmed = true,
                        EmailConfirmed = true,
                    };

                    var result = await _userManager.CreateAsync(user, "!QAZ@WSX#EDC");
                    if (result.Succeeded)
                    {
                        user.EmailConfirmed = true;
                        await _userManager.UpdateAsync(user);
                    }
                }

                var adminUser = await _userManager.FindByNameAsync("admin@HamrahBina.com");

                if (!await _userManager.IsInRoleAsync(adminUser, "Admin"))
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
