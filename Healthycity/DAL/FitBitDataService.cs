using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Healthycity.DAL
{
    public class FitBitDataService: IFitBitDataService
    {
        MongoDataModel _model { get; set; }

        FitBitDataService(MongoDataModel m)
        {
            _model = m;
        }

        public  FitBitUser GetFitBitUserByName(string name)
        {
            var collection = _model._database.GetCollection<FitBitUser>("FitBitUser");

            // Get user by matching the user name. Implement $match by Where method
            //TODO: What if multiple user with same user name?
            var result =  collection.AsQueryable()
                .Where(u => u.user_name == name);
            
            //TODO: check if null
            return (FitBitUser)result;
        }

        public async void NewFitBitUser(FitBitUser new_user)
        {
            var collection = _model._database.GetCollection<FitBitUser>("FitBitUser");
            if (GetFitBitUserByName(new_user.user_name)== null)
            {
                await collection.InsertOneAsync(new_user);
            }
        }
        
        public async void RemoveFitBitUserById(string id)
        {
            var collection = _model._database.GetCollection<FitBitUser>("FitBitUser");

            var filter = new BsonDocument("_id", id);
            var result = await collection.DeleteOneAsync(filter);
        }

        public async void ModifyFitBitUser(FitBitUser user)
        {
            var collection = _model._database.GetCollection<FitBitUser>("FitBitUser");

            var filter = new BsonDocument("_id", user._id);
            var result = await collection.FindOneAndReplaceAsync(filter, user);

            //TODO: handle excpetion case
        }

        public IEnumerable<FitBitUser> GetAllFitBitUsers() {
            var collection = _model._database.GetCollection<FitBitUser>("FitBitUser");
            var result = collection.AsQueryable();
            return result;
            //TODO: handle excpetion case
        }
    }
}