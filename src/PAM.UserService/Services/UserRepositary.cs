using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PAM.UserService.Model;
using PAM.UserService.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PAM.UserService.Services
{
    public class UserRepositary : IUserRepositary
    {
        private IMongoCollection<User> Users { get; }

        private ILogger<UserRepositary> Logger { get; }

        public UserRepositary(ILogger<UserRepositary> logger, IOptions<MongoOptions> mongoOptions)
        {
            Logger = logger;

            var client = new MongoClient(mongoOptions.Value.Server);
            Users = client.GetDatabase(mongoOptions.Value.Database)
                .GetCollection<User>(mongoOptions.Value.UserCollection);
        }

        public async Task<User> Create(User user)
        {
            await Users.InsertOneAsync(user);

            Logger.LogInformation("User created", user);

            return user;
        }

        public async Task<User> Update(User user)
        {
            if (!(await Users.FindAsync(x => x.Email == user.Email)).Any())
                throw new ApplicationException("Try to update user, which not exist.");

            await Users.FindOneAndReplaceAsync(x => x.Email == user.Email, user);

            Logger.LogInformation("User updated", user);

            return user;
        }

        public async Task<User> FindByEmail(string email)
        {
            return (await Users.FindAsync(x => x.Email == email)).SingleOrDefault();
        }

    }
}
