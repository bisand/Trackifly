using System;

namespace Trackifly.Data.Models
{
    public class AccessToken
    {
        public string Token { get; set; }
        public DateTime Expires { get; set; }
    }
}