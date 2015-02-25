using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using ServiceStack.Redis;
using System.Text;
namespace VSOSamples.Controllers
{
#if DEMO3
    public partial class HomeController : Controller
    {
        public ActionResult Bug()
        {
            var workItem = new Bug();
            return View(workItem);
        }

        [HttpPost]
        public async Task<ActionResult> Bug(Bug itemToSave)
        {
            var authorizationInfo = LoadAuthInfo();
           
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
                    authorizationInfo.access_token);

                var dataToSend = new List<Dictionary<string, string>>(){
                    new Dictionary<string, string>() { 
                        {"op", "add"},
                        {"path", "/fields/System.Title"},
                        {"value", itemToSave.Title}
                    }
                };

                var content = new StringContent(JsonConvert.SerializeObject(dataToSend), Encoding.UTF8, HttpClientExtensions.MimeJson);

                HttpResponseMessage response =
                    await client.PatchAsync("https://rlbisbe.VisualStudio.com/DefaultCollection/DemoDotNetConf/_apis/wit/workitems/$Bug?api-version=1.0", content);

                ViewBag.ResponseMessage = response.ReasonPhrase.ToString();
                ViewBag.Response = await response.Content.ReadAsStringAsync();
                return View("TeamProjects");
            }
        }
    }

    public class Bug
    {
        public string Title { get; set; }
    }


    public static class HttpClientExtensions
    {
        public const string MimeJson = "application/json-patch+json";

        public static Task<HttpResponseMessage> PatchAsync(this HttpClient client, string requestUri, HttpContent content)
        {
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = new HttpMethod("PATCH"),
                RequestUri = new Uri(client.BaseAddress + requestUri),
                Content = content,
            };

            return client.SendAsync(request);
        }
    }
#endif
}