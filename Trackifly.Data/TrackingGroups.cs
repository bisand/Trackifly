using System;
using System.Collections.Generic;
using System.Linq;
using Trackifly.Data.Models;
using Trackifly.Data.Models.Enums;
using Trackifly.Data.Storage;

namespace Trackifly.Data
{
    public class TrackingGroups
    {
        private readonly IDataStore _dataStore;

        /// <summary>
        /// Get a group with the provided Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TrackingGroup Get(string id)
        {
            var result = _dataStore.Load<TrackingGroup>(id);
            return result;
        }

        /// <summary>
        /// Get tracking groups from the provided query predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public List<TrackingGroup> Query(Func<TrackingGroup, bool> predicate)
        {
            var result = _dataStore.Query(predicate);
            return result;
        }

        /// <summary>
        /// Get an IQueryable of tracking groups.
        /// </summary>
        /// <returns></returns>
        public IQueryable<TrackingGroup> Query()
        {
            var result = _dataStore.Query<TrackingGroup>();
            return result;
        }

        /// <summary>
        /// Add a new group to the database and return the result.
        /// </summary>
        /// <returns></returns>
        public TrackingGroup Add()
        {
            var result = new TrackingGroup();
            _dataStore.Save(result);
            return result;
        }

        /// <summary>
        /// Add a new group and with the provided display name and return the result. 
        /// </summary>
        /// <param name="displayName"></param>
        /// <returns></returns>
        public TrackingGroup Add(string displayName)
        {
            var result = new TrackingGroup();
            result.DisplayName = displayName;
            _dataStore.Save(result);
            return result;
        }

        /// <summary>
        /// Add the provided tracking group to the database.
        /// </summary>
        /// <param name="trackingGroup"></param>
        public void Add(TrackingGroup trackingGroup)
        {
            if (trackingGroup == null)
                throw new ArgumentException("The tracking group cannot be null");
            _dataStore.Save(trackingGroup);
        }

        /// <summary>
        /// Update the database with the provided tracking group.
        /// </summary>
        /// <param name="trackingGroup"></param>
        public void Update(TrackingGroup trackingGroup)
        {
            _dataStore.Save(trackingGroup);
        }

        /// <summary>
        /// Delete the provided tracking group from the database.
        /// </summary>
        /// <param name="trackingGroup"></param>
        public void Delete(TrackingGroup trackingGroup)
        {
            _dataStore.Delete(trackingGroup);
        }

        /// <summary>
        /// Delete a tracking group from the database with the provided Id.
        /// </summary>
        /// <param name="id"></param>
        public void Delete(string id)
        {
            _dataStore.Delete<TrackingGroup>(id);
        }
    }
}