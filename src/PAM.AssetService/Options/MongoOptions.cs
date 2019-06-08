using PAM.Infrastructure.Options;

namespace PAM.AssetService.Options
{
    public class MongoOptions : MongoOptionsBase
    {
        public string AssetCollection { get; set; }
    }
}
