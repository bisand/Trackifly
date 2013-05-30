using System.Configuration;

namespace Trackifly.Server.Configuration
{
    public class AppSettings
    {
        public static int ServerPort
        {
            get
            {
                int port;
                var value = ConfigurationManager.AppSettings["ServerPort"] ?? "8888";
                return int.TryParse(value, out port) ? port : 8888;
            }
        }
    }
}