using Nancy.Json;
using Trackifly.Data.Models;

namespace Trackifly.Server.Models
{
    public class UserModel
    {
        public UserModel(TrackingUser user)
        {
            Username = user.Username;
        }

        public UserModel()
        {
        }

        public string Username { get; set; }
        [ScriptIgnore]
        public string Password { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}