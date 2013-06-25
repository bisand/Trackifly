using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Trackifly.Data.Encryption;
using Trackifly.Data.Models;
using Trackifly.Data.Storage;

namespace Trackifly.Data
{
    public class TrackingUsers
    {
        private readonly IDataStore _dataStore;
        private readonly PasswordManager _passwordManager;

        public TrackingUsers(IDataStore dataStore, PasswordManager passwordManager)
        {
            _dataStore = dataStore;
            _passwordManager = passwordManager;
        }

        /// <summary>
        /// Get user with the provided id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TrackingUser Get(string id)
        {
            var user = _dataStore.Load<TrackingUser>(id);
            return user;
        }

        /// <summary>
        /// Get the users matching the provided predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public List<TrackingUser> Query(Func<TrackingUser, bool> predicate)
        {
            var users = _dataStore.Query(predicate);
            return users;
        }

        /// <summary>
        /// Get an IQueryable of users.
        /// </summary>
        /// <returns></returns>
        public IQueryable<TrackingUser> Query()
        {
            var users = _dataStore.Query<TrackingUser>();
            return users;
        }

        /// <summary>
        /// Add a user to the database, providing at least an e-mail address. Name is mandatory.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <param name="email"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public TrackingUser Add(string username, byte[] password, byte[] salt, string email = null, string name = null)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("The user must contain at least a username, password and salt.");

            var user = new TrackingUser(username, password, salt)
            {
                Email = email,
                Name = name,
            };
            _dataStore.Save(user);
            return user;
        }


        public TrackingUser Add(string username, string password, string email = null, string name = null)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("The user must contain at least a username, password and salt.");

            var salt = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString());
            var hashPassword = _passwordManager.HashPassword(password, salt);

            var user = new TrackingUser(username, hashPassword, salt)
            {
                Email = email,
                Name = name,
            };
            _dataStore.Save(user);
            return user;
        }

        /// <summary>
        /// Add a user to the database, providing at least an e-mail address. Name is mandatory.
        /// </summary>
        /// <param name="user"></param>
        public void Add(TrackingUser user)
        {
            if (user == null || (string.IsNullOrWhiteSpace(user.Username) && string.IsNullOrWhiteSpace(user.Email)))
                throw new ArgumentException("The user must contain a username or at least a valid e-mail address");
            if (string.IsNullOrWhiteSpace(user.Username))
                user.Username = user.Email;
            _dataStore.Save(user);
        }

        /// <summary>
        /// Update the database with the provided user entity.
        /// </summary>
        /// <param name="user"></param>
        public void Update(TrackingUser user)
        {
            _dataStore.Save(user);
        }

        /// <summary>
        /// Delete the provided entity from the database.
        /// </summary>
        /// <param name="user"></param>
        public void Delete(TrackingUser user)
        {
            _dataStore.Delete(user);
        }

        /// <summary>
        /// Delete the user with the provided id from the database.
        /// </summary>
        /// <param name="id"></param>
        public void Delete(string id)
        {
            _dataStore.Delete<TrackingUser>(id);
        }
    }
}
