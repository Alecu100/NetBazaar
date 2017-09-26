using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NetBazaar.Startup))]
namespace NetBazaar
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
