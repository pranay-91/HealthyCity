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

        public FitBitUser GetFitBitUserByName(string name)
        {
            var collection = _model._database.GetCollection<FitBitUser>("FitBitUser");
        }

        public async void NewFitBitUser(FitBitUser new_user)
        {
            var collection = _model._database.GetCollection<FitBitUser>("FitBitUser");

            if (GetFitBitUserByName(new_user.user_name)== null)
            {
                await collection.InsertOneAsync(new_user);
            }
        }

        public void RemoveFitBitUser(FitBitUser user)
        {
        }

    }
}