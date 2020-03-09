using System.IO;
using System.Linq;
using ClientIpAspNetCore.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using NLog.Targets;

namespace ClientIpAspNetCore
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
            services.AddScoped<ClientIdCheckFilter>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			// Add framework services.
			services.AddControllers();
        }
        
        public void Configure(IApplicationBuilder app)
        {
			GlobalDiagnosticsContext.Set("configDir", "C:\\git\\damienbod\\ClientIpAspNetCoreIIS\\Logs");
            GlobalDiagnosticsContext.Set("connectionString", Configuration.GetConnectionString("DefaultConnection"));

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseMiddleware<AdminSafeListMiddleware>(Configuration["AdminSafeList"]);

            app.UseRouting();

            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
			
        }
    }
}
