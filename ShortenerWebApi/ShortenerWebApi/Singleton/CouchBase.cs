using System;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.Core;
using Couchbase.IO;
using ShortenerWebApi.Core;

namespace ShortenerWebApi.Singleton
{
    static class CouchBase
    {
        private static readonly IBucket _bucket;

        static CouchBase()
        {
            _bucket = new Cluster(Config.CouchbaseConfig).OpenBucket(Config.BucketName);
        }

        public static async Task<string> GetShortUrl(string longUrl)
        {
            // Get new ID
            var id = await _bucket.IncrementAsync(Config.PrimaryKey);
            var res = await _bucket.InsertAsync(id.Value.ToString(), new { url = longUrl });

            if (res.Success)
                return (new ShortUrl((long)id.Value)).ToString();

            if (res.Status == ResponseStatus.KeyExists)
                throw new CouchbaseDuplicateKeyException(res.Message);

            throw new Exception(res.Message);
        }

        public static async Task<string> GetLongUrl(string shortUrl)
        {
            // Get ID
            long id = new ShortUrl(shortUrl);
            var res = await _bucket.GetDocumentAsync<dynamic>(id.ToString());

            if (res.Success)
                return res.Document.Content.url;

            if (res.Status == ResponseStatus.KeyNotFound)
                throw new CouchbaseNotFoundException(res.Message);

            throw new Exception(res.Message);
        }

        public static async Task<ShortUrl> GetShortUrlObject(string longUrl)
        {
            // Get new ID
            var id = await _bucket.IncrementAsync(Config.PrimaryKey);
            var res = await _bucket.InsertAsync(id.Value.ToString(), new { url = longUrl });

            if (!res.Success)
                throw new Exception(res.Message);

            return new ShortUrl((long)id.Value);
        }
    }

    /// <summary>
    /// Exception class for couchbase
    /// </summary>
    public class CouchbaseNotFoundException : Exception
    {
        public CouchbaseNotFoundException()
        {
        }

        public CouchbaseNotFoundException(string message)
            : base(message)
        {
        }

        public CouchbaseNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Exception class for couchbase
    /// </summary>
    public class CouchbaseDuplicateKeyException : Exception
    {
        public CouchbaseDuplicateKeyException()
        {
        }

        public CouchbaseDuplicateKeyException(string message)
            : base(message)
        {
        }

        public CouchbaseDuplicateKeyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
