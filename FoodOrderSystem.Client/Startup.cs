using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoodOrderSystem.Client.Data;
using FoodOrderSystem.Client.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;

namespace FoodOrderSystem.Client
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

      
        public void ConfigureServices(IServiceCollection services)
        {
            //Including DbContext as service 
            services.AddDbContext<FoodOrderIdentityContext>(options =>
            options
            .UseSqlServer(Configuration.GetConnectionString("FoodOrderIdentityContext")));

            // Including Identity as service
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<FoodOrderIdentityContext>()
                .AddDefaultTokenProviders();

            //Configure options related to the identity system
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.SignIn.RequireConfirmedEmail = false;        
            }
           );

           // Configue Cookie options - Cookie expiration and Unauthorized() when not logged in
            services.ConfigureApplicationCookie(options =>
                {                  
                    options.LoginPath = "";
                    options.ExpireTimeSpan = TimeSpan.FromHours(24);
                    options.Cookie.IsEssential = true;
                    options.Events.OnRedirectToLogin = context =>
                    {
                        context.Response.StatusCode = 401;                      
                        return Task.CompletedTask;
                    };
                }
            );

            //Configure Cookie Policy
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // Adding Cross origin resource sharing
            services.AddCors();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }
     
        // Middleware - request pipeline
        public void Configure(
            IApplicationBuilder app, 
            IHostingEnvironment env, 
            IServiceProvider serviceProvider,
            FoodOrderIdentityContext context,      
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
           )
        {
            var userStoreManage = serviceProvider.GetService<UserManager<FoodOrderIdentityContext>>();
           

            if (env.IsDevelopment())
            {
               
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
           
            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            app.UseMvc();

           // Call the Initilize method to seed the database with data
          Initializer.Initialize(context, userManager, roleManager).Wait();

        }
    }
}
