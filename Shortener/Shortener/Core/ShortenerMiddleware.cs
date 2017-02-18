using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Owin;
using Shortener.Singleton;

namespace Shortener.Core
{
    class ShortenerMiddleware : OwinMiddleware
    {
        public ShortenerMiddleware(OwinMiddleware next) :
            base(next) { }

        public override async Task Invoke(IOwinContext context)
        {
            var redirect = "";
            var request = context.Request;
            var status = HttpStatusCode.OK;
            var sw = new Stopwatch();
            
            sw.Start();

            try
            {
                if (request.Uri.Host != Config.Host)
                {
                    redirect = request.Uri.AbsoluteUri;
                    await Next.Invoke(context);
                    return;
                }

                var url = request.Uri.PathAndQuery.Replace("/", "");
                var key = new ShortUrl(url).ToInt();

                // Get value from cache
                redirect = await CouchBase.GetValueAsync(key);
            }
            catch (ShortUrlException)
            {
                status = HttpStatusCode.NotFound;
                redirect = Config.NotFoundPage;
            }
            catch (CouchbaseNotFoundException)
            {
                status = HttpStatusCode.NotFound;
                redirect = Config.NotFoundPage;
            }
            catch (Exception ex)
            {
                status = HttpStatusCode.InternalServerError;
                redirect = Config.ErrorPage;
                Logger.Log.Error("Internal server error: {0}", ex.Message);
            }
            finally
            {
                // Redirect
                context.Response.Redirect(redirect);

                sw.Stop();
                // Log request
                // Format: URL User-Agent Status Redirect Timing 
                Logger.Request.Info("{0} {1} {2} {3} {4}",
                    request.Uri.PathAndQuery.Replace(" ", "+"),
                    request.Headers["User-Agent"].Replace(" ", "+"),
                    (int)status,
                    redirect,
                    sw.ElapsedMilliseconds); 
            }
        }
    }
}
