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
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult LogIn()
        {
            var appId = "C792BE55-9DE9-4968-944C-7A2697FC1FE2";
            var scope = "vso.build_execute vso.chat_manage vso.code_manage vso.test_write vso.work_write";
            var redirectUri = "https://dotnetconference.local:44300/home/Callback";

            return Redirect(string.Format("https://app.vssps.visualstudio.com/oauth2/authorize" +
                "?mkt=es&client_id={0}&response_type=Assertion&scope={1}&redirect_uri={2}", appId, scope, redirectUri));
        }

        public async Task<ActionResult> Callback()
        {

            using (var client = new HttpClient())
            {

                var code = Request.Params["code"];
                var redirectUri = "https://dotnetconference.local:44300/home/Callback";
                var url = "https://app.vssps.visualstudio.com/oauth2/token";

                var dataDictionary = new Dictionary<string, string>() { 
                    {"client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer"},
                    {"client_assertion", "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Im9PdmN6NU1fN3AtSGpJS2xGWHo5M3VfVjBabyJ9.eyJjaWQiOiJjNzkyYmU1NS05ZGU5LTQ5NjgtOTQ0Yy03YTI2OTdmYzFmZTIiLCJjc2kiOiJjN2Y4ZmZmMy0yNzE1LTRlZjQtOWRhMi1lMWJiM2E5NjQ0ZTciLCJuYW1laWQiOiIzMzljYmQxYi1iNmRkLTQzOGUtODE1OC0xODIzMjM4NTU3N2EiLCJpc3MiOiJhcHAudnNzcHMudmlzdWFsc3R1ZGlvLmNvbSIsImF1ZCI6ImFwcC52c3Nwcy52aXN1YWxzdHVkaW8uY29tIiwibmJmIjoxNDI0NjA0Mjc3LCJleHAiOjE0NTYxNDAyNzd9.wA1SuhNeUpR_Z69orvtWNFXBf5kslH1n3NW6Gr97YYsnsKuRP3DUzMsLn7fE6V_odMrS8lBUwFjoNKaZ37mS6Cf9ALCTGRAk1m4tpffiNQ4ovpC9Wp0NvpZB1pN_HTSNLO7dlBUs0CPwzRmh75DeI4Ghadr_w5JPrYhAWXQKvvS3bjMyi7ehr3f4rXVCWZt5tDRAFeDB6cLFrCuVv0pewHe86pSb_azJdibjmqeLn20fLRY6n1-n21wyMSl40VRDMHNs1yMBQ9IHsqRZ7Ct6DpXvMt19qkQ3UBeawc11_iZSZhsbSuwn1NlnO_55Dz24D35RAZII7MzM17F8kcgkyw"},
                    {"grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer"},
                    {"assertion", code},
                    {"redirect_uri", redirectUri}
                };

                var content = new FormUrlEncodedContent(dataDictionary);

                HttpResponseMessage response = await client.PostAsync(url, content);

                string responseResult = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var redisManager = new BasicRedisClientManager("localhost");
                    using (var redis = redisManager.GetClient())
                    {
                        var redisCredentials = redis.As<Auth>();
                        redisCredentials.FlushAll();

                        var credentialResponse = JsonConvert.DeserializeObject<Auth>(responseResult);
                        credentialResponse.generationTime = DateTime.UtcNow;

                        redisCredentials.Store(credentialResponse);
                    }
                }
               
                ViewBag.ResponseMessage = response.ReasonPhrase.ToString();
                ViewBag.Response = responseResult;
                return View("TeamProjects");
            }
        }

        public async Task<ActionResult> TeamProjects()
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
                    await client.GetAsync("https://rlbisbe.VisualStudio.com/DefaultCollection/_apis/projects?api-version=1.0");

                ViewBag.ResponseMessage = response.ReasonPhrase.ToString();
                ViewBag.Response = await response.Content.ReadAsStringAsync();
                return View();
            }
        }

        private async Task<Auth> RefreshToken(string refreshToken)
        {
            using (var client = new HttpClient())
            {
                var redirectUri = "https://dotnetconference.local:44300/home/Callback";
                var url = "https://app.vssps.visualstudio.com/oauth2/token";

                var dataDictionary = new Dictionary<string, string>() { 
                    {"client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer"},
                    {"client_assertion", "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Im9PdmN6NU1fN3AtSGpJS2xGWHo5M3VfVjBabyJ9.eyJjaWQiOiJjNzkyYmU1NS05ZGU5LTQ5NjgtOTQ0Yy03YTI2OTdmYzFmZTIiLCJjc2kiOiJjN2Y4ZmZmMy0yNzE1LTRlZjQtOWRhMi1lMWJiM2E5NjQ0ZTciLCJuYW1laWQiOiIzMzljYmQxYi1iNmRkLTQzOGUtODE1OC0xODIzMjM4NTU3N2EiLCJpc3MiOiJhcHAudnNzcHMudmlzdWFsc3R1ZGlvLmNvbSIsImF1ZCI6ImFwcC52c3Nwcy52aXN1YWxzdHVkaW8uY29tIiwibmJmIjoxNDI0NjA0Mjc3LCJleHAiOjE0NTYxNDAyNzd9.wA1SuhNeUpR_Z69orvtWNFXBf5kslH1n3NW6Gr97YYsnsKuRP3DUzMsLn7fE6V_odMrS8lBUwFjoNKaZ37mS6Cf9ALCTGRAk1m4tpffiNQ4ovpC9Wp0NvpZB1pN_HTSNLO7dlBUs0CPwzRmh75DeI4Ghadr_w5JPrYhAWXQKvvS3bjMyi7ehr3f4rXVCWZt5tDRAFeDB6cLFrCuVv0pewHe86pSb_azJdibjmqeLn20fLRY6n1-n21wyMSl40VRDMHNs1yMBQ9IHsqRZ7Ct6DpXvMt19qkQ3UBeawc11_iZSZhsbSuwn1NlnO_55Dz24D35RAZII7MzM17F8kcgkyw"},
                    {"grant_type", "refresh_token"},
                    {"assertion", refreshToken},
                    {"redirect_uri", redirectUri}
                };

                var content = new FormUrlEncodedContent(dataDictionary);

                HttpResponseMessage response = client.PostAsync(url, content).Result;

                string responseResult = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var redisManager = new BasicRedisClientManager("localhost");
                    using (var redis = redisManager.GetClient())
                    {
                        var redisCredentials = redis.As<Auth>();
                        redisCredentials.FlushAll();

                        var credentialResponse = JsonConvert.DeserializeObject<Auth>(responseResult);
                        credentialResponse.generationTime = DateTime.UtcNow;

                        redisCredentials.Store(credentialResponse);

                        return credentialResponse;
                    }
                }
            }
            throw new Exception("Error refreshing token");
        }
    }


    public class Auth
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public double expires_in { get; set; }
        public string refresh_token { get; set; }
        public string scope { get; set; }

        public DateTime generationTime { get; set; }
    }
#endif
}