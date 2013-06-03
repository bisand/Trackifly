using MongoDB.Driver;
using Nancy;
using Nancy.Conventions;
using Trackifly.Data;
using Trackifly.Data.Storage;
using Trackifly.Server.Configuration;

namespace Trackifly.Server.Bootstrappers
{
    public class TrackiflyBootstrapper : DefaultNancyBootstrapper
    {
        private MongoClient _client;
        private MongoServer _server;
        private MongoDatabase _database;
        private MongoDataStore _dataStore;

        protected override void ConfigureConventions(NancyConventions conventions)
        {
            base.ConfigureConventions(conventions);

            conventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("css", @"Content/css"));
            conventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("js", @"Content/js"));
            conventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("images", @"Content/images"));
        }

        protected override void ApplicationStartup(Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);
        }

        protected override void ConfigureApplicationContainer(Nancy.TinyIoc.TinyIoCContainer container)
        {
            var connectionString = AppSettings.ConnectionString;
            _client = new MongoClient(connectionString);
            _server = _client.GetServer();
            _database = _server.GetDatabase(AppSettings.DatabaseName);
            _dataStore = new MongoDataStore(_database);

            container.Register(typeof(IDataStore), _dataStore);

            base.ConfigureApplicationContainer(container);
        }
        protected override void ConfigureRequestContainer(Nancy.TinyIoc.TinyIoCContainer container, NancyContext context)
        {
            container.Register(typeof(IDataStore), _dataStore);
            container.Register(typeof(Users));
            container.Register(typeof(TrackingSessions));

            base.ConfigureRequestContainer(container, context);
        }
    }
}