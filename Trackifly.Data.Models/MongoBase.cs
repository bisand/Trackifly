using System;

namespace Trackifly.Data.Models
{
    public class MongoBase
    {
        public MongoBase()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
    }
}