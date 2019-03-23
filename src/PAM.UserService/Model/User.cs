using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PAM.UserService.Model
{
    public class User
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonRequired]
        public string Email { get; set; }
    }
}