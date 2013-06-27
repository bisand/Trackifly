using System.Collections.Generic;
using Nancy.Security;

namespace Trackifly.Server.Models
{
    public class UserIdentity : IUserIdentity
    {
        public UserIdentity(string username, IEnumerable<string> claims)
        {
            UserName = username;
            Claims = claims;
        }

        public string UserId { get; set; }
        public string UserName { get; private set; }
        public IEnumerable<string> Claims { get; private set; }
    }
}