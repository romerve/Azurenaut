using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace WebPortal
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
            /* 
                Configures Microsoft Identity Web: MIW is a set of ASP.NET Core libraries
                to facilitate AuthN and AuthZ for apps and APIs. If you are only interested
                on obtaining tokens, then use MSAL.NET
             */
            services.AddMicrosoftIdentityWebAppAuthentication(Configuration, "Azure:AzureAdB2C")
                .EnableTokenAcquisitionToCallDownstreamApi(new string[] { Configuration["ServiceApis:weatherApi:weatherApiScope"] })
                .AddInMemoryTokenCaches();

            services.AddControllersWithViews();

            services.AddRazorPages().AddMicrosoftIdentityUI();

            services.AddOptions();
            services.Configure<OpenIdConnectOptions>(Configuration.GetSection("Azure:AzureAdB2C"));
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }
    }
}
