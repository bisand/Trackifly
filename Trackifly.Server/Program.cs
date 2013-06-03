using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Hosting.Self;
using Topshelf;
using Trackifly.Server.Configuration;

namespace Trackifly.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>                                 //1
            {
                x.Service<NancyService>(s =>                        //2
                {
                    s.ConstructUsing(name => new NancyService());     //3
                    s.WhenStarted(tc => tc.Start());              //4
                    s.WhenStopped(tc => tc.Stop());               //5
                });
                x.RunAsLocalSystem();                            //6

                x.SetDescription("Sample Topshelf Host");        //7
                x.SetDisplayName("Stuff");                       //8
                x.SetServiceName("stuff");                       //9
            });                                                  //10
        }
    }

    internal class NancyService
    {
        private readonly NancyHost _host;

        public NancyService()
        {
            var port = AppSettings.ServerPort;
            var uriBuilder = new UriBuilder("http", "localhost", port);
            INancyBootstrapper bootStrapper = new DefaultNancyBootstrapper();
            
            _host = new NancyHost(uriBuilder.Uri);
        }

        public void Start()
        {
            _host.Start(); // start hosting
        }

        public void Stop()
        {
            _host.Stop(); // start hosting
        }
    }
}
