using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Healthycity.Context
{
    public class FitbitContext
    {

        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public string ClientId { get; set; }

        public FitbitContext() {
            ConsumerKey = ConfigurationManager.AppSettings["FitbitConsumerKey"];
            ConsumerSecret = ConfigurationManager.AppSettings["FitbitConsumerSecret"];
            ClientId = ConfigurationManager.AppSettings["FitbitClientId"];
        }

    }
}