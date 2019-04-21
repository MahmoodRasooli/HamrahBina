using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HamrahBina.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using HamrahBina.Models.Entities;
using HamrahBina.Common.Tools;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;
using HttpContext = HamrahBina.Common.Tools.HttpContext;
using Microsoft.AspNetCore.Routing;

namespace HamrahBina
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // EF
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            // Authentication
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {

                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddErrorDescriber<CustomIdentityErrorDescriber>()
            .AddDefaultTokenProviders();

            services.AddAuthentication()
                .AddCookie(config =>
                {
                    config.LoginPath = new PathString("/Account/Login");
                    config.Cookie.Expiration = TimeSpan.FromMinutes(20);
                    config.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                    config.SlidingExpiration = false;
                }).AddJwtBearer(config =>
                {
                    config.RequireHttpsMetadata = false;
                    config.SaveToken = true;
                    config.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = Configuration["Tokens:Issuer"],
                        ValidAudience = Configuration["Tokens:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Tokens:Key"]))
                    };
                });

            // One time login
            services.AddOptions();
            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                // This is the key to control how often validation takes place ... must be zero
                options.ValidationInterval = TimeSpan.FromMinutes(0);
                options.OnRefreshingPrincipal = context =>
                {
                    context.NewPrincipal = context.CurrentPrincipal;
                    return Task.FromResult(0);//todo retain current user
                };
            });

            // DI
            services.AddTransient<DataSeeder>();
            services.TryAddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddHttpContextAccessor();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.TryAddSingleton<ContentResultExecutor>();
            services.AddScoped<IViewRenderService, ViewRenderService>();

            // MVC
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
            });
            services.AddRouting();
            services.AddMvc().AddSessionStateTempDataProvider().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            return services.BuildServiceProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env,
            IActionDescriptorCollectionProvider actionDescriptorCollectionProvider,
            DataSeeder seeder,
            IApplicationLifetime lifetime)
        {
            // Setting Persian culture
            var culture = new PersianCulture();
            var localizationOptions = new RequestLocalizationOptions
            {
                SupportedCultures = new List<CultureInfo>
                {
                    culture
                },
                SupportedUICultures = new List<CultureInfo>
                {
                    culture
                },
                DefaultRequestCulture = new RequestCulture(culture),
                FallBackToParentCultures = false,
                FallBackToParentUICultures = false,
                RequestCultureProviders = null
            };
            app.UseRequestLocalization(localizationOptions);
            
            // Custom exception handling
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // MVC
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseSession();

            // This is used to get httpContext in extension or static methods
            HttpContext.Configure(app.ApplicationServices.GetRequiredService<Microsoft.AspNetCore.Http.IHttpContextAccessor>());

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            seeder.SeedAsync().Wait();

            // Routing
            var routeBuilder = new RouteBuilder(app);
            var middleWareRoutes = routeBuilder.Build();
            app.UseRouter(middleWareRoutes);
        }
    }
}
