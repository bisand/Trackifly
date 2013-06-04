using System;
using System.Collections.Generic;
using Nancy;
using Nancy.ModelBinding;
using Trackifly.Data;
using Trackifly.Data.Models;
using Trackifly.Data.Storage;
using Trackifly.Server.Configuration;
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
            Get["/tracking/{sessionid}"] = parameters =>
                {
                    string sessionId = parameters.sessionId;
                    if(sessionId != null)
                    {
                        var trackingSession = _trackingSessions.Get(sessionId);
                        if(trackingSession == null)
                            return ErrorResponse(HttpStatusCode.NotFound);
                        if (trackingSession.Expires >= DateTime.Now)
                            return ErrorResponse(HttpStatusCode.Forbidden, "Session has expired!");

                        return Response.AsJson(trackingSession);
                    }
                    return ErrorResponse(HttpStatusCode.NotFound);
                };
            Post["/tracking/"] = parameters =>
                {
                    var ip = Request.UserHostAddress;
                    DateTime createdDate;
                    if (SessionCache.TryGetValue(ip, out createdDate))
                    {
                        if (createdDate.AddSeconds(AppSettings.GlobalSaveRetention) > DateTime.Now)
                            return ErrorResponse(HttpStatusCode.TooManyRequests, string.Format("Please wait for at least {0} seconds before you create a new session.", AppSettings.GlobalSaveRetention));
                    }
                    SessionCache[ip] = DateTime.Now;

                    var trackingSession = this.Bind<TrackingSession>();
                    
                    _trackingSessions.Add(trackingSession);

                    return Response.AsJson(trackingSession);
                };
        }
    }
}