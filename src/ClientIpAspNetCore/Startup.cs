using System.IO;
using System.Linq;
using ClientIpAspNetCore.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ClientIdCheckFilter>();

            services.AddMvc();
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddNLog();

            app.UseStaticFiles();

            //var configDir = "C:\\git\\ClientIpAspNetCore\\Logs";
            var configDir = "C:\\inetpub\\wwwroot\\clientidaspnetcore\\Logs";

            if (configDir != string.Empty)
            {
                var logEventInfo = NLog.LogEventInfo.CreateNullEvent();
                foreach (FileTarget target in LogManager.Configuration.AllTargets.Where(t => t is FileTarget))
                {
                    var filename = target.FileName.Render(logEventInfo).Replace("'", "");
                    target.FileName = Path.Combine(configDir, filename);
                }

                LogManager.ReconfigExistingLoggers();
            }
            app.UseMvc();
        }
    }
}
