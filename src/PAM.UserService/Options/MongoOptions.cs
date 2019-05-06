using PAM.Infrastructure.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PAM.UserService.Options
{
    public class MongoOptions : MongoOptionsBase
    {       
        public string UserCollection { get; set; }

        public string HouseholdCollection { get; set; }
    }
}
