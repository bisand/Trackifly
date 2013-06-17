using System;
using System.Linq;
using MongoDB.Driver;
using Nancy;
using Nancy.Authentication.Stateless;
using Nancy.Conventions;
using Nancy.Security;
using Nancy.TinyIoc;
using Trackifly.Data;
using Trackifly.Data.Models;
using Trackifly.Data.Storage;
using Trackifly.Server.Configuration;
using Trackifly.Server.Helpers;
using Trackifly.Server.Models;
using Trackifly.Server.RequestValidators;

namespace Trackifly.Server.Bootstrappers
{
    public class TrackiflyBootstrapper : DefaultNancyBootstrapper
    {
        private MongoClient _client;
        private MongoDataStore _dataStore;
        private MongoDatabase _database;
        private ErrorCodes _errorCodes;
        private IRequestValidator _requestRetentionValidator;
        private MongoServer _server;

        protected override void ConfigureConventions(NancyConventions conventions)
        {
            base.ConfigureConventions(conventions);

            conventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("css", @"Content/css"));
            conventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("js", @"Content/js"));
            conventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("images",
                                                                                                  @"Content/images"));
        }

        protected override void ApplicationStartup(TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
        {
            var statelessAuthConfiguration =
                new StatelessAuthenticationConfiguration(ctx =>
                    {
                        if (!ctx.Request.Cookies.ContainsKey("AccessToken") || string.IsNullOrWhiteSpace(ctx.Request.Cookies["AccessToken"]))
                            return null;

                        var accessToken = ctx.Request.Cookies["AccessToken"];
                        var dataStore = container.Resolve<IDataStore>();
                        var trackingUser = dataStore.Query<TrackingUser>().FirstOrDefault(x => x.AccessToken.Token == accessToken && x.AccessToken.Expires > DateTime.Now);

                        if (trackingUser == null)
                            return null;

                        var test = new UserIdentity(trackingUser.Username, trackingUser.Claims);

                        return test;
                    });

            StatelessAuthentication.Enable(pipelines, statelessAuthConfiguration);

            _requestRetentionValidator = new RequestRetentionValidator();
            pipelines.BeforeRequest.AddItemToEndOfPipeline(context => RetentionValidator(context, container));
            base.ApplicationStartup(container, pipelines);
        }

        protected override void ConfigureApplicationContainer(Nancy.TinyIoc.TinyIoCContainer container)
        {
            var connectionString = AppSettings.ConnectionString;
            _client = new MongoClient(connectionString);
            _server = _client.GetServer();
            _database = _server.GetDatabase(AppSettings.DatabaseName);
            _dataStore = new MongoDataStore(_database);
            _errorCodes = new ErrorCodes();

            container.Register(typeof (IDataStore), _dataStore);
            container.Register(typeof (RequestRetentionValidator), _requestRetentionValidator);

            base.ConfigureApplicationContainer(container);
        }

        protected override void ConfigureRequestContainer(Nancy.TinyIoc.TinyIoCContainer container, NancyContext context)
        {
            container.Register(typeof (IDataStore), _dataStore);
            container.Register(typeof (TrackingUsers));
            container.Register(typeof (TrackingSessions));
            container.Register(typeof (TrackingGroups));
            container.Register(typeof (ErrorCodes), _errorCodes);

            base.ConfigureRequestContainer(container, context);
        }

        protected override void RequestStartup(Nancy.TinyIoc.TinyIoCContainer container,
                                               Nancy.Bootstrapper.IPipelines pipelines, NancyContext context)
        {
            base.RequestStartup(container, pipelines, context);
        }

        private static Response RetentionValidator(NancyContext context, TinyIoCContainer container)
        {
            var validator = container.Resolve<RequestRetentionValidator>();
            return validator.Validate(context, container);
        }
    }
}