using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PAM.AssetService.Model
{
    public class Asset
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonRequired]
        public string Name { get; set; }

        [BsonRequired]
        public int Lifecycle { get; set; }

        [BsonRequired]
        public string HouseholdId { get; set; }
    }
}
