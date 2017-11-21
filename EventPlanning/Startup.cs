using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EventPlanning.Startup))]
namespace EventPlanning
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
