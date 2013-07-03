using System;
using System.Linq;
using System.Text;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Security;
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
            : base("/user", dataStore, trackingUsers, errorCodes)
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
                        return ErrorResponse(HttpStatusCode.Conflict, "Username already exists!");

                    existingUser = _trackingUsers.Query().FirstOrDefault(x => x.Email == model.Email);
                    if (existingUser != null)
                        return ErrorResponse(HttpStatusCode.Conflict, "E-mail is already registered!");

                    var salt = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString());
                    var hashPassword = _passwordManager.HashPassword(model.Password, salt);
                    var user = new TrackingUser(model.Username, hashPassword, salt)
                        {
                            Name = model.Name,
                            Email = model.Email,
                        };

                    _trackingUsers.Add(user);

                    return Response.AsJson(new UserModel(user), HttpStatusCode.Created);
                };

            Get["/availability/{username}"] = parameters =>
                {
                    string username = parameters.Username;
                    var user = _trackingUsers.Query().FirstOrDefault(x => x.Username == username);
                    var response = user == null
                                       ? new BasicResponseModel(0, "")
                                       : new BasicResponseModel(0, "User already exists!");

                    return Response.AsJson(response);
                };

            Get["/availability/email/{email}"] = parameters =>
                {
                    string email = parameters.Email;
                    var user = _trackingUsers.Query().FirstOrDefault(x => x.Email == email);
                    var response = user == null
                                       ? new BasicResponseModel(0, "")
                                       : new BasicResponseModel(0, "Email is already registered!");

                    return Response.AsJson(response);
                };

            Delete["/{id}"] = parameters =>
                {
                    string id = parameters.Id;
                    var currentUser = Context.CurrentUser as UserIdentity;
                    if (currentUser == null)
                        return HttpStatusCode.Unauthorized;

                    var accessToken = currentUser.AccessToken;

                    this.RequiresClaims(new[] {"Admin"});

                    var user = _trackingUsers.Get(id);
                    if (user != null && user.Claims.All(x => x != "Admin"))
                    {
                        if (user.AccessToken != null && user.AccessToken.Token != accessToken.Token)
                            return HttpStatusCode.Unauthorized;
                    }

                    if (user == null)
                        return HttpStatusCode.NotFound;

                    _trackingUsers.Delete(id);
                    
                    return HttpStatusCode.OK;
                };
        }
    }
}