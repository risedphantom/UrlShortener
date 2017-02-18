using System;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.Core;
using Couchbase.IO;
using Shortener.Singleton;

namespace Shortener.Core
{
    /// <summary>
    /// Class to interact with memcached database
    /// </summary>
    static class CouchBase
    {
        private static readonly IBucket _bucket;
        
        static CouchBase()
        {
            _bucket = new Cluster(Config.CouchbaseConfig).OpenBucket("shortener");
        }

        public static async Task<string> GetValueAsync(long key)
        {
            var docResult = await _bucket.GetDocumentAsync<dynamic>(key.ToString());
            
            if (docResult.Status == ResponseStatus.KeyNotFound)
                throw new CouchbaseNotFoundException("Key not found");

            return docResult.Document.Content.url;
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
}
