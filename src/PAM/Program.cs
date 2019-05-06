using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using PAM.Options;

namespace PAM
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureServices(ConfigureOptions)
                .UseStartup<Startup>();

        private static void ConfigureOptions(WebHostBuilderContext hostingContext, IServiceCollection services)
        {
            var configuration = hostingContext.Configuration;

            services.AddOptions()
                .Configure<ServicesOptions>(configuration.GetSection("Services"));
        }
    }
}
