using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PAM.AssetService.Options;
using System.Security.Cryptography.X509Certificates;

namespace PAM.AssetService
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IOptions<JWTOptions> jwtOptions)
        {
            Configuration = configuration;
            JWTOptions = jwtOptions.Value;
        }

        public IConfiguration Configuration { get; }

        public JWTOptions JWTOptions { get; }

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
                            IssuerSigningKey = new X509SecurityKey(new X509Certificate2(JWTOptions.SigningCertificate, JWTOptions.SigningPassword)),
                            TokenDecryptionKey = new X509SecurityKey(new X509Certificate2(JWTOptions.EncryptionCertificate, JWTOptions.EncryptionPassword))
                        };

                    });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
