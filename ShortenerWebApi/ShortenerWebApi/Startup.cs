using Owin;
using System.Web.Http;
using ShortenerWebApi.Core;

namespace ShortenerWebApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var httpConfig = new HttpConfiguration();

            // Configure Web API Routes:
            // - Enable Attribute Mapping
            // - Enable Default routes at /api.
            httpConfig.MapHttpAttributeRoutes();

            app.Use(typeof(LoggingMiddleware));
            app.UseWebApi(httpConfig);
        }
    }
}
