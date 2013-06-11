using System;
using Nancy;
using Trackifly.Data;
using Trackifly.Data.Storage;
using Trackifly.Server.Helpers;
using Trackifly.Server.Models;

namespace Trackifly.Server.Modules
{
    public class RootModule : BaseModule
    {
        public RootModule(IDataStore dataStore, ErrorCodes errorCodes)
            : base(dataStore, errorCodes)
        {
            Get["/"] = parameters =>
            {
                var model = new RootModel
                {
                    Username = "Unknown",
                    CurrentDate = DateTime.Now
                };
                return View["Views/index.html", model];
            };
        }
    }
}