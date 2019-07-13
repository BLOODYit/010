using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Marsad.Startup))]
namespace Marsad
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
