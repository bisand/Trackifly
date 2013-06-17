using System.IO;
using Nancy;
using Nancy.ErrorHandling;

namespace Trackifly.Server.Handlers
{
    public class PageNotFoundHandler : IStatusCodeHandler
    {
        public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context)
        {
            return statusCode == HttpStatusCode.NotFound && context.Response is NotFoundResponse;
        }

        public void Handle(HttpStatusCode statusCode, NancyContext context)
        {
            // Create empty result.
            context.Response.Contents = stream =>
                {
                    using (var sw = new StreamWriter(stream))
                    {
                        sw.Write("");
                        sw.Flush();
                    }
                };
        }
    }
}