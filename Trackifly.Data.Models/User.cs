using System;

namespace Trackifly.Data.Models
{
    public class User : MongoBase
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime DateCreated { get; set; }
    }
}