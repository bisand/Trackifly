using System;
using Nancy;
using Trackifly.Server.Models;

namespace Trackifly.Server.Modules
{
    public class RootModule : NancyModule
    {
        public RootModule()
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