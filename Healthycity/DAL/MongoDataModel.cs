using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Healthycity.DAL
{
    public class MongoDataModel
    {
        public IMongoClient _client { get; set; }
        public IMongoDatabase _database { get; set; }
        
        public string _serverURL { get; set; }
        public string _serverDefaultDatabase { get; set; }
    }
}