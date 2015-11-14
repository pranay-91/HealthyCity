using Healthycity.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace Healthycity.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public async Task<ActionResult> Index()
        {
            var _client = new MongoClient();
            var _database = _client.GetDatabase(ConfigurationManager.AppSettings["MongoDefaultDatabase"].ToString());
            var AccessTokenCollection = _database.GetCollection<AccessToken>("OAuth2AccessToken");
            var filter = new BsonDocument();
            using (var cursor = await AccessTokenCollection.FindAsync(filter))
            {
                while (await cursor.MoveNextAsync())
                {
                    var batch = cursor.Current;
                    foreach (var document in batch)
                    {
                        // process document
                     
                    }
                }
            }
            return View();
        }


        //    var chart = new CanvasJS.Chart("chartContainer",
        //{
        //  title: {
        //    text: "Monthly Downloads"
        //  },
        //    data: [
        //  {
        //    type: "area",
        //    dataPoints: [//array

        //    { x: new Date(2012, 00, 1), y: 2600 },
        //    { x: new Date(2012, 01, 1), y: 3800 },
        //    { x: new Date(2012, 02, 1), y: 4300 },
        //    { x: new Date(2012, 03, 1), y: 2900 },
        //    { x: new Date(2012, 04, 1), y: 4100 },
        //    { x: new Date(2012, 05, 1), y: 4500 },
        //    { x: new Date(2012, 06, 1), y: 8600 },
        //    { x: new Date(2012, 07, 1), y: 6400 },
        //    { x: new Date(2012, 08, 1), y: 5300 },
        //    { x: new Date(2012, 09, 1), y: 6000 }
        //    ]
        //  }
        //  ]
        //});

        public async Task<ActionResult> GetXML()
        {


            /* This part gets the request url and convert the xml file(.gpx) and parse into string directy using read as string
             string url = "/images/5/51/Gpx-full-sample.gpx";

            HttpClient client = new HttpClient() { BaseAddress = new Uri("http://shmuma.ru") };

            HttpResponseMessage response = await client.GetAsync(url);
             string responseBody = "";

             if (response.IsSuccessStatusCode) {
                 responseBody = await response.Content.ReadAsStringAsync();
             }
             else {
                 responseBody = "error";
             }

             ViewBag.responseMessage = responseBody;
             return View();
             
            */
            string url = "/images/5/51/Gpx-full-sample.gpx";

            HttpClient client = new HttpClient() { BaseAddress = new Uri("http://shmuma.ru") };

            HttpResponseMessage response = await client.GetAsync(url);

            Stream responseStream;
            if (response.IsSuccessStatusCode)
            {
                responseStream = await response.Content.ReadAsStreamAsync();
            }
            else
            {
                responseStream = null;
            }


            var xmlDoc = new XmlDocument();
            using (var reader = XmlReader.Create(responseStream)) {
                xmlDoc.Load(reader);
            }


            using (var stringWriter = new StringWriter())
            using (var xmlTextWriter = XmlWriter.Create(stringWriter))
            {
                xmlDoc.WriteTo(xmlTextWriter);
                xmlTextWriter.Flush();
                ViewBag.responseMessage = stringWriter.GetStringBuilder().ToString();
            }

            return View();
            
        }


        public FileResult GetXMLFile()
        {
            string url = "/images/5/51/Gpx-full-sample.gpx";

            HttpResponseMessage file = new HttpResponseMessage();
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("http://shmuma.ru");
                Task<HttpResponseMessage> response = httpClient.GetAsync(url);
                file = response.Result;
            }

            return File(file.Content.ReadAsByteArrayAsync().Result, "application/xml", "sample.gpx");
        }


        
    }
}