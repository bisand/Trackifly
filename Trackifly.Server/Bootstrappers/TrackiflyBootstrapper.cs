using System;
using System.Linq;
using MongoDB.Driver;
using Nancy;
using Nancy.Authentication.Stateless;
using Nancy.Conventions;
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
        private static MongoDataStore _dataStore;
        //private MongoClient _client;
        //private MongoDataStore _dataStore;
        //private MongoDatabase _database;
        //private ErrorCodes _errorCodes;
        //private IRequestValidator _requestRetentionValidator;
        //private MongoServer _server;
        //private TinyIoCContainer _container;

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
            container.Register<RequestRetentionValidator>();

            pipelines.BeforeRequest.AddItemToEndOfPipeline(context => RetentionValidator(context, container));
            base.ApplicationStartup(container, pipelines);
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            var connectionString = AppSettings.ConnectionString;
            var client = new MongoClient(connectionString);
            var server = client.GetServer();
            var database = server.GetDatabase(AppSettings.DatabaseName);
            _dataStore = new MongoDataStore(database);

            container.Register<ErrorCodes>();

            base.ConfigureApplicationContainer(container);
        }

        protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
        {
            container.Register<IDataStore>(_dataStore);
            container.Register<TrackingUsers>();
            container.Register<TrackingSessions>();
            container.Register<TrackingGroups>();

            base.ConfigureRequestContainer(container, context);
        }

        protected override void RequestStartup(TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines, NancyContext context)
        {
            var statelessAuthConfiguration =
                new StatelessAuthenticationConfiguration(ctx =>
                {
                    if (string.IsNullOrWhiteSpace(ctx.Request.Headers["access-token"].FirstOrDefault()))
                        return null;

                    var accessToken = ctx.Request.Headers["access-token"].FirstOrDefault();
                    var canResolve = container.CanResolve<IDataStore>();
                    if (!canResolve)
                        return null;

                    var dataStore = container.Resolve<IDataStore>();
                    var trackingUser =
                        dataStore.Query<TrackingUser>()
                                 .FirstOrDefault(
                                     x => x.AccessToken.Token == accessToken && x.AccessToken.Expires > DateTime.Now);

                    if (trackingUser == null)
                        return null;

                    var test = new UserIdentity(trackingUser.Username, trackingUser.Claims);

                    return test;
                });

            StatelessAuthentication.Enable(pipelines, statelessAuthConfiguration);

            base.RequestStartup(container, pipelines, context);
        }

        private static Response RetentionValidator(NancyContext context, TinyIoCContainer container)
        {
            var validator = container.Resolve<RequestRetentionValidator>();
            return validator.Validate(context, container);
        }
    }
}