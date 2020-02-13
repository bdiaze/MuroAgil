using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MuroAgil.Models;

namespace MuroAgil
{
    public class Startup {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services) {
			services.AddMvc();
			services.AddDbContext<MuroAgilContext>();
			services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
				.AddCookie(option => {
					option.LoginPath = "/Usuario/IniciarSesion";
					option.AccessDeniedPath = "/Error/AccessDenied";
					option.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    option.SlidingExpiration = true;
                    option.ExpireTimeSpan = TimeSpan.FromDays(7);
                    option.Cookie.Expiration = TimeSpan.FromDays(7);
                });
		}

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
			app.UseExceptionHandler("/Error/Index");
			app.UseStaticFiles();
			app.UseAuthentication();
			app.UseMvcWithDefaultRoute();
		}
    }
}
