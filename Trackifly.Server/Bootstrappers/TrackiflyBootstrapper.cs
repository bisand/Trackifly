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
                        var cnt = ctx.Items.Values.FirstOrDefault(x=>x is TinyIoCContainer) as TinyIoCContainer;
                        if (cnt == null)
                            return null;

                        if (string.IsNullOrWhiteSpace(ctx.Request.Headers["access-token"].FirstOrDefault()))
                            return null;

                        var accessToken = ctx.Request.Headers["access-token"].FirstOrDefault();
                        var canResolve = container.CanResolve<IDataStore>();
                        if (!canResolve)
                            return null;

                        var dataStore = cnt.Resolve<IDataStore>();
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

            container.Register<RequestRetentionValidator>();

            pipelines.BeforeRequest.AddItemToEndOfPipeline(context => RetentionValidator(context, container));
            base.ApplicationStartup(container, pipelines);
        }

        protected override void ConfigureApplicationContainer(Nancy.TinyIoc.TinyIoCContainer container)
        {
            var connectionString = AppSettings.ConnectionString;
            var client = new MongoClient(connectionString);
            var server = client.GetServer();
            var database = server.GetDatabase(AppSettings.DatabaseName);
            _dataStore = new MongoDataStore(database);

            container.Register<ErrorCodes>().AsSingleton();

            base.ConfigureApplicationContainer(container);
        }

        protected override void ConfigureRequestContainer(Nancy.TinyIoc.TinyIoCContainer container, NancyContext context)
        {
            container.Register<IDataStore>(_dataStore);
            container.Register<TrackingUsers>();
            container.Register<TrackingSessions>();
            container.Register<TrackingGroups>();

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