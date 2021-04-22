using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InmobiliariaLucero
{
    public class Startup
    {
        private readonly IConfiguration configuration;
        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.

            public void ConfigureServices(IServiceCollection services)
            {
                services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(options =>//el sitio web valida con cookie
                {
                        options.LoginPath = "/Usuario/Login";
                        options.LogoutPath = "/Usuario/Logout";
                        options.AccessDeniedPath = "/Home/Restringido";

                    });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Administrador", policy => policy.RequireClaim(ClaimTypes.Role, "Administrador", "SuperAdministrador"));
            });

             services.AddControllersWithViews();

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
                app.UseHttpsRedirection();
                app.UseStaticFiles();
                app.UseRouting();
                app.UseCookiePolicy(new CookiePolicyOptions
                {
                    MinimumSameSitePolicy = SameSiteMode.None,
                });

                app.UseAuthentication();
                app.UseAuthorization();

                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();//página amarilla de errores
                }


                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllerRoute("login", "login/{**accion}", new { controller = "Usuario", action = "Login" });

                    endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                });
            }


        }
    }

