using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WizBooklat.Startup))]
namespace WizBooklat
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
