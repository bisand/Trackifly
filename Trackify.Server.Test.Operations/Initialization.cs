using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using NUnit.Framework;
using Trackifly.Data;
using Trackifly.Data.Encryption;
using Trackifly.Data.Models;
using Trackifly.Data.Models.Enums;
using Trackifly.Data.Storage;

namespace Trackify.Server.Test.Operations
{
    [TestFixture]
    [Explicit("Run manually")]
    public class Initialization
    {
        [Test]
        public void ProofOfConcept()
        {
            const string connectionString = "mongodb://localhost";
            var client = new MongoClient(connectionString);
            var server = client.GetServer();
            var database = server.GetDatabase("test");
            var collection = database.GetCollection<Entity>("entities");

            var entity = new Entity {Id = Guid.NewGuid().ToString(), Name = "Tom"};
            collection.Insert(entity);
            var id = entity.Id;

            var query = Query<Entity>.EQ(e => e.Id, id);
            entity = collection.FindOne(query);

            entity.Name = "Dick";
            collection.Save(entity);

            var update = Update<Entity>.Set(e => e.Name, "Harry");
            collection.Update(query, update);

            var entities = collection.AsQueryable().Where(x => x.Name == "Harry").ToList();

            collection.Remove(query);
        }

        [Test]
        public void TestSaveLoadDelete()
        {
            const string connectionString = "mongodb://localhost";
            var client = new MongoClient(connectionString);
            var server = client.GetServer();
            var database = server.GetDatabase("test");
            var dataStore = new MongoDataStore(database);

            var name1 = Guid.NewGuid().ToString();
            var name2 = Guid.NewGuid().ToString();
            
            var user = new TrackingUser {Name = name1, Email = "test@test.com"};

            dataStore.Save(user);
            
            var loadedUser = dataStore.Query<TrackingUser>().FirstOrDefault(x => x.Name == name1);
            Assert.That(loadedUser != null && user.Name.Equals(loadedUser.Name));
            
            user.Name = name2;
            user.Email = "testing@test.com";
            dataStore.Save(user);
            
            loadedUser = dataStore.Query<TrackingUser>().FirstOrDefault(x => x.Name == name1);
            Assert.That(loadedUser == null);
            
            loadedUser = dataStore.Query<TrackingUser>().FirstOrDefault(x => x.Name == name2);
            Assert.That(loadedUser != null && user.Name.Equals(loadedUser.Name));

            dataStore.Delete<TrackingUser>(loadedUser.Id);

            loadedUser = dataStore.Query<TrackingUser>().FirstOrDefault(x => x.Name == name2);
            Assert.That(loadedUser == null);
        }

        [Test]
        public void TestTrackiflyUsers()
        {
            // Use IOC instead...
            const string connectionString = "mongodb://localhost";
            var client = new MongoClient(connectionString);
            var server = client.GetServer();
            var database = server.GetDatabase("test");
            var dataStore = new MongoDataStore(database);
            var j = 1;
            var users = new TrackingUsers(dataStore, new PasswordManager());
            for (int i = 0; i < 100; i++)
            {
                users.Add(string.Format("test{0}", j++), "test", "andre@biseth.net");
                users.Add(string.Format("test{0}", j++), "test", "andre.biseth@paretosec.com");
                users.Add(string.Format("test{0}", j++), "test", "andre@biseth.com");
            }
        }

        [Test]
        public void TestTrackingSessions()
        {
            // Use IOC instead...
            const string connectionString = "mongodb://localhost";
            var client = new MongoClient(connectionString);
            var server = client.GetServer();
            var database = server.GetDatabase("test");
            var dataStore = new MongoDataStore(database);

            var sessions = new TrackingSessions(dataStore);
            var session1 = sessions.Add();
            for (int i = 0; i < 100; i++)
            {
                session1.AddPosition(new TrackingPosition((decimal) (0.01*i), (decimal) (0.02*i)));
                sessions.Update(session1);
            }
            var session2 = sessions.Add(TrackingType.Full);
            for (int i = 0; i < 100; i++)
            {
                session2.AddPosition(new TrackingPosition((decimal)(0.01 * i), (decimal)(0.02 * i)));
                sessions.Update(session2);
            }
        }
    }


    public class Entity
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }


}
