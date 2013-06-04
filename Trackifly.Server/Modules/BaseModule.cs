using Nancy;
using Trackifly.Data;
using Trackifly.Data.Storage;
using Trackifly.Server.Helpers;

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
    }
}