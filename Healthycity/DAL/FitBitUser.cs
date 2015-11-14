using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Healthycity.DAL
{
    public class FitBitUser
    {
        public string user_id { get; set; }
        public string user_name { get; set; }
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public string expires_in { get; set; }
        
    }
}