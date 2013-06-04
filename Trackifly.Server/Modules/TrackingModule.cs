using System;
using System.Collections.Generic;
using Nancy;
using Nancy.ModelBinding;
using Trackifly.Data;
using Trackifly.Data.Models;
using Trackifly.Data.Storage;
using Trackifly.Server.Helpers;
using Trackifly.Server.Models;

namespace Trackifly.Server.Modules
{
    public class TrackingModule : BaseModule
    {
        private static readonly Dictionary<string,DateTime> SessionCache = new Dictionary<string, DateTime>();
        private readonly TrackingSessions _trackingSessions;

        public TrackingModule(IDataStore dataStore, TrackingSessions trackingSessions, ErrorCodes errorCodes) : base(dataStore, errorCodes)
        {
            _trackingSessions = trackingSessions;
            Get["/tracking/"] = parameters =>
                {
                    var trackingSession = this.Bind<TrackingSession>();

                    return "OK";
                };
            Post["/tracking/"] = parameters =>
                {
                    var ip = Request.UserHostAddress;
                    DateTime createdDate;
                    if (SessionCache.TryGetValue(ip, out createdDate))
                    {
                        if (createdDate.AddSeconds(10) > DateTime.Now)
                            return Response.AsJson(new ErrorModel
                                {
                                    Error = 10,
                                    Description = ErrorCodes[10]
                                });
                    }
                    SessionCache[ip] = DateTime.Now;

                    var trackingSession = this.Bind<TrackingSession>();
                    
                    _trackingSessions.Add(trackingSession);

                    return Response.AsJson(trackingSession);
                };
        }
    }
}