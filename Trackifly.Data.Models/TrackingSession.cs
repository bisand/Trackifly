using System.Collections.Generic;
using System.Linq;
using Trackifly.Data.Models.Enums;

namespace Trackifly.Data.Models
{
    public class TrackingSession : TrackingRoot
    {
        public TrackingSession()
        {
            TrackingType = TrackingType.Single;
            Positions = new List<TrackingPosition>();
        }

        public TrackingSession(string userId)
        {
            UserId = userId;
        }

        public TrackingSession(string userId, TrackingPosition position)
        {
            UserId = userId;
            AddPosition(position);
        }

        public TrackingSession(string userId, string displayName, TrackingPosition position)
        {
            UserId = userId;
            DisplayName = displayName;
            AddPosition(position);
        }

        public TrackingSession(string userId, string displayName, TrackingPosition position,
                               TrackingType trackingType)
        {
            UserId = userId;
            DisplayName = displayName;
            TrackingType = trackingType;
            AddPosition(position);
        }

        public List<TrackingPosition> Positions { get; set; }
        public string DisplayName { get; set; }
        public TrackingType TrackingType { get; set; }
        public string UserId { get; set; }
        public string GroupId { get; set; }

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