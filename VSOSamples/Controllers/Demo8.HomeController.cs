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
using VSOSamples.Hubs;
using Microsoft.AspNet.SignalR;
namespace VSOSamples.Controllers
{
#if DEMO3
    public partial class HomeController : Controller
    {
        public ActionResult Watch(ResponseResult data)
        {
            var clients = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>().Clients;
            clients.All.broadcastMessage(data);

            return new EmptyResult();
        }

        public ActionResult Dashboard()
        {
            return View();
        }
    }

    public class Message
    {
        public string text { get; set; }
        public string html { get; set; }
        public string markdown { get; set; }
    }

    public class ResponseResult
    {
        public string id { get; set; }
        public string eventType { get; set; }
        public string publisherId { get; set; }
        public Message message { get; set; }
    }

#endif
}