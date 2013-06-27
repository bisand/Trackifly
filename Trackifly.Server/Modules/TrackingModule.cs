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
        private static readonly Dictionary<string, DateTime> SessionCache = new Dictionary<string, DateTime>();
        private readonly TrackingSessions _trackingSessions;

        public TrackingModule(IDataStore dataStore, TrackingUsers trackingUsers, TrackingSessions trackingSessions, ErrorCodes errorCodes)
            : base("/tracking", dataStore, trackingUsers, errorCodes)
        {
            Before += ctx =>
                {
                    var user = Context.CurrentUser as UserIdentity;
                    return user == null
                        ? ErrorResponse(HttpStatusCode.Unauthorized, "Invalid access token! Please login to obtain a new access token.")
                        : null;
                };

            _trackingSessions = trackingSessions;
            Get["/{sessionid}"] = parameters =>
                {
                    var user = Context.CurrentUser as UserIdentity;

                    string sessionId = parameters.sessionId;
                    if (sessionId == null)
                        return ErrorResponse(HttpStatusCode.NotFound);
                    var trackingSession = _trackingSessions.Get(sessionId);
                    if (trackingSession == null)
                        return ErrorResponse(HttpStatusCode.NotFound);
                    if (user == null || trackingSession.UserId != user.UserId)
                        return ErrorResponse(HttpStatusCode.Unauthorized);
                    if (trackingSession.Expires <= DateTime.Now)
                        return ErrorResponse(HttpStatusCode.Forbidden, "Session has expired!");

                    return Response.AsJson(trackingSession);
                };
            Post["/"] = parameters =>
                {
                    var user = Context.CurrentUser as UserIdentity;
                    if (user == null)
                        return ErrorResponse(HttpStatusCode.Unauthorized);

                    Response response;
                    if (!CheckSaveRetention(SessionCache, out response))
                        return response;

                    var trackingSession = this.Bind<TrackingSession>();
                    trackingSession.UserId = user.UserId;
                    _trackingSessions.Add(trackingSession);

                    return Response.AsJson(trackingSession);
                };
            Put["/{sessionid}"] = parameters =>
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

                    var trackingSession = this.Bind<TrackingSession>();
                    trackingSession.Id = sessionId;
                    trackingSession.UserId = user.UserId;

                    _trackingSessions.Update(trackingSession);

                    return Response.AsJson(trackingSession);
                };
            Delete["/{sessionid}"] = parameters =>
                {
                    var user = Context.CurrentUser as UserIdentity;
                    if (user == null)
                        return ErrorResponse(HttpStatusCode.Unauthorized);

                    string sessionId = parameters.sessionId;
                    if (sessionId != null)
                    {
                        var trackingSession = _trackingSessions.Get(sessionId);
                        if (trackingSession == null)
                            return ErrorResponse(HttpStatusCode.NotFound);
                        if (trackingSession.UserId != user.UserId)
                            return ErrorResponse(HttpStatusCode.Unauthorized);

                        _trackingSessions.Delete(sessionId);
                        return ErrorResponse(HttpStatusCode.OK,
                                             string.Format("Tracking session '{0}' including all its positions is removed.", sessionId));
                    }
                    return ErrorResponse(HttpStatusCode.BadRequest);
                };
        }
    }
}