using System;
using System.Collections.Generic;
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
        public ActionResult Index()
        {
            return View();
        }

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