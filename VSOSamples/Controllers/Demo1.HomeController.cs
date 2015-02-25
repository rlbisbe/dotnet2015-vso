using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace VSOSamples.Controllers
{
#if DEMO1
    public partial class HomeController : Controller
    {
        public async Task<ActionResult> TeamProjects()
        {
            using (var client = new HttpClient())
            {
                HttpResponseMessage response 
                    = await client.GetAsync("https://rlbisbe.VisualStudio.com/DefaultCollection/_apis/projects?api-version=1.0");
                
                ViewBag.ResponseMessage = response.ReasonPhrase.ToString();
                ViewBag.Response = await response.Content.ReadAsStringAsync();
                return View();
            }
        }
    }
#endif
}