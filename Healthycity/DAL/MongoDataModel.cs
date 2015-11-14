using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Healthycity.DAL
{
    public class MongoDataModel
    {
        IMongoClient _client { get; set; }
        IMongoDatabase _database { get; set; }
        
        string _serverURL { get; set; }
        string _serverDefaultDatabase { get; set; }

        MongoDataModel(string database, string url)
        {

        }
    }
}