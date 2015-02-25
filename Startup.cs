using Owin;
using Microsoft.Owin;

[assembly: OwinStartup(typeof(ASP.NET_MVC___SignalR_sample.Startup))]
namespace ASP.NET_MVC___SignalR_sample
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}