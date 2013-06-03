using System;

namespace Trackifly.Data.Models
{
    public class User : MongoBase
    {
        public User()
        {
            DateCreated = DateTime.Now;
        }

        public User(string email, string name)
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