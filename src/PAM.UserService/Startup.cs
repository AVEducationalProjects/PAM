using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using PAM.Infrastructure.Options;
using PAM.UserService.Authorization;
using PAM.UserService.Mappings;
using PAM.UserService.Model;
using PAM.UserService.Options;
using PAM.UserService.Services;
using Swashbuckle.AspNetCore.Swagger;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace PAM.UserService
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

        public JWTOptions JWTOptions { get; set; }

        public MongoOptions MongoOptions { get; set; }

        private void PrintVersionToLog()
        {
            Logger.LogInformation($"Personal Asset Manager User Service {Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion}");
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

            services.AddAuthorization(options => {
                options.AddPolicy("UserProfilePolicy", policy => policy.Requirements.Add(new ProfileRequirement()));
            });

            services.AddSingleton<IAuthorizationHandler, ProfileAuthorizationHandler>();

            services.AddAutoMapper(typeof(MappingProfile));

            SetupDatabase();
            services.AddScoped<IUserRepositary, UserRepositary>();
            services.AddScoped<IHouseholdRepositary, HouseholdRepositary>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "PAM User Service API", Version = "v1" });
            });
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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "PAM User Service API");
            });

            app.UseMvc();
        }

        private void SetupDatabase()
        {
            try
            {
                var client = new MongoClient(MongoOptions.Server);
                var db = client.GetDatabase(MongoOptions.Database);
                var userCollection = db.GetCollection<User>(MongoOptions.UserCollection);
                var indexes = userCollection.Indexes.List().ToList();
                if (!indexes.Any(x => x["name"] == "_idx_email"))
                {
                    var indexModel = new CreateIndexModel<User>(
                        Builders<User>.IndexKeys.Ascending(x => x.Email), 
                        new CreateIndexOptions { Name = "_idx_email", Unique = true });

                    userCollection.Indexes.CreateOne(indexModel);
                }
            }
            catch (MongoConnectionException)
            {
                Logger.LogError("Can't setup database. Server unavailable.");
                throw;
            }
            catch (MongoException)
            {
                Logger.LogError("Can't setup database. Index setup failed.");
                throw;
            }
        }

    }
}