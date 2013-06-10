using System;
using System.Collections.Generic;
using Nancy;
using Nancy.ModelBinding;
using Trackifly.Data;
using Trackifly.Data.Models;
using Trackifly.Data.Storage;
using Trackifly.Server.Configuration;
using Trackifly.Server.Helpers;

namespace Trackifly.Server.Modules
{
    public class TrackingModule : BaseModule
    {
        private static readonly Dictionary<string, DateTime> SessionCache = new Dictionary<string, DateTime>();
        private readonly TrackingSessions _trackingSessions;

        public TrackingModule(IDataStore dataStore, TrackingSessions trackingSessions, ErrorCodes errorCodes)
            : base("/tracking", dataStore, errorCodes)
        {
            _trackingSessions = trackingSessions;
            Get["/{sessionid}"] = parameters =>
                {
                    string sessionId = parameters.sessionId;
                    if (sessionId == null)
                        return ErrorResponse(HttpStatusCode.NotFound);
                    
                    var trackingSession = _trackingSessions.Get(sessionId);
                    if (trackingSession == null)
                        return ErrorResponse(HttpStatusCode.NotFound);
                    if (trackingSession.Expires <= DateTime.Now)
                        return ErrorResponse(HttpStatusCode.Forbidden, "Session has expired!");

                    return Response.AsJson(trackingSession);
                };
            Post["/"] = parameters =>
                {
                    Response response;
                    if (!CheckSaveRetention(SessionCache, out response))
                        return response;

                    var trackingSession = this.Bind<TrackingSession>();

                    _trackingSessions.Add(trackingSession);

                    return Response.AsJson(trackingSession);
                };
            Put["/{sessionid}"] = parameters =>
                {
                    Response response;
                    if (!CheckSaveRetention(SessionCache, out response))
                        return response;

                    string sessionId = parameters.sessionId;
                    if (sessionId == null)
                        return ErrorResponse(HttpStatusCode.NotFound);

                    var trackingSession = this.Bind<TrackingSession>();
                    trackingSession.Id = sessionId;

                    _trackingSessions.Update(trackingSession);

                    return Response.AsJson(trackingSession);
                };
            Delete["/{sessionid}"] = parameters =>
                {
                    string sessionId = parameters.sessionId;
                    if (sessionId != null)
                    {
                        _trackingSessions.Delete(sessionId);
                        return ErrorResponse(HttpStatusCode.OK,
                                             string.Format("Tracking session {0} is removed.", sessionId));
                    }
                    return ErrorResponse(HttpStatusCode.BadRequest);
                };
        }
    }
}