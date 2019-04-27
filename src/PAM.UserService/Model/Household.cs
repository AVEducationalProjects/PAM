using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PAM.UserService.Model
{
    public class Household
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonRequired]
        public string Name { get; set; }
    }
}
