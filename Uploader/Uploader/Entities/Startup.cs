using Microsoft.Owin;
using Owin;
[assembly: OwinStartup(typeof(Uploader.Entities.Startup))]
namespace Uploader.Entities
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Any connection or hub wire up and configuration should go here
            app.MapSignalR();
        }
    }
}