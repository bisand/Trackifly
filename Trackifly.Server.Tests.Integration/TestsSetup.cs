using MongoDB.Driver;
using NUnit.Framework;
using Trackifly.Data.Storage;
using Trackifly.Server.Configuration;
using Trackifly.Server.Helpers;
using Trackifly.Server.RequestValidators;

namespace Trackifly.Server.Tests.Integration
{
    [TestFixture]
    public class TestsSetup
    {
        [SetUp]
        public void Setup()
        {
            var connectionString = AppSettings.ConnectionString;

            var requestRetentionValidator = new RequestRetentionValidator();
            var client = new MongoClient(connectionString);
            var server = client.GetServer();
            var database = server.GetDatabase(AppSettings.DatabaseName);
            var dataStore = new MongoDataStore(database);
            var errorCodes = new ErrorCodes();

            _dependencies = new object[]
                {
                    requestRetentionValidator,
                    client,
                    server,
                    database,
                    dataStore,
                    errorCodes,
                };
        }

        [TearDown]
        public void Teardown()
        {
        }

        protected object[] _dependencies;
    }
}