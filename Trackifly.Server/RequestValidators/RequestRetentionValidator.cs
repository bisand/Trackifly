using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Nancy;
using Nancy.Responses;
using Nancy.TinyIoc;
using Trackifly.Server.Configuration;
using Trackifly.Server.Helpers;
using Trackifly.Server.Models;

namespace Trackifly.Server.RequestValidators
{
    public class RequestRetentionValidator : IRequestValidator
    {
        private static readonly ErrorCodes ErrorCodes;
        private static readonly ConcurrentDictionary<string, DateTime> SessionCache;

        static RequestRetentionValidator()
        {
            ErrorCodes = new ErrorCodes();
            SessionCache = new ConcurrentDictionary<string, DateTime>();
        }

        public Response Validate(NancyContext nancyContext, TinyIoCContainer container)
        {
            var method = nancyContext.Request.Method;
            Response response;
            switch (method)
            {
                case "POST":
                    if (CheckRetention(nancyContext, 60/AppSettings.MaxSaveRequestsPerMinute, out response))
                        return response;
                    break;
                case "PUT":
                    if (CheckRetention(nancyContext, 60/AppSettings.MaxSaveRequestsPerMinute, out response))
                        return response;
                    break;
                case "DELETE":
                    if (CheckRetention(nancyContext, 60/AppSettings.MaxSaveRequestsPerMinute, out response))
                        return response;
                    break;
                default:
                    if (CheckRetention(nancyContext, 60/AppSettings.MaxLoadRequestsPerMinute, out response))
                        return response;
                    break;
            }
            return null;
        }

        private bool CheckRetention(NancyContext nancyContext, int retentionSeconds, out Response response)
        {
            response = null;
            var ip = nancyContext.Request.UserHostAddress;
            DateTime createdDate;
            if (SessionCache.TryGetValue(ip, out createdDate))
            {
                if (createdDate.AddSeconds(retentionSeconds) > DateTime.Now)
                {
                    response = ErrorResponse(HttpStatusCode.TooManyRequests, nancyContext,
                                             string.Format(
                                                 "Please wait for at least {0} seconds before you create a new session.",
                                                 AppSettings.GlobalSaveRetention));
                    return false;
                }
            }
            SessionCache[ip] = DateTime.Now;
            return true;
        }

        private static Response ErrorResponse(HttpStatusCode httpStatusCode, NancyContext nancyContext, string customErrorMessage = null)
        {
            var statusCode = (int) httpStatusCode;
            var response = new JsonResponse<BasicResponseModel>(new BasicResponseModel
                {
                    Error = statusCode, Description = customErrorMessage ?? ErrorCodes[statusCode]
                }, new DefaultJsonSerializer());
            return response;
        }



    }
}