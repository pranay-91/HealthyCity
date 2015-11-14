using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Healthycity.DAL
{
    public class FitBitDataService: IFitBitDataService
    {
        MongoDataModel _model { get; set; }

        public FitBitDataService(MongoDataModel m)
        {
            _model = m;
        }

        public  FitBitUser GetFitBitUserByName(string name)
        {
            var collection = _model.GetFitBitUserCollection();

            // Get user by matching the user name. Implement $match by Where method
            //TODO: What if multiple user with same user name?
            var result = collection.AsQueryable()
                .Where(u => u.user_name == name).SingleOrDefault<FitBitUser>();
                
            
            //TODO: check if null
            return result;
        }

        public async Task<int> NewFitBitUser(FitBitUser new_user)
        {
            var collection = _model.GetFitBitUserCollection();
            if (GetFitBitUserByName(new_user.user_name)== null)
            {
                await collection.InsertOneAsync(new_user);
            }
            return 1;
        }
        
        public async Task<int> RemoveFitBitUserById(string id)
        {
            var collection = _model.GetFitBitUserCollection();

            var filter = new BsonDocument("_id", id);
            var result = await collection.DeleteOneAsync(filter);
            return 1;
        }

        public async Task<int> ModifyFitBitUser(FitBitUser user)
        {
            var collection = _model.GetFitBitUserCollection();

            var filter = new BsonDocument("user_name", user.user_name);
            var updateDocument = new BsonDocument { { "user_name", user.user_name }, { "access_token", user.access_token }, { "refresh_token", user.refresh_token }, { "expires_in", user.expires_in }, { "token_type", user.token_type } };
            var update = new BsonDocument("$set", updateDocument);
            var result = await collection.UpdateOneAsync(filter, update);

            //TODO: handle excpetion case
            return 1;
        }

        public IEnumerable<FitBitUser> GetAllFitBitUsers() {
            var collection = _model.GetFitBitUserCollection();
            var result = collection.AsQueryable();
            return result;
            //TODO: handle excpetion case
        }
    }
}