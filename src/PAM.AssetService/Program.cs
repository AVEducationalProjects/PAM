using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using PAM.AssetService.Options;
using PAM.Infrastructure.Options;
using Serilog;

namespace PAM.AssetService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                 .UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
                    .ReadFrom.Configuration(hostingContext.Configuration)
                    .Enrich.FromLogContext()
                    .WriteTo.Console())
                .ConfigureServices(ConfigureOptions)
                .UseStartup<Startup>();

        private static void ConfigureOptions(WebHostBuilderContext hostingContext, IServiceCollection services)
        {
            var configuration = hostingContext.Configuration;

            services.AddOptions()
                .Configure<MongoOptions>(configuration.GetSection("Mongo"))
                .Configure<JWTOptions>(configuration.GetSection("JWT"));
        }
    }
}
