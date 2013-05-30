using System;
using System.Collections.Generic;
using System.Linq;
using Trackifly.Data.Models.Enums;

namespace Trackifly.Data.Models
{
    public class TrackingEntity : TrackingRoot
    {
        public TrackingEntity()
        {
            TrackingType = TrackingType.Single;
            DateCreated = DateTime.Now;
            Expires = DateTime.MaxValue;
            Positions = new List<TrackingPosition>();
        }

        public List<TrackingPosition> Positions { get; set; }
        public string DisplayName { get; set; }
        public TrackingType TrackingType { get; set; }

        public void AddPosition(TrackingPosition position)
        {
            switch (TrackingType)
            {
                case TrackingType.Single:
                    Positions = new List<TrackingPosition> {position};
                    break;
                case TrackingType.TimeLimited:
                    Positions.Add(position);
                    Positions = Positions.Where(x => x.PositionTime < Expires).ToList();
                    break;
                default:
                    Positions.Add(position);
                    break;
            }
        }
    }
}