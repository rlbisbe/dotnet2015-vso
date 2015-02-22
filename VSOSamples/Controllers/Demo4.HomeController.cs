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
namespace VSOSamples.Controllers
{
#if DEMO3
    public partial class HomeController : Controller
    {
        public async Task<ActionResult> Repositories()
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
                    await client.GetAsync("https://rlbisbe.VisualStudio.com/DefaultCollection/_apis/git/repositories?api-version=1.0");

                ViewBag.ResponseMessage = response.ReasonPhrase.ToString();
                ViewBag.Response = await response.Content.ReadAsStringAsync();
                return View("TeamProjects");
            }
        }

        public async Task<ActionResult> Commits()
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
                    await client.GetAsync("https://rlbisbe.VisualStudio.com/DefaultCollection/_apis/git/repositories/663ccf42-95cc-467b-9b2b-841cff8f2388/commits?api-version=1.0");

                ViewBag.ResponseMessage = response.ReasonPhrase.ToString();
                ViewBag.Response = await response.Content.ReadAsStringAsync();
                return View("TeamProjects");
            }
        }
    }
#endif
}