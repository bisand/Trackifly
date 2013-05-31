using System;
using System.Collections.Generic;
using Trackifly.Data.Models.Enums;

namespace Trackifly.Data.Models
{
    public class TrackingGroup : TrackingRoot
    {
        public TrackingGroup()
        {
            TrackingEntities = new List<TrackingEntity>();
        }

        public List<TrackingEntity> TrackingEntities { get; set; }
        public string DisplayName { get; set; }

        public void AddTrackingEntity(TrackingEntity entity)
        {
            TrackingEntities.Add(entity);
        }

        public void AddTrackingEntity(string displayName, TrackingType trackingType, DateTime expires)
        {
            var entity = new TrackingEntity
                {
                    DisplayName = displayName,
                    TrackingType = trackingType,
                    Expires = expires,
                };
            TrackingEntities.Add(entity);
        }
    }
}