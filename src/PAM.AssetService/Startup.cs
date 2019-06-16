using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PAM.AssetService.Mappings;
using PAM.AssetService.Options;
using PAM.AssetService.Services;
using PAM.Infrastructure.Options;
using System;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace PAM.AssetService
{
    public class Startup
    {
        public Startup(ILogger<Startup> logger, IConfiguration configuration,
            IOptions<MongoOptions> mongoOptions, IOptions<JWTOptions> jwtOptions)
        {
            Configuration = configuration;
            Logger = logger;

            JWTOptions = jwtOptions.Value;
            MongoOptions = mongoOptions.Value;

            PrintVersionToLog();
        }

        public IConfiguration Configuration { get; }

        public ILogger<Startup> Logger { get; }

        public JWTOptions JWTOptions { get; }

        public MongoOptions MongoOptions { get; set; }

        private void PrintVersionToLog()
        {
            Logger.LogInformation($"Personal Asset Manager Asset Service {Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion}");
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.RequireHttpsMetadata = false;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidIssuer = "PAM",
                            ValidateAudience = true,
                            ValidAudience = "*.PAM",
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new X509SecurityKey(new X509Certificate2(JWTOptions.SigningCertificate)),
                            TokenDecryptionKey = new X509SecurityKey(new X509Certificate2(JWTOptions.EncryptionCertificate, JWTOptions.EncryptionPassword))
                        };

                    });

            services.AddAutoMapper(typeof(MappingProfile));

            SetupDatabase();
            services.AddScoped<IAssetRepositary, AssetRepositary>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "PAM Asset Service API");
            });

            app.UseMvc();
        }

        private void SetupDatabase()
        {
        }

    }
}
