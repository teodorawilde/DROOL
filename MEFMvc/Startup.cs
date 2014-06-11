using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MEFMvc.Startup))]
namespace MEFMvc
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
