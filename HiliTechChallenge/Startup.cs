using System;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using HiliTechChallenge.Core;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HiliTechChallenge
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();

            services.AddRazorPages();

            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AdsDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });

            var authUrl = "http://hili.guftall.ir";

            services.AddHttpClient("auth", config =>
            {
                config.BaseAddress = new Uri(authUrl);
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("admin", policy =>
                {
                    policy.Requirements.Add(new ClaimsAuthorizationRequirement(ClaimTypes.Role, new[] {"admin"}));
                });
                options.AddPolicy("advertiser", policy =>
                {
                    policy.Requirements.Add(new ClaimsAuthorizationRequirement(ClaimTypes.Role, new[] {"advertiser"}));
                });
            });
            
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromDays(1);
                    options.Cookie.Domain = "hili.guftall.ir";
                })
                .AddIdentityServerAuthentication("Bearer", options =>
                {
                    options.Authority = authUrl;
                    options.LegacyAudienceValidation = false;
                    options.ApiName = "user.manage";
                    options.RequireHttpsMetadata = false;
                });

            services.AddScoped<AuthHelper>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}