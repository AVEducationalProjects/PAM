using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using PAM.UserService.Model;
using PAM.UserService.Options;

namespace PAM.UserService.Services
{
    public class HouseholdRepositary : IHouseholdRepositary
    {
        private IMongoCollection<User> Users { get; }

        private IMongoCollection<Household> Households { get; }

        private ILogger<UserRepositary> Logger { get; }

        public HouseholdRepositary(ILogger<UserRepositary> logger, IOptions<MongoOptions> mongoOptions)
        {
            Logger = logger;

            var client = new MongoClient(mongoOptions.Value.Server);
            var database = client.GetDatabase(mongoOptions.Value.Database);

            Households = database.GetCollection<Household>(mongoOptions.Value.HouseholdCollection);
            Users = database.GetCollection<User>(mongoOptions.Value.UserCollection);
        }

        public async Task<Household> Create(User user, Household household)
        {
            if (user.Id == null)
                throw new ApplicationException("Can't create household for unstored user.");

            await Households.InsertOneAsync(household);

            await Users.FindOneAndUpdateAsync(x => x.Id == user.Id, 
                Builders<User>.Update.AddToSet(x=>x.Households, household.Id));

            Logger.LogInformation("New household created", household);

            return household;
        }

        public async Task<Household[]> FindHouseholdsById(IEnumerable<ObjectId> households)
        {
            var result = await Households.FindAsync(x => households.Contains(x.Id));
            return result.ToList().ToArray();
        }

        public async Task RemoveUserHousehold(User user, ObjectId householdId)
        {
            await Users.FindOneAndUpdateAsync(x => x.Id == user.Id,
                Builders<User>.Update.Pull(x => x.Households, householdId));

            Logger.LogInformation("Household removes from user", user, householdId);
        }

        public async Task AddHouseholdToUser(User user, ObjectId householdId)
        {
            if (!(await Households.FindAsync(x => x.Id == householdId)).Any())
                throw new ApplicationException("Household doesn't exist.");

            await Users.FindOneAndUpdateAsync(x => x.Id == user.Id,
                Builders<User>.Update.AddToSet(x => x.Households, householdId));

            Logger.LogInformation("Household adds to user", user, householdId);
        }
    }
}
