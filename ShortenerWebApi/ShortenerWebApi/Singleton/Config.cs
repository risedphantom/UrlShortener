using System;
using System.Configuration;
using Couchbase.Configuration.Client;
using Couchbase.Configuration.Client.Providers;

namespace ShortenerWebApi.Singleton
{
    /// <summary>
    /// Singleton: configuration class
    /// </summary>
    static class Config
    {
        #region --- Strongly typed settings ---
        // Connection strings
        public static ClientConfiguration CouchbaseConfig { get; private set; }

        // OWIN settings
        public static string OwinHost { get; private set; }

        // Couchbase settings
        public static string PrimaryKey { get; private set; }
        public static string BucketName { get; private set; }
        #endregion

        static Config()
        {
            try
            {
                // Get OWIN settings
                OwinHost = ConfigurationManager.AppSettings["owin.host"];

                // Get couchbase settings
                PrimaryKey = ConfigurationManager.AppSettings["primary.key"];
                BucketName = ConfigurationManager.AppSettings["bucket.name"];

                // Connection strings
                CouchbaseConfig = new ClientConfiguration((CouchbaseClientSection)ConfigurationManager.GetSection("couchbaseClients/couchbase"));
            }
            catch (Exception ex)
            {
                Logger.Log.Fatal("Error at reading config: {0}", ex.Message);
                throw;
            }
        }
    }
}
