using System;
using Microsoft.Owin.Hosting;
using ShortenerWebApi.Singleton;

namespace ShortenerWebApi
{
    class WebApiService
    {
        private IDisposable _webApplication;

        public void Start()
        {
            // Init
            var baseAddress = Config.OwinHost;
            
            Logger.Log.Info("**************************");
            Logger.Log.Info("*** Self-hosted WebApi ***");
            Logger.Log.Info("**************************");

            _webApplication = WebApp.Start<Startup>(baseAddress);

            Logger.Log.Info("Service running at address {0}", baseAddress);
        }

        public void Stop()
        {
            _webApplication.Dispose();
            Logger.Log.Info("**************************");
            Logger.Log.Info("***      STOPPED       ***");
            Logger.Log.Info("**************************");
        }
    }
}
