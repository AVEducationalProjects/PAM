using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using PAM.Infrastructure.Options;
using PAM.UserService.Options;
using Serilog;

namespace PAM.UserService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
                    .ReadFrom.Configuration(hostingContext.Configuration)
                    .Enrich.FromLogContext()
                    .WriteTo.Console())
                .ConfigureServices(ConfigureOptions)
                .UseStartup<Startup>();
        }

        private static void ConfigureOptions(WebHostBuilderContext hostingContext, IServiceCollection services)
        {
            var configuration = hostingContext.Configuration;

            services.AddOptions()
                .Configure<MongoOptions>(configuration.GetSection("Mongo"))
                .Configure<JWTOptions>(configuration.GetSection("JWT"))
                .Configure<JWTSigninOptions>(configuration.GetSection("JWTSignIn"));
        }
    }
}
