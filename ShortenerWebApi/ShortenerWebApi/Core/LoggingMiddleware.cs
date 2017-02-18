using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ShortenerWebApi.Singleton;
using Microsoft.Owin;

namespace ShortenerWebApi.Core
{
    public class LoggingMiddleware : OwinMiddleware
    {
        public LoggingMiddleware(OwinMiddleware next) : base(next)
        {
        }

        public async override Task Invoke(IOwinContext context)
        {
            Logger.Log.Warn("{0} {1}{2} called", context.Request.Method, context.Request.Path, context.Request.QueryString);
            Logger.Log.Trace("--REQUEST--\r\n{0}", HeadersStringify(context.Request.Headers));

            // Make response readable
            var responseBuffer = new MemoryStream();
            var responseStream = new ContentStream(responseBuffer, context.Response.Body);
            context.Response.Body = responseStream;

            await Next.Invoke(context);

            Logger.Log.Trace("--RESPONSE--\r\n{0}", HeadersStringify(context.Response.Headers));
            if (context.Response.StatusCode != (int)HttpStatusCode.OK)
                Logger.Log.Error(await BodyStringify(responseStream, context.Response.Headers));
        }

        private static string HeadersStringify(IHeaderDictionary headers)
        {
            return headers.Aggregate("", (current, header) => current + string.Format("{0}: {1}\r\n", header.Key, string.Join("\r\n", header.Value)));
        }

        private static async Task<string> BodyStringify(ContentStream body, IDictionary<string, string[]> headers)
        {
            var contentType = headers.ContainsKey("Content-Type") ? headers["Content-Type"][0] : null;

            return await body.ReadContentAsync(contentType);
        }
    }
}
