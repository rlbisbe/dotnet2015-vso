using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace VSOSamples.Controllers
{
#if DEMO2
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> TeamProjects()
        {
            string username = "rlbisbe";
            string password = "Abc123456";

            using (var client = new HttpClient())
            {

                client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(
                        System.Text.ASCIIEncoding.ASCII.GetBytes(
                            string.Format("{0}:{1}", username, password))));


                HttpResponseMessage response = 
                    await client.GetAsync("https://rlbisbe.VisualStudio.com/DefaultCollection/_apis/projects?api-version=1.0");
                response.EnsureSuccessStatusCode();

                ViewBag.ResponseMessage = response.ReasonPhrase.ToString();
                ViewBag.Response = await response.Content.ReadAsStringAsync();
                return View();
            }
        }
    }
#endif
}