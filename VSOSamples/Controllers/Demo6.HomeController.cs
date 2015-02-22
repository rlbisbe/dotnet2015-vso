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

        public async Task<ActionResult> Builds()
        {
            Auth authorizationInfo;

            var redisManager = new BasicRedisClientManager("localhost");
            using (var redis = redisManager.GetClient())
            {
                var redisCredentials = redis.As<Auth>();
                var credentials = redisCredentials.GetAll();
                if (credentials.Count == 0)
                {
                    RedirectToAction("LogIn");
                }
                authorizationInfo = credentials[0];
            }

            if (authorizationInfo.generationTime.AddSeconds(authorizationInfo.expires_in) < DateTime.UtcNow)
            {
                authorizationInfo = RefreshToken(authorizationInfo.refresh_token).Result;
            }

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
                    authorizationInfo.access_token);

                HttpResponseMessage response =
                    await client.GetAsync("https://rlbisbe.VisualStudio.com/DefaultCollection/DemoDotNetConf/_apis/build/builds?api-version=1.0");

                ViewBag.ResponseMessage = response.ReasonPhrase.ToString();
                ViewBag.Response = await response.Content.ReadAsStringAsync();
                return View("TeamProjects");
            }
        }


        public async Task<ActionResult> TriggerBuild()
        {
            Auth authorizationInfo;

            var redisManager = new BasicRedisClientManager("localhost");
            using (var redis = redisManager.GetClient())
            {
                var redisCredentials = redis.As<Auth>();
                var credentials = redisCredentials.GetAll();
                if (credentials.Count == 0)
                {
                    RedirectToAction("LogIn");
                }
                authorizationInfo = credentials[0];
            }

            if (authorizationInfo.generationTime.AddSeconds(authorizationInfo.expires_in) < DateTime.UtcNow)
            {
                authorizationInfo = RefreshToken(authorizationInfo.refresh_token).Result;
            }

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
                    authorizationInfo.access_token);

                var dataToSend = new Dictionary<string, object>() { 
                        {"definition", new Dictionary<string, object> () {{"id", 4}}},
                        {"reason", "Manual"},
                        {"priority", "Normal"}
                    };

                var content = new StringContent(JsonConvert.SerializeObject(dataToSend), Encoding.UTF8, "application/json");


                HttpResponseMessage response =
                    await client.PostAsync("https://rlbisbe.VisualStudio.com/DefaultCollection/DemoDotNetConf/_apis/build/requests?api-version=1.0", content);

                ViewBag.ResponseMessage = response.ReasonPhrase.ToString();
                ViewBag.Response = await response.Content.ReadAsStringAsync();
                return View("TeamProjects");
            }
        }
    }

#endif
}