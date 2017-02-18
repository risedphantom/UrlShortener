using System;
using System.Configuration;
using Couchbase.Configuration.Client;
using Couchbase.Configuration.Client.Providers;

namespace Shortener.Singleton
{
    static class Config
    {
        #region --- Strongly typed settings ---
        public static string Host { get; }

        // Connection strings
        public static ClientConfiguration CouchbaseConfig { get; }

        // Error pages
        public static string NotFoundPage { get; }
        public static string ErrorPage { get; }
        #endregion

        static Config()
        {
            try
            {
                Host = ConfigurationManager.AppSettings["host"];

                //Connection strings
                CouchbaseConfig = new ClientConfiguration((CouchbaseClientSection)ConfigurationManager.GetSection("couchbaseClients/couchbase"));

                // Error pages
                NotFoundPage = ConfigurationManager.AppSettings["notfound.page"];
                ErrorPage = ConfigurationManager.AppSettings["error.page"];
            }
            catch (Exception ex)
            {
                Logger.Log.Fatal("Error at reading config: {0}", ex.Message);
                throw;
            }
        }
    }
}
