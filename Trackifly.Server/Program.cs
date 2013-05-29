using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy.Hosting.Self;
using Topshelf;

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
        private NancyHost _host;

        public NancyService()
        {
            const string uri = "http://localhost:8888";
            Console.WriteLine(uri);
            // initialize an instance of NancyHost (found in the Nancy.Hosting.Self package)
            _host = new NancyHost(new Uri(uri));
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
