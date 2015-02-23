using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(VSOSamples.Startup))]
namespace VSOSamples
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
