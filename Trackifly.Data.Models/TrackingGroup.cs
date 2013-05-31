using System;
using System.Collections.Generic;
using Trackifly.Data.Models.Enums;

namespace Trackifly.Data.Models
{
    public class TrackingGroup : TrackingRoot
    {
        public TrackingGroup()
        {
            TrackingEntities = new List<TrackingSession>();
        }

        public List<TrackingSession> TrackingEntities { get; set; }
        public string DisplayName { get; set; }

        public void AddTrackingEntity(TrackingSession session)
        {
            TrackingEntities.Add(session);
        }

        public void AddTrackingEntity(string displayName, TrackingType trackingType, DateTime expires)
        {
            var entity = new TrackingSession
                {
                    DisplayName = displayName,
                    TrackingType = trackingType,
                    Expires = expires,
                };
            TrackingEntities.Add(entity);
        }
    }
}