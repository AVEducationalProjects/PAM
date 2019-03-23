using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using PAM.UserService.Model;
using System.Linq;

namespace PAM.UserService.Services
{
    public class UserRepositary : IUserRepositary
    {
        private IMongoCollection<User> Users { get; set; }

        public UserRepositary(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("UserDb"));
            Users = client.GetDatabase("pam_users").GetCollection<User>("users");
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
