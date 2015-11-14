﻿using System;
using System.Configuration;
using System.Web.Mvc;
using Fitbit.Api.Portable;
using System.Threading.Tasks;
using Fitbit.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Healthycity.Models;
using System.Net.Http;
using Healthycity.DAL;

namespace Healthycity.Controllers
{
    public class FitbitController : Controller
    {
        private static IMongoClient _client;
        private static IMongoDatabase _database;
        //private string ConsumerKey;
        //private string ConsumerSecret;
        //private string ClientId;
        //private Authenticator2 authenticator;

        //FitbitController()
        //{
        //    ConsumerKey = ConfigurationManager.AppSettings["FitbitConsumerKey"];
        //    ConsumerSecret = ConfigurationManager.AppSettings["FitbitConsumerSecret"];
        //    ClientId = ConfigurationManager.AppSettings["FitbitClientId"];
        //    Authenticator2 authenticator = new Authenticator2(ClientId, ConsumerSecret, Request.Url.GetLeftPart(UriPartial.Authority) + "/Fitbit/Callback");
        //}
        // GET: Fitbit
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Authorize()
        {
            string ConsumerKey = ConfigurationManager.AppSettings["FitbitConsumerKey"];
            string ConsumerSecret = ConfigurationManager.AppSettings["FitbitConsumerSecret"];
            string ClientId = ConfigurationManager.AppSettings["FitbitClientId"];

            Authenticator2 authenticator = new Authenticator2(ClientId, ConsumerSecret, Request.Url.GetLeftPart(UriPartial.Authority) + "/Fitbit/Callback");



            string[] scopes = new string[] { "profile", "activity", "heartrate", "location" };

            string authUrl = authenticator.GenerateAuthUrl(scopes, null);

            return Redirect(authUrl);        
        }

        public async Task<ActionResult> Callback()
        {
            //_client = new MongoClient();
            //_database = _client.GetDatabase(ConfigurationManager.AppSettings["MongoDefaultDatabase"].ToString());

            //var collection = _database.GetCollection<AccessToken>("OAuth2AccessToken");

            //string ConsumerKey = ConfigurationManager.AppSettings["FitbitConsumerKey"];
            //string ConsumerSecret = ConfigurationManager.AppSettings["FitbitConsumerSecret"];
            //string ClientId = ConfigurationManager.AppSettings["FitbitClientId"];

            //Authenticator2 authenticator = new Authenticator2(ClientId, ConsumerSecret, Request.Url.GetLeftPart(UriPartial.Authority) + "/Fitbit/Callback");

            //string code = Request.Params["code"];
            //OAuth2AccessToken accessToken = await authenticator.ExchangeAuthCodeForAccessTokenAsync(code);

            //AccessToken tokenData = new AccessToken();
            //tokenData.UserName = "test";
            //tokenData.Token = accessToken.Token;
            //tokenData.TokenType = accessToken.TokenType;
            //tokenData.ExpiresIn = accessToken.ExpiresIn;
            //tokenData.RefreshToken = accessToken.RefreshToken;

            //await collection.InsertOneAsync(tokenData);

            //Session["AccessToken"] = accessToken;
            //System.Diagnostics.Debug.WriteLine("Access Token is: {0} and Expires in: {1} ", accessToken.Token, accessToken.ExpiresIn);
            //return RedirectToAction("Index", "Home");   

            
            // FitBit data model to connect Mongo database
            // Database settings stored in Webconfig--> appSettings
            MongoDataModel dm = new MongoDataModel(ConfigurationManager.AppSettings["MongoDefaultDatabase"].ToString());
            FitBitDataService FitData = new FitBitDataService(dm);
                     

            string ConsumerKey = ConfigurationManager.AppSettings["FitbitConsumerKey"];
            string ConsumerSecret = ConfigurationManager.AppSettings["FitbitConsumerSecret"];
            string ClientId = ConfigurationManager.AppSettings["FitbitClientId"];

            Authenticator2 authenticator = new Authenticator2(ClientId, ConsumerSecret, Request.Url.GetLeftPart(UriPartial.Authority) + "/Fitbit/Callback");

            //get authorisation code and exchange for access token
            string code = Request.Params["code"];
            OAuth2AccessToken accessToken = await authenticator.ExchangeAuthCodeForAccessTokenAsync(code);

            //get the user name by using the access token being currently recieved
            FitbitClient client = GetFitbitClient(accessToken.Token, accessToken.RefreshToken);
            FitbitResponse<UserProfile> response = await client.GetUserProfileAsync();


            FitBitUser new_user = await getNewUser(accessToken);
            //add the new user to the database
            await FitData.NewFitBitUser(new_user);

            Session["AccessToken"] = accessToken;
            System.Diagnostics.Debug.WriteLine("Access Token is: {0} and Expires in: {1} ", accessToken.Token, accessToken.ExpiresIn);
         
            return RedirectToAction("Index", "Home");
        }

        private async Task<FitBitUser> RefreshToken(string refresh_token, string user_name)
        {

            MongoDataModel dm = new MongoDataModel(ConfigurationManager.AppSettings["MongoDefaultDatabase"].ToString());
            FitBitDataService FitData = new FitBitDataService(dm);

            string ConsumerKey = ConfigurationManager.AppSettings["FitbitConsumerKey"];
            string ConsumerSecret = ConfigurationManager.AppSettings["FitbitConsumerSecret"];
            string ClientId = ConfigurationManager.AppSettings["FitbitClientId"];
            Authenticator2 authenticator = new Authenticator2(ClientId, ConsumerSecret, Request.Url.GetLeftPart(UriPartial.Authority) + "/Fitbit/Callback");

            OAuth2AccessToken access_token = await authenticator.RefreshAccessTokenAsync(refresh_token);
            //TODO: update the database with the new token

            FitBitUser user = await getNewUser(access_token);
            
            //update the user token
            await FitData.ModifyFitBitUser(user);
            return user;
        }

        private async Task<FitBitUser> getNewUser(OAuth2AccessToken token) {

            //get the user name by using the access token being currently recieved
            FitbitClient client = GetFitbitClient(token.Token, token.RefreshToken);
            FitbitResponse<UserProfile> response = await client.GetUserProfileAsync();

            // Create a new user with the access token recieved and user name 
            FitBitUser user = new FitBitUser();
            user.user_name = response.Data.FullName;
            user.access_token = token.Token;
            user.token_type = token.TokenType;
            user.expires_in = token.ExpiresIn;
            user.refresh_token = token.RefreshToken;

            return user;
        }


        // Test method for updating a user information
        private async Task<int> testModifyFitbitUser() {
            MongoDataModel dm = new MongoDataModel(ConfigurationManager.AppSettings["MongoDefaultDatabase"].ToString());
            FitBitDataService FitData = new FitBitDataService(dm);
            FitBitUser user = FitData.GetFitBitUserByName("Pronoy Pradhananga");
            user.expires_in = 2000;

            await FitData.ModifyFitBitUser(user);
            return 1;
        }

        public async Task<ActionResult> GetUserProfile()
        {
            _client = new MongoClient();
            _database = _client.GetDatabase(ConfigurationManager.AppSettings["MongoDefaultDatabase"].ToString());

            var OAuth2AccessTokenCollection = _database.GetCollection<AccessToken>("OAuth2AccessToken");
            var collection = _database.GetCollection<UserProfile>("UserProfile");

            var AccessTokenDocument = await OAuth2AccessTokenCollection.Find(new BsonDocument()).FirstOrDefaultAsync();

           // OAuth2AccessToken accessToken = (OAuth2AccessToken)AccessTokenDocument;

            //FitbitClient client = GetFitbitClient(Acc.Token, accessToken.RefreshToken);

            FitbitClient client = GetFitbitClient(AccessTokenDocument.Token, AccessTokenDocument.RefreshToken);
            FitbitResponse<UserProfile> response = await client.GetUserProfileAsync();

            //var error = response.Errors.Find(x => x.ErrorType== ErrorType.OAuth);

            //if (error != null)
            //{
            //    var new_access_token = RefreshToken(AccessTokenDocument.RefreshToken, "GetUserProfile");
            //    var result = await collection.ReplaceOneAsync(item => item.Id == AccessTokenDocument.id, new_access_token);
            //}

            await collection.InsertOneAsync(response.Data);

            return View(response.Data);
        }

        private FitbitClient GetFitbitClient(string bearerToken, string refreshToken)
        {
            OAuth2Authorization authorization = new OAuth2Authorization(bearerToken, refreshToken);
            FitbitClient client = new FitbitClient(authorization);
            return client;
        }

        public async Task<ActionResult> LastWeekSteps()
        {
            OAuth2AccessToken accessToken = (OAuth2AccessToken)Session["AccessToken"];

            FitbitClient client = GetFitbitClient(accessToken.Token, accessToken.RefreshToken);

            FitbitResponse<TimeSeriesDataListInt> response = await client.GetTimeSeriesIntAsync(TimeSeriesResourceType.Steps, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow);

            return View(response.Data);

        }

        public async Task<ActionResult> getActivityList()
        {
            DateTime activity_date = new DateTime(2015, 10, 14);
            _client = new MongoClient();
            _database = _client.GetDatabase(ConfigurationManager.AppSettings["MongoDefaultDatabase"].ToString());
            var OAuth2AccessTokenCollection = _database.GetCollection<AccessToken>("OAuth2AccessToken");

            var AccessTokenDocument = await OAuth2AccessTokenCollection.Find(new BsonDocument()).FirstOrDefaultAsync();

            FitbitClient client = GetFitbitClient(AccessTokenDocument.Token, AccessTokenDocument.RefreshToken);
            FitbitResponse<ActivityLogList> response = await client.GetActivityListAsync(activity_date);

            return View(response.Data.DataSet);
        }
        


        public async Task<FileResult> GetTCX() {


            _client = new MongoClient();
            _database = _client.GetDatabase(ConfigurationManager.AppSettings["MongoDefaultDatabase"].ToString());

            var OAuth2AccessTokenCollection = _database.GetCollection<AccessToken>("OAuth2AccessToken");

            var AccessTokenDocument = await OAuth2AccessTokenCollection.Find(new BsonDocument()).FirstOrDefaultAsync();

            FitbitClient client = GetFitbitClient(AccessTokenDocument.Token, AccessTokenDocument.RefreshToken);

            HttpResponseMessage response = await client.GetActivityTCX("366076610");
            return File(response.Content.ReadAsByteArrayAsync().Result, "application/vnd.garmin.tcx+xml", "sample.tcx");
        }

        public async Task<ActionResult> GetHeartRateString() {
            _client = new MongoClient();
            _database = _client.GetDatabase(ConfigurationManager.AppSettings["MongoDefaultDatabase"].ToString());

            var OAuth2AccessTokenCollection = _database.GetCollection<AccessToken>("OAuth2AccessToken");

            var AccessTokenDocument = await OAuth2AccessTokenCollection.Find(new BsonDocument()).FirstOrDefaultAsync();

            FitbitClient client = GetFitbitClient(AccessTokenDocument.Token, AccessTokenDocument.RefreshToken);

            string responseString = await client.GetHeartRateSeriesString(DateTime.Now, "7d");

            return Content(responseString, "application/json");
        }


        public async Task<ActionResult> refreshDemo()
        {
            MongoDataModel dm = new MongoDataModel(ConfigurationManager.AppSettings["MongoDefaultDatabase"].ToString());
            FitBitDataService FitData = new FitBitDataService(dm);
            FitBitUser user = FitData.GetFitBitUserByName("Pronoy Pradhananga");

            var accessToken =  await RefreshToken(user.refresh_token, user.user_name);
            return View(accessToken);
        }
    }
}


/*
pass json to view for graph
 [httpPost]
    public JsonResult something(string userGuid)
    {
        var p = GetUserProducts(userGuid);
        return Json(p, JsonRequestBehavior.AllowGet);
    }
    in view
    $.post( "../something", {userGuid: "foo"}, function( data ) {
  console.log(data)
});
*/
