using System;
using System.Collections.Generic;
using Nancy;
using Trackifly.Data;
using Trackifly.Data.Storage;
using Trackifly.Server.Helpers;
using Trackifly.Server.Models;

namespace Trackifly.Server.Modules
{
    public class TrackingGroupModule : BaseModule
    {
        private static readonly Dictionary<string, DateTime> SessionCache = new Dictionary<string, DateTime>();
        private readonly TrackingGroups _trackingGroups;

        public TrackingGroupModule(IDataStore dataStore, TrackingUsers trackingUsers, TrackingGroups trackingGroups,
                                   ErrorCodes errorCodes)
            : base("/tracking/group", dataStore, trackingUsers, errorCodes)
        {
            _trackingGroups = trackingGroups;
            Before += ctx =>
                {
                    var user = Context.CurrentUser as UserIdentity;
                    return user == null
                               ? ErrorResponse(HttpStatusCode.Unauthorized,
                                               "Invalid access token! Please login to obtain a new access token.")
                               : null;
                };

            Get["/{groupid}"] = parameters =>
                {
                    string groupId = parameters.groupId;
                    if (groupId == null)
                        return ErrorResponse(HttpStatusCode.NotFound);

                    var trackingGroup = _trackingGroups.Get(groupId);
                    if (trackingGroup == null)
                        return ErrorResponse(HttpStatusCode.NotFound);
                    if (trackingGroup.Expires <= DateTime.Now)
                        return ErrorResponse(HttpStatusCode.Forbidden, "Session has expired!");

                    return Response.AsJson(trackingGroup);
                };
        }
    }
}