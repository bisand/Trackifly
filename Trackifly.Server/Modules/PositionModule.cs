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
    public class PositionModule : BaseModule
    {
        private static readonly Dictionary<string, DateTime> SessionCache = new Dictionary<string, DateTime>();
        private readonly TrackingSessions _trackingSessions;

        public PositionModule(IDataStore dataStore, TrackingUsers trackingUsers, TrackingSessions trackingSessions,
                              ErrorCodes errorCodes)
            : base("/position", dataStore, trackingUsers, errorCodes)
        {
            _trackingSessions = trackingSessions;
            Before += ctx =>
                {
                    var user = Context.CurrentUser as UserIdentity;
                    return user == null
                               ? ErrorResponse(HttpStatusCode.Unauthorized,
                                               "Invalid access token! Please login to obtain a new access token.")
                               : null;
                };

            Post["/{sessionId}"] = parameters =>
                {
                    var user = Context.CurrentUser as UserIdentity;
                    if (user == null)
                        return ErrorResponse(HttpStatusCode.Unauthorized);

                    Response response;
                    if (!CheckSaveRetention(SessionCache, out response))
                        return response;

                    string sessionId = parameters.sessionId;
                    if (sessionId == null)
                        return ErrorResponse(HttpStatusCode.NotFound);

                    var trackingPosition = this.Bind<TrackingPosition>();
                    var trackingSession = _trackingSessions.Get(sessionId);
                    if (trackingSession.UserId != user.UserId)
                        return ErrorResponse(HttpStatusCode.Unauthorized);

                    trackingSession.Positions.Add(trackingPosition);
                    _trackingSessions.Update(trackingSession);

                    return Response.AsJson(trackingPosition);
                };
        }
    }
}