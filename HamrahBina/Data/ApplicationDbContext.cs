using System;
using System.Collections.Generic;
using System.Text;
using HamrahBina.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HamrahBina.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string,
            IdentityUserClaim<string>, ApplicationUserRole, IdentityUserLogin<string>,
            IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<HamrahBina.Models.Entities.ApplicationUserRole> ApplicationUserRole { get; set; }
        public DbSet<HamrahBina.Models.Entities.ApplicationRole> ApplicationRole { get; set; }
        public DbSet<HamrahBina.Models.Entities.ApiCallLog> ApiCallLogs { get; set; }
        public DbSet<HamrahBina.Models.Entities.OcrCallLog> OcrCallLogs { get; set; }
    }
}