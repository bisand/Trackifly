using System;
using System.Collections.Generic;
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

            Get["/{page?}/{take?}"] = parameters =>
                {
                    int page;
                    int take;
                    page = int.TryParse(parameters.Page, out page) ? page : 0;
                    take = int.TryParse(parameters.Take, out take) ? take : 10;

                    var currentUser = Context.CurrentUser as UserIdentity;
                    if (currentUser == null)
                        return HttpStatusCode.Unauthorized;

                    this.RequiresClaims(new[] {"Admin"});

                    var users = _trackingUsers.Query().Skip(page*take).Take(take).ToList();

                    return currentUser.Claims.All(x => x != "Admin")
                               ? HttpStatusCode.Unauthorized
                               : Response.AsJson(users);
                };

            Get["/{userId}"] = parameters =>
                {
                    var currentUser = Context.CurrentUser as UserIdentity;
                    if (currentUser == null || currentUser.AccessToken == null ||
                        string.IsNullOrWhiteSpace(currentUser.AccessToken.Token))
                        return HttpStatusCode.Unauthorized;

                    string userId = parameters.UserId;

                    var accessToken = currentUser.AccessToken;

                    this.RequiresClaims(new[] {"Admin"});

                    var user = _trackingUsers.Get(userId);
                    if (currentUser.Claims.All(x => x != "Admin"))
                    {
                        if (user.AccessToken == null || user.AccessToken.Token != accessToken.Token)
                            return HttpStatusCode.Unauthorized;
                    }

                    return user == null
                               ? (dynamic) new BasicResponseModel((int) HttpStatusCode.NotFound, "User not found!")
                               : Response.AsJson(user);
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

            Delete["/{id}"] = parameters =>
                {
                    string id = parameters.Id;
                    var currentUser = Context.CurrentUser as UserIdentity;
                    if (currentUser == null)
                        return HttpStatusCode.Unauthorized;

                    var accessToken = currentUser.AccessToken;

                    this.RequiresClaims(new[] {"Admin"});

                    var user = _trackingUsers.Get(id);
                    if (currentUser.Claims.All(x => x != "Admin"))
                    {
                        if (user.AccessToken == null || user.AccessToken.Token != accessToken.Token)
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