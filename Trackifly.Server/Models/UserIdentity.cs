using System.Collections.Generic;
using Nancy.Security;
using Trackifly.Data.Models;

namespace Trackifly.Server.Models
{
    public class UserIdentity : IUserIdentity
    {
        public UserIdentity(string username, IEnumerable<string> claims, AccessToken accessToken)
        {
            UserName = username;
            Claims = claims;
            AccessToken = accessToken;
        }

        public UserIdentity()
        {
        }

        public string UserId { get; set; }
        public AccessToken AccessToken { get; set; }
        public string UserName { get; private set; }
        public IEnumerable<string> Claims { get; private set; }
    }
}