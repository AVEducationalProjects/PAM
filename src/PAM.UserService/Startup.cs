using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PAM.UserService.Mappings;
using PAM.UserService.Model;
using PAM.UserService.Services;

namespace PAM.UserService
{
    public class Startup
    {
        public Startup(ILogger<Startup> logger, IConfiguration configuration)
        {
            Configuration = configuration;
            Logger = logger;
        }

        public IConfiguration Configuration { get; }

        public ILogger<Startup> Logger { get;  }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Mapper.Initialize(cfg => cfg.AddProfile<MappingProfile>());
            services.AddAutoMapper();

            SetupDatabase();
            services.AddScoped<IUserRepositary, UserRepositary>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }

        private void SetupDatabase()
        {
            try
            {
                var client = new MongoClient(Configuration.GetConnectionString("UserDb"));
                var db = client.GetDatabase(Configuration.GetValue<string>("Database"));
                var userCollection = db.GetCollection<User>(Configuration.GetValue<string>("UserCollection"));
                var indexes = userCollection.Indexes.List().ToList();
                if (!indexes.Any(x=>x["name"] == "_idx_email"))
                {
                    var indexModel = new CreateIndexModel<User>(Builders<User>.IndexKeys.Ascending(x => x.Email), new CreateIndexOptions { Name = "_idx_email", Unique = true });
                    userCollection.Indexes.CreateOne(indexModel);
                }
            }
            catch(MongoConnectionException)
            {
                Logger.LogError("Can't setup database. Server unavailable.");
                throw;
            }
            catch(MongoException)
            {
                Logger.LogError("Can't setup database. Index setup failed.");
                throw;
            }
        }

    }
}