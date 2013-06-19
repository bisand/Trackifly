using System;
using System.Collections.Generic;

namespace Trackifly.Data.Models
{
    public class TrackingUser : MongoBase
    {
        public TrackingUser()
        {
            DateCreated = DateTime.Now;
            Active = true;
            AccessToken = new AccessToken(Guid.NewGuid().ToString());
        }

        public TrackingUser(string username, byte[] password, byte[] passwordSalt)
            : this()
        {
            Username = username;
            Password = password;
            Salt = passwordSalt;
        }

        public string Name { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public byte[] Password { get; set; }
        public byte[] Salt { get; set; }
        public bool Active { get; set; }
        public DateTime DateCreated { get; set; }
        public List<string> Claims { get; set; }
        public AccessToken AccessToken { get; set; }
    }
}