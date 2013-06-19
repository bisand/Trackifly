using System;
using System.Collections.Generic;
using System.Linq;
using Nancy;
using Nancy.ModelBinding;
using Trackifly.Data;
using Trackifly.Data.Encryption;
using Trackifly.Data.Models;
using Trackifly.Data.Storage;
using Trackifly.Server.Helpers;
using Trackifly.Server.Models;

namespace Trackifly.Server.Modules
{
    public class LoginModule : BaseModule
    {
        private static readonly Dictionary<string, DateTime> SessionCache = new Dictionary<string, DateTime>();
        private readonly TrackingUsers _trackingUsers;
        private readonly PasswordManager _passwordManager;

        public LoginModule(IDataStore dataStore, TrackingUsers trackingUsers, ErrorCodes errorCodes, PasswordManager passwordManager)
            : base("/login", dataStore, trackingUsers, errorCodes)
        {
            _trackingUsers = trackingUsers;
            _passwordManager = passwordManager;

            Post["/"] = parameters =>
                {
                    Response response;
                    if (!CheckSaveRetention(SessionCache, out response))
                        return response;

                    var loginModel = this.Bind<LoginModel>();

                    var trackingUser = _trackingUsers.Query().FirstOrDefault(x => x.Username == loginModel.Username);
                    if (trackingUser == null)
                        return ErrorResponse(HttpStatusCode.Unauthorized, "Wrong username or password!");

                    var password = loginModel.Password;
                    var passwordSalt = trackingUser.Salt;
                    var passwordHash = trackingUser.Password;

                    var confirmPassword = _passwordManager.ConfirmPassword(password, passwordHash, passwordSalt);
                    return confirmPassword ? 
                        ErrorResponse(HttpStatusCode.OK, "Ok") : 
                        ErrorResponse(HttpStatusCode.Unauthorized, "Wrong username or password!");
                };
        }
    }
}