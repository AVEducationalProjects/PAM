using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using PAM.UserService.Model;
using System.Linq;

namespace PAM.UserService.Services
{
    public class UserRepositary : IUserRepositary
    {
        private IMongoCollection<User> Users { get; }

        private ILogger<UserRepositary> Logger { get; }

        public UserRepositary(ILogger<UserRepositary> logger, IConfiguration configuration)
        {
            Logger = logger;

            var client = new MongoClient(configuration.GetConnectionString("UserDb"));
            Users = client.GetDatabase(configuration.GetValue<string>("Database"))
                .GetCollection<User>(configuration.GetValue<string>("UserCollection"));
        }

        public void Create(User user)
        {
            Users.InsertOne(user);
        }

        public void CreateOrUpdate(User user)
        {
            if (Users.Find(x => x.Email == user.Email).Any())
            {
                Users.FindOneAndReplace(x => x.Email == user.Email, user);
                return;
            }
            Create(user);
        }

        public void DeleteByEmail(string email)
        {
            Users.FindOneAndDelete(x => x.Email == email);
        }

        public User FindByEmail(string email)
        {
            return Users.Find(x => x.Email == email).SingleOrDefault();
        }

    }
}
