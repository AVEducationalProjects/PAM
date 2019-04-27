using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace PAM.UserService.Model
{
    public class User
    {
        public User()
        {
            Households = new List<ObjectId>();
        }

        [BsonId]
        public ObjectId Id { get; set; }

        [BsonRequired]
        public string Email { get; set; }

        [BsonRequired]
        public string Name { get; set; }
        
        public IList<ObjectId> Households { get; set; }
    }
}