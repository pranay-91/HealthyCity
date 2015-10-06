using System;
using System.Configuration;
using System.Web.Mvc;
using Fitbit.Api.Portable;
using System.Threading.Tasks;
using Fitbit.Models;

namespace Healthycity.Controllers
{
    public class FitbitController : Controller
    {
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
            string ConsumerKey = ConfigurationManager.AppSettings["FitbitConsumerKey"];
            string ConsumerSecret = ConfigurationManager.AppSettings["FitbitConsumerSecret"];
            string ClientId = ConfigurationManager.AppSettings["FitbitClientId"];

            Authenticator2 authenticator = new Authenticator2(ClientId, ConsumerSecret, Request.Url.GetLeftPart(UriPartial.Authority) + "/Fitbit/Callback");

            string code = Request.Params["code"];
            OAuth2AccessToken accessToken = await authenticator.ExchangeAuthCodeForAccessTokenAsync(code);

            Session["AccessToken"] = accessToken;
            System.Diagnostics.Debug.WriteLine("Access Token is: {0} and Expires in: {1} ", accessToken.Token, accessToken.ExpiresIn);
            return RedirectToAction("Index", "Home");
        }

        public async Task<ActionResult> GetUserProfile()
        {
            OAuth2AccessToken accessToken = (OAuth2AccessToken)Session["AccessToken"];

            FitbitClient client = GetFitbitClient(accessToken.Token, accessToken.RefreshToken);
            FitbitResponse<UserProfile> response = await client.GetUserProfileAsync();

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