using System;
using System.Collections.Generic;
using Nancy;
using Nancy.ModelBinding;
using Trackifly.Data;
using Trackifly.Data.Models;
using Trackifly.Data.Storage;
using Trackifly.Server.Models;

namespace Trackifly.Server.Modules
{
    public class TrackingModule : NancyModule
    {
        private static Dictionary<string,DateTime> _createdSessions = new Dictionary<string, DateTime>(); 
        private readonly IDataStore _dataStore;

        public TrackingModule(IDataStore dataStore, TrackingSessions trackingSessions)
        {
            _dataStore = dataStore;
            Get["/tracking/"] = parameters =>
                {
                    var trackingSession = this.Bind<TrackingSession>();

                    return "OK";
                };
            Post["/tracking/"] = parameters =>
                {
                    var ip = Request.UserHostAddress;
                    DateTime createdDate;
                    if (_createdSessions.TryGetValue(ip, out createdDate))
                    {
                        if (createdDate.AddSeconds(10) > DateTime.Now)
                            return Response.AsJson(new ErrorModel
                            {
                                Error = 10,
                                Description = "Too early to create a new session! Try again later."
                            });
                    }
                    _createdSessions[ip] = DateTime.Now;

                    var trackingSession = this.Bind<TrackingSession>();
                    
                    trackingSessions.Add(trackingSession);

                    return Response.AsJson(trackingSession);
                };
        }
    }
}