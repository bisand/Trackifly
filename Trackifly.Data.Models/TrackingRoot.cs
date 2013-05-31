using System;

namespace Trackifly.Data.Models
{
    public abstract class TrackingRoot : MongoBase
    {
        protected TrackingRoot()
        {
            DateCreated = DateTime.Now;
            Expires = DateTime.MaxValue;
        }

        public DateTime DateCreated { get; set; }
        public DateTime Expires { get; set; }
    }
}