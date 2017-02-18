using Topshelf;

namespace ShortenerWebApi
{
    class Program
    {
        static void Main()
        {
            HostFactory.Run(x =>
            {
                x.Service<WebApiService>(s =>
                {
                    s.ConstructUsing(name => new WebApiService());
                    s.WhenStarted(svc => svc.Start());
                    s.WhenStopped(svc => svc.Stop());
                });

                x.RunAsLocalSystem();
                x.SetDescription("Служба для взаимодействия с кэшем коротких ссылок");
                x.SetDisplayName("Shortener WebApi");
                x.SetServiceName("ShortenerWebApi");
            });
        }
    }
}
