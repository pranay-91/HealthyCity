using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Healthycity.DAL
{
    public class FitBitUser
    {
        public ObjectId _id { get; set; }
        public string user_name { get; set; }
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public int expires_in { get; set; }
        public string token_type { get; set; } // "Bearer" is expected
    }
}