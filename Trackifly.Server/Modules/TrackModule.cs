using Nancy;

namespace Trackifly.Server.Modules
{
    public class TrackModule : NancyModule
    {
        public TrackModule()
        {
            Get["/track/"] = parameters =>
                {
                    return "OK";
                };
        }
    }
}