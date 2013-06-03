using System;

namespace Trackifly.Data.Models
{
    public class TrackingPosition
    {
        public TrackingPosition()
        {
            PositionTime = DateTime.Now;
        }

        public TrackingPosition(decimal latitude, decimal longditude)
            : this()
        {
            Longditude = longditude;
            Latitude = latitude;
        }

        public decimal Longditude { get; set; }
        public decimal Latitude { get; set; }
        public DateTime PositionTime { get; set; }
    }
}