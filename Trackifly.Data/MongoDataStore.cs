using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
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
            var collection = GetCollection<T>();
            var value = GetPropertyValue("Id", entity);

            var argParam = Expression.Parameter(typeof(T), "x");
            var nameProperty = Expression.Property(argParam, "Id");
            var lambda = Expression.Lambda<Expression<Func<T, string>>>(nameProperty, argParam);
            var compile = lambda.Compile();

            var query = MongoDB.Driver.Builders.Query<T>.EQ(compile, value);
            var e = collection.Remove(query);
        }

        public void Delete<T>(string entityId) where T : MongoBase
        {
            var collection = GetCollection<T>();
            var query = MongoDB.Driver.Builders.Query<T>.EQ(x => x.Id, entityId);
            var e = collection.Remove(query);
        }

        private MongoCollection GetCollection<T>()
        {
            var collectionName = typeof (T).Name;
            var collection = _database.GetCollection<T>(collectionName);
            return collection;
        }

        private static string GetPropertyValue(string id, object entity)
        {
            if (entity == null)
                return null;
            var type = entity.GetType();
            var propertyInfo = type.GetProperty(id,
                                                BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            if (propertyInfo == null)
                return null;
            var value = propertyInfo.GetValue(entity);
            return value != null ? value.ToString() : null;
        }
    }
}