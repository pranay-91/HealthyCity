using System;
using System.Configuration;
using System.Web.Mvc;
using Fitbit.Api.Portable;
using System.Threading.Tasks;
using Fitbit.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Healthycity.Models;

namespace Healthycity.Controllers
{
    public class FitbitController : Controller
    {
        private static IMongoClient _client;
        private static IMongoDatabase _database;

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

            
            string[] scopes = new string[] { "profile" };
            string authUrl = authenticator.GenerateAuthUrl(scopes, null);

            return Redirect(authUrl);        
        }

        public async Task<ActionResult> Callback()
        {
            _client = new MongoClient();
            _database = _client.GetDatabase(ConfigurationManager.AppSettings["MongoDefaultDatabase"].ToString());

            var collection = _database.GetCollection<OAuth2AccessToken>("OAuth2AccessToken");

            string ConsumerKey = ConfigurationManager.AppSettings["FitbitConsumerKey"];
            string ConsumerSecret = ConfigurationManager.AppSettings["FitbitConsumerSecret"];
            string ClientId = ConfigurationManager.AppSettings["FitbitClientId"];

            Authenticator2 authenticator = new Authenticator2(ClientId, ConsumerSecret, Request.Url.GetLeftPart(UriPartial.Authority) + "/Fitbit/Callback");

            string code = Request.Params["code"];
            OAuth2AccessToken accessToken = await authenticator.ExchangeAuthCodeForAccessTokenAsync(code);

            await collection.InsertOneAsync(accessToken);

            Session["AccessToken"] = accessToken;
            System.Diagnostics.Debug.WriteLine("Access Token is: {0} and Expires in: {1} ", accessToken.Token, accessToken.ExpiresIn);
            return RedirectToAction("Index", "Home");
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



    }
}