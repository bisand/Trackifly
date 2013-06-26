using System;
using System.Collections.Generic;
using Trackifly.Data.Models.Enums;

namespace Trackifly.Data.Models
{
    public class TrackingGroup : TrackingRoot
    {
        public TrackingGroup()
        {
            TrackingSessionIds = new List<string>();
        }

        public string OwnerId { get; set; }
        public List<string> TrackingSessionIds { get; set; }
        public string DisplayName { get; set; }

        public void AddTrackingSessionId(string session)
        {
            TrackingSessionIds.Add(session);
        }
    }
}