using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Homeschooledsystem.Startup))]
namespace Homeschooledsystem
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
