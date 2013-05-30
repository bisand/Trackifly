using System;

namespace Trackifly.Data.Models
{
    public class TrackingPosition
    {
        public decimal Longditude { get; set; }
        public decimal Latitude { get; set; }
        public DateTime PositionTime { get; set; }
    }
}