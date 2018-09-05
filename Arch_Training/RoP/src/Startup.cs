using System.Threading.Tasks;
using Microsoft.Owin;
using MvcMusicStore;
using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace MvcMusicStore
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            ConfigureApp(app).Wait();
        }
    }
}