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

                if (!_context.Users.Any())
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

                    var result = await _userManager.CreateAsync(user, "123456");
                    if (result.Succeeded)
                    {
                        user.EmailConfirmed = true;
                        await _userManager.UpdateAsync(user);
                    }
                }

                if (!_context.Roles.Any())
                {
                    var adminRole = new ApplicationRole
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Admin",
                        NormalizedName = "ADMIN"
                    };
                    var createAdminRoleResult = await _roleManager.CreateAsync(adminRole);

                    var userRole = new ApplicationRole
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Uer",
                        NormalizedName = "USER"
                    };
                    var createUserRoleResult = await _roleManager.CreateAsync(userRole);

                    if (createAdminRoleResult.Succeeded && createUserRoleResult.Succeeded)
                    {
                        var user = await _userManager.FindByNameAsync("admin@HamrahBina.com");
                        await _userManager.AddToRoleAsync(user, adminRole.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
