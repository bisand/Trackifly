using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using NUnit.Framework;

namespace Trackify.Server.Test.Operations
{
    [TestFixture]
    [Explicit("Run manually")]
    public class Initialization
    {
        [Test]
        public void CreateDatabase()
        {
            const string connectionString = "mongodb://localhost";
            var client = new MongoClient(connectionString);
            var server = client.GetServer();
            var database = server.GetDatabase("test");
            
        }
    }
}
