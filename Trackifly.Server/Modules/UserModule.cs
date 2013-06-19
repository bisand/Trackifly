using System;
using System.Linq;
using System.Text;
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
    public class UserModule : BaseModule
    {
        private readonly PasswordManager _passwordManager;
        private readonly TrackingUsers _trackingUsers;

        public UserModule(IDataStore dataStore, TrackingUsers trackingUsers, ErrorCodes errorCodes,
                          PasswordManager passwordManager)
            : base("/user", dataStore, errorCodes)
        {
            _trackingUsers = trackingUsers;
            _passwordManager = passwordManager;

            Post["/"] = _ =>
                {
                    var model = this.Bind<UserModel>();
                    if (model == null)
                        return ErrorResponse(HttpStatusCode.BadRequest,
                                             "Unknown request. Please provide a username and password.");

                    var existingUser = _trackingUsers.Query().FirstOrDefault(x => x.Username == model.Username);
                    if (existingUser != null)
                        return ErrorResponse(HttpStatusCode.Conflict, "Username allready exists!");

                    var salt = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString());
                    var hashPassword = _passwordManager.HashPassword(model.Password, salt);
                    var user = new TrackingUser(model.Username, hashPassword, salt)
                        {
                            Name = model.Name,
                            Email = model.Email,
                        };

                    _trackingUsers.Add(user);

                    return Response.AsJson(user, HttpStatusCode.Created);
                };

            Delete["/{accessToken}/{id}"] = parameters =>
                {
                    string id = parameters.Id;
                    string accessToken = parameters.AccessToken;

                    var user = _trackingUsers.Get(id);
                    if (user == null)
                        return HttpStatusCode.NotFound;

                    _trackingUsers.Delete(id);
                    return HttpStatusCode.OK;
                };
        }
    }
}