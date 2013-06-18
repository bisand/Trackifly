using MongoDB.Driver;
using NUnit.Framework;
using Nancy;
using Nancy.Testing;
using Trackifly.Data.Storage;
using Trackifly.Server.Configuration;
using Trackifly.Server.Helpers;
using Trackifly.Server.Modules;
using Trackifly.Server.RequestValidators;

namespace Trackifly.Server.Tests.Unit
{
    [TestFixture]
    public class LoginTests
    {
        [SetUp]
        public void Setup()
        {
            _requestRetentionValidator = new RequestRetentionValidator();
            var connectionString = AppSettings.ConnectionString;
            _client = new MongoClient(connectionString);
            _server = _client.GetServer();
            _database = _server.GetDatabase(AppSettings.DatabaseName);
            _dataStore = new MongoDataStore(_database);
            _errorCodes = new ErrorCodes();
        }

        [TearDown]
        public void Teardown()
        {
        }

        private MongoClient _client;
        private MongoServer _server;
        private MongoDatabase _database;
        private MongoDataStore _dataStore;
        private ErrorCodes _errorCodes;
        private RequestRetentionValidator _requestRetentionValidator;

        [Test]
        public void Login_should_respond_with_Ok()
        {
            var bootstrapper = new ConfigurableBootstrapper(with => { with.Module<LoginModule>(); });
            var browser = new Browser(with =>
                {
                    with.Module<LoginModule>();
                    with.Dependency(_requestRetentionValidator);
                    with.Dependency(_client);
                    with.Dependency(_server);
                    with.Dependency(_database);
                    with.Dependency(_dataStore);
                    with.Dependency(_errorCodes);
                });

            var response = browser.Post("/login", with =>
                {
                    with.HttpRequest();
                    with.Body("{\"Username\":\"test\",\"Password\":\"test\"}");
                });

            Assert.That(response.StatusCode == HttpStatusCode.OK);
        }
    }
}