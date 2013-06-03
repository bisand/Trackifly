using System;
using System.Collections.Generic;
using System.Linq;
using Trackifly.Data.Models;

namespace Trackifly.Data.Storage
{
    public interface IDataStore
    {
        IQueryable<T> Query<T>();
        List<T> Query<T>(Func<T, bool> predicate);
        T Load<T>(string id) where T : MongoBase;
        void Save<T>(T entity);
        void Delete<T>(T entity) where T : MongoBase;
        void Delete<T>(string id);
    }
}