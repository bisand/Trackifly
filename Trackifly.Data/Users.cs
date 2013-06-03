using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Trackifly.Data.Models;
using Trackifly.Data.Storage;

namespace Trackifly.Data
{
    public class Users
    {
        private readonly IDataStore _dataStore;

        public Users(IDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        /// <summary>
        /// Get user with the provided id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public User Get(string id)
        {
            var user = _dataStore.Load<User>(id);
            return user;
        }

        /// <summary>
        /// Get the users matching the provided predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public List<User> Query(Func<User, bool> predicate)
        {
            var users = _dataStore.Query(predicate);
            return users;
        }

        /// <summary>
        /// Add a user to the database, providing at least an e-mail address. Name is mandatory.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public User Add(string email, string name = null)
        {
            if(string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("The user must contain at least a valid e-mail address");

            var user = new User(email, name);
            _dataStore.Save(user);
            return user;
        }

        /// <summary>
        /// Add a user to the database, providing at least an e-mail address. Name is mandatory.
        /// </summary>
        /// <param name="user"></param>
        public void Add(User user)
        {
            if(user == null || string.IsNullOrWhiteSpace(user.Email))
                throw new ArgumentException("The user must contain at least a valid e-mail address");
            _dataStore.Save(user);
        }

        /// <summary>
        /// Update the database with the provided user entity.
        /// </summary>
        /// <param name="user"></param>
        public void Update(User user)
        {
            _dataStore.Save(user);
        }

        /// <summary>
        /// Delete the provided entity from the database.
        /// </summary>
        /// <param name="user"></param>
        public void Delete(User user)
        {
            _dataStore.Delete(user);
        }

        /// <summary>
        /// Delete the user with the provided id from the database.
        /// </summary>
        /// <param name="id"></param>
        public void Delete(string id)
        {
            _dataStore.Delete<User>(id);
        }
    }
}
