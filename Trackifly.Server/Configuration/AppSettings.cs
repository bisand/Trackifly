using System.Configuration;
using MongoDB.Driver;

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

        public static string ConnectionString
        {
            get
            {
                var value = ConfigurationManager.AppSettings["ConnectionString"] ?? "mongodb://localhost";
                return value;
            }
        }

        public static string DatabaseName
        {
            get
            {
                var value = ConfigurationManager.AppSettings["DatabaseName"] ?? "trackifly";
                return value;
            }
        }
    }
}