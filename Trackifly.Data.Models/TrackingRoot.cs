using System;

namespace Trackifly.Data.Models
{
    public abstract class TrackingRoot
    {
        public string Id { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime Expires { get; set; }
    }
}