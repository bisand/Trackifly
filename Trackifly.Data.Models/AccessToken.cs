using System;

namespace Trackifly.Data.Models
{
    public class AccessToken
    {
        public AccessToken(string token)
        {
            Token = token;
            Expires = DateTime.Now.AddDays(1);
        }

        public AccessToken()
        {
            Token = Guid.NewGuid().ToString();
            Expires = DateTime.Now.AddDays(1);
        }

        public string Token { get; set; }
        public DateTime Expires { get; set; }
    }
}