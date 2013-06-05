using System;
using System.Collections.Generic;
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

        public ErrorCodes ErrorCodes { get; set; }

        public BaseModule(IDataStore dataStore, ErrorCodes errorCodes)
        {
            ErrorCodes = errorCodes;
            _dataStore = dataStore;
        }

        protected bool CheckSaveRetention(Dictionary<string, DateTime> sessionCache, out Response response)
        {
            response = null;
            var ip = Request.UserHostAddress;
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

        protected Response ErrorResponse(HttpStatusCode httpStatusCode, string customErrorMessage = null)
        {
            var statusCode = (int)httpStatusCode;
            return Response.AsJson(new ErrorModel
            {
                Error = statusCode,
                Description = customErrorMessage ?? ErrorCodes[statusCode]
            }, httpStatusCode);
        }
    }
}