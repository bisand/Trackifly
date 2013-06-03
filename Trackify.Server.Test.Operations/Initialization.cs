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
using Trackifly.Data.Models;

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
            
            var user = new User {Name = name1, Email = "test@test.com"};

            dataStore.Save(user);
            
            var loadedUser = dataStore.Query<User>().FirstOrDefault(x => x.Name == name1);
            Assert.That(loadedUser != null && user.Name.Equals(loadedUser.Name));
            
            user.Name = name2;
            user.Email = "testing@test.com";
            dataStore.Save(user);
            
            loadedUser = dataStore.Query<User>().FirstOrDefault(x => x.Name == name1);
            Assert.That(loadedUser == null);
            
            loadedUser = dataStore.Query<User>().FirstOrDefault(x => x.Name == name2);
            Assert.That(loadedUser != null && user.Name.Equals(loadedUser.Name));

            dataStore.Delete(loadedUser);

            loadedUser = dataStore.Query<User>().FirstOrDefault(x => x.Name == name2);
            Assert.That(loadedUser == null);
        }
    }


    public class Entity
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }


}
