using Microsoft.Owin;
using Owin;
using Shortener;
using Shortener.Core;

[assembly: OwinStartup(typeof(Startup))]

namespace Shortener
{
    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Use(typeof(ShortenerMiddleware));
        }
    }
}
