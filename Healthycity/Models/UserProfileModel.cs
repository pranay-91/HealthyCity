using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Healthycity.Models
{
    public class UserProfileModel : Fitbit.Models.UserProfile
    {
        public ObjectId id { get; set; }
    }
}