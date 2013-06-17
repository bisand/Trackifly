using System.Configuration;

namespace Trackifly.Server.Configuration
{
    public static class AppSettings
    {
        static AppSettings()
        {
            LoadConfiguration();
        }

        public static int ServerPort { get; private set; }
        public static string ConnectionString { get; private set; }
        public static string DatabaseName { get; private set; }
        public static int GlobalSaveRetention { get; private set; }
        public static int GlobalLoginRetention { get; private set; }
        public static int MaxLoadRequestsPerMinute { get; private set; }
        public static int MaxSaveRequestsPerMinute { get; private set; }


        public static void LoadConfiguration()
        {
            int intResult;
            ConnectionString = ConfigurationManager.AppSettings["ConnectionString"] ?? "mongodb://localhost";
            DatabaseName = ConfigurationManager.AppSettings["DatabaseName"] ?? "trackifly";

            ServerPort =
                int.TryParse(ConfigurationManager.AppSettings["ServerPort"], out intResult)
                    ? intResult
                    : 8888;
            GlobalSaveRetention =
                int.TryParse(ConfigurationManager.AppSettings["GlobalSaveRetention"], out intResult)
                    ? intResult
                    : 10;
            GlobalSaveRetention =
                int.TryParse(ConfigurationManager.AppSettings["GlobalLoginRetention"], out intResult)
                    ? intResult
                    : 2;
            MaxLoadRequestsPerMinute =
                int.TryParse(ConfigurationManager.AppSettings["MaxLoadRequestsPerMinute"],
                             out intResult)
                    ? intResult
                    : 60;
            MaxSaveRequestsPerMinute =
                int.TryParse(ConfigurationManager.AppSettings["MaxSaveRequestsPerMinute"],
                             out intResult)
                    ? intResult
                    : 10;
        }
    }
}