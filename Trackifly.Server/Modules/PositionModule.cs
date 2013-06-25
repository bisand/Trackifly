using System;
using System.Collections.Generic;
using Nancy;
using Nancy.ModelBinding;
using Trackifly.Data;
using Trackifly.Data.Models;
using Trackifly.Data.Storage;
using Trackifly.Server.Helpers;

namespace Trackifly.Server.Modules
{
    public class PositionModule : BaseModule
    {
        private static readonly Dictionary<string, DateTime> SessionCache = new Dictionary<string, DateTime>();
        private readonly TrackingSessions _trackingSessions;

        public PositionModule(IDataStore dataStore, TrackingUsers trackingUsers, TrackingSessions trackingSessions, ErrorCodes errorCodes)
            : base("/position", dataStore, trackingUsers, errorCodes)
        {
            Before += ctx =>
            {
                if (Context.CurrentUser == null)
                    return ErrorResponse(HttpStatusCode.Unauthorized, "Invalid access token! Please login to obtain a new access token.");

                return null;
            };

            Post["/{sessionId}"] = parameters =>
            {
                Response response;
                if (!CheckSaveRetention(SessionCache, out response))
                    return response;

                    string sessionId = parameters.sessionId;
                    if (sessionId == null)
                        return ErrorResponse(HttpStatusCode.NotFound);
                    
                var trackingPosition = this.Bind<TrackingPosition>();
                var trackingSession = _trackingSessions.Get(sessionId);
                trackingSession.Positions.Add(trackingPosition);
                _trackingSessions.Update(trackingSession);

                return Response.AsJson(trackingPosition);
            };

        }
    }
}