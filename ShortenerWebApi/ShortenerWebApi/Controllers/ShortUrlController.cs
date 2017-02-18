using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using ShortenerWebApi.Singleton;

namespace ShortenerWebApi.Controllers
{
    [RoutePrefix("api/rest")]
    public class ShortUrlController : ApiController
    {
        // GET api/rest/universe_answer
        [HttpGet]
        [Route("universe_answer")]
        public string AnswerForUniverse()
        {
            Logger.Log.Trace("AnswerForUniverse called.");
            return "42";
        }

        // GET api/rest/getShortUrl
        [HttpGet]
        [Route("getShortUrl")]
        public async Task<HttpResponseMessage> GetShortUrl(string longUrl)
        {
            Logger.Log.Trace("GetShortUrl called.");

            try
            {
                var shortUrl = await CouchBase.GetShortUrl(longUrl);
                return Request.CreateResponse(HttpStatusCode.OK, shortUrl);
            }
            catch (CouchbaseDuplicateKeyException ex)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, ex.Message);
            }
        }

        // GET api/rest/getLongUrl
        [HttpGet]
        [Route("getLongUrl")]
        public async Task<HttpResponseMessage> GetLongUrl(string shortUrl)
        {
            Logger.Log.Trace("GetLongUrl called.");

            try
            {
                var longUrl = await CouchBase.GetLongUrl(shortUrl);
                return Request.CreateResponse(HttpStatusCode.OK, longUrl);
            }
            catch (CouchbaseNotFoundException ex)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message);
            }
        }
    }
}
