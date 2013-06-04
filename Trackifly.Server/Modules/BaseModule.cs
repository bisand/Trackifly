using Nancy;
using Trackifly.Data;
using Trackifly.Data.Storage;
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