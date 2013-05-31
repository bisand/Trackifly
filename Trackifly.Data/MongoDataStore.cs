using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Driver.Builders;
using Trackifly.Data.Models;

namespace Trackifly.Data
{
    public class MongoDataStore : IDataStore
    {
        private readonly MongoDatabase _database;

        public MongoDataStore(MongoDatabase database)
        {
            _database = database;
        }

        public IQueryable<T> Query<T>()
        {
            var collection = GetCollection<T>();
            var result = collection.AsQueryable<T>();
            return result;
        }

        public List<T> Query<T>(Func<T, bool> predicate)
        {
            var collection = GetCollection<T>();
            var result = collection.AsQueryable<T>().Where(predicate).ToList();
            return result;
        }

        public T Load<T>(string id) where T : MongoBase
        {
            var collection = GetCollection<T>();
            var entity = collection.AsQueryable<T>().FirstOrDefault(x => x.Id == id);
            return entity;
        }

        public void Save<T>(T entity)
        {
            var collection = GetCollection<T>();
            collection.Save(entity);
        }

        public void Delete<T>(T entity)
        {
        }

        public void Delete(string entityId)
        {
        }

        private MongoCollection GetCollection<T>()
        {
            var collectionName = typeof (T).Name;
            var collection = _database.GetCollection<T>(collectionName);
            return collection;
        }
    }
}