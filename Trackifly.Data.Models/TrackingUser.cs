using System;

namespace Trackifly.Data.Models
{
    public class TrackingUser : MongoBase
    {
        public TrackingUser()
        {
            DateCreated = DateTime.Now;
        }

        public TrackingUser(string email, string name)
            : this()
        {
            Name = name;
            Email = email;
        }

        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime DateCreated { get; set; }
    }
}