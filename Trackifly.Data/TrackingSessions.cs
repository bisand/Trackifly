using System;
using System.Collections.Generic;
using System.Linq;
using Trackifly.Data.Models;
using Trackifly.Data.Models.Enums;
using Trackifly.Data.Storage;

namespace Trackifly.Data
{
    public class TrackingSessions
    {
        private readonly IDataStore _dataStore;

        public TrackingSessions(IDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        /// <summary>
        /// Get tracking session with the provided id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TrackingSession Get(string id)
        {
            var session = _dataStore.Load<TrackingSession>(id);
            return session;
        }

        /// <summary>
        /// Get the tracking sessions matching the provided predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public List<TrackingSession> Query(Func<TrackingSession, bool> predicate)
        {
            var sessions = _dataStore.Query(predicate);
            return sessions;
        }

        /// <summary>
        /// Get an IQueryable of tracking sessions.
        /// </summary>
        /// <returns></returns>
        public IQueryable<TrackingSession> Query()
        {
            var sessions = _dataStore.Query<TrackingSession>();
            return sessions;
        }

        /// <summary>
        /// Add a tracking session to the database.
        /// </summary>
        /// <returns></returns>
        public TrackingSession Add()
        {
            var session = new TrackingSession();
            _dataStore.Save(session);
            return session;
        }

        /// <summary>
        /// Add a tracking session to the database.
        /// </summary>
        /// <returns></returns>
        public TrackingSession Add(TrackingType trackingType, string displayName = null)
        {
            var session = new TrackingSession();
            session.TrackingType = trackingType;
            session.DisplayName = displayName;
            _dataStore.Save(session);
            return session;
        }

        /// <summary>
        /// Add a tracking session to the database.
        /// </summary>
        /// <param name="trackingSession"></param>
        public void Add(TrackingSession trackingSession)
        {
            if (trackingSession == null)
                throw new ArgumentException("The tracking session cannot be null");
            _dataStore.Save(trackingSession);
        }

        /// <summary>
        /// Update the database with the provided tracking session entity.
        /// </summary>
        /// <param name="trackingSession"></param>
        public void Update(TrackingSession trackingSession)
        {
            _dataStore.Save(trackingSession);
        }

        /// <summary>
        /// Delete the provided tracking session from the database.
        /// </summary>
        /// <param name="trackingSession"></param>
        public void Delete(TrackingSession trackingSession)
        {
            _dataStore.Delete(trackingSession);
        }

        /// <summary>
        /// Delete the tracking session with the provided id from the database.
        /// </summary>
        /// <param name="id"></param>
        public void Delete(string id)
        {
            _dataStore.Delete<TrackingSession>(id);
        }
    }
}