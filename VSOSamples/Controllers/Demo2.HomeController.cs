using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace VSOSamples.Controllers
{
#if DEMO2
    public partial class HomeController : Controller
    {
        public async Task<ActionResult> TeamProjects()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(
                        System.Text.ASCIIEncoding.ASCII.GetBytes(
                            string.Format("{0}:{1}", "rlbisbe", "Abc123456"))));

                HttpResponseMessage response = 
                    await client.GetAsync("https://rlbisbe.VisualStudio.com/DefaultCollection/_apis/projects?api-version=1.0");

                ViewBag.ResponseMessage = response.ReasonPhrase.ToString();
                ViewBag.Response = await response.Content.ReadAsStringAsync();
                return View();
            }
        }
    }
#endif
}