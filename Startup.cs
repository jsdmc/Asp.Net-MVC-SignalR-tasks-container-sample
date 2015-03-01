using Owin;
using Microsoft.Owin;
using System.Threading;

[assembly: OwinStartup(typeof(ASP.NET_MVC___SignalR_sample.Startup))]
namespace ASP.NET_MVC___SignalR_sample
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();

            //ThreadPool.SetMinThreads(7,7);
        }
    }
}