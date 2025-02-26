using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace idunno.CookieSharing.Core
{
    public class Startup
    {
        private readonly IHostingEnvironment env;
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            this.env = env;
        }        

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {            
            // We need to configure data protection to use a specific key directory
            // we can share between applications.
            //
            // We also need a common protector purpose, as different purposes are
            // automatically isolated from one another.
            //
            // Finally we need to wire up a common ticket formatter.

            // Normally you'd just have a hard coded, or configuration based path,
            // but for this demo we're going to share a directory in the solution directory,
            // so we have to do some jiggery-pokery to figure it out.
            string contentRoot = env.ContentRootPath;
            string keyRingPath = Path.GetFullPath(Path.Combine(contentRoot, "..", "idunno.KeyRing"));

            // Now we create a data protector, with a fixed purpose and sub-purpose used in key derivation.
            var protectionProvider = DataProtectionProvider.Create(new DirectoryInfo(keyRingPath));
            var dataProtector = protectionProvider.CreateProtector(
                "Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationMiddleware",
                "Cookie",
                "v2");
            // And finally create a new auth ticket formatter using the data protector.
            var ticketFormat = new TicketDataFormat(dataProtector);            

            services.AddAuthentication("CustomCookie").AddCookie("CustomCookie", o =>
            {                                           
                o.CookieName = ".AspNet.SharedCookie";
                o.TicketDataFormat = ticketFormat;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            

            // Now configure the cookie options to have the same cookie name, and use
            // the common format.         
            app.UseAuthentication();
            

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();         

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
