using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace VSOSamples.Controllers
{
#if DEMO1
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> TeamProjects()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://rlbisbe.VisualStudio.com");
                
                client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("/DefaultCollection/_apis/projects?api-version=1.0");
                response.EnsureSuccessStatusCode();

                ViewBag.ResponseMessage = response.ReasonPhrase.ToString();
                ViewBag.Response = await response.Content.ReadAsStringAsync();
                return View();
            }
        }
    }
#endif
}