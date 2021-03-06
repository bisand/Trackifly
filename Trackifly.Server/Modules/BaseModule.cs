﻿using System;
using System.Collections.Generic;
using System.Linq;
using Nancy;
using Trackifly.Data;
using Trackifly.Data.Storage;
using Trackifly.Server.Configuration;
using Trackifly.Server.Helpers;
using Trackifly.Server.Models;

namespace Trackifly.Server.Modules
{
    public class BaseModule : NancyModule
    {
        private readonly IDataStore _dataStore;
        private readonly TrackingUsers _trackingUsers;

        public BaseModule(IDataStore dataStore, ErrorCodes errorCodes)
        {
            ErrorCodes = errorCodes;
            _dataStore = dataStore;
        }

        public BaseModule(string modulePath, IDataStore dataStore, TrackingUsers trackingUsers, ErrorCodes errorCodes)
            : base(modulePath)
        {
            ErrorCodes = errorCodes;
            _dataStore = dataStore;
            _trackingUsers = trackingUsers;
        }

        public ErrorCodes ErrorCodes { get; set; }

        protected bool CheckSaveRetention(Dictionary<string, DateTime> sessionCache, out Response response)
        {
            response = null;
            var ip = Request.UserHostAddress ?? "unknown";
            DateTime createdDate;
            if (sessionCache.TryGetValue(ip, out createdDate))
            {
                if (createdDate.AddSeconds(AppSettings.GlobalSaveRetention) > DateTime.Now)
                {
                    response = ErrorResponse(HttpStatusCode.TooManyRequests,
                                             string.Format(
                                                 "Please wait for at least {0} seconds before you create a new session.",
                                                 AppSettings.GlobalSaveRetention));
                    return false;
                }
            }
            sessionCache[ip] = DateTime.Now;
            return true;
        }

        protected bool CheckLoginRetention(Dictionary<string, DateTime> sessionCache, out Response response)
        {
            response = null;
            var ip = Request.UserHostAddress;
            DateTime createdDate;
            if (sessionCache.TryGetValue(ip, out createdDate))
            {
                if (createdDate.AddSeconds(AppSettings.GlobalLoginRetention) > DateTime.Now)
                {
                    response = ErrorResponse(HttpStatusCode.TooManyRequests,
                                             string.Format(
                                                 "Please wait for at least {0} seconds before you try a new login request.",
                                                 AppSettings.GlobalLoginRetention));
                    return false;
                }
            }
            sessionCache[ip] = DateTime.Now;
            return true;
        }

        protected bool IsAccessTokenValid(string accessToken)
        {
            var users =
                _trackingUsers.Query(
                    x =>
                    x.AccessToken != null && x.AccessToken.Token == accessToken && x.AccessToken.Expires > DateTime.Now)
                              .FirstOrDefault();
            return users != null;
        }

        protected Response ErrorResponse(HttpStatusCode httpStatusCode, string customErrorMessage = null)
        {
            var statusCode = (int) httpStatusCode;
            return Response.AsJson(new BasicResponseModel
                {
                    Error = statusCode,
                    Description = customErrorMessage ?? ErrorCodes[statusCode]
                }, httpStatusCode);
        }
    }
}