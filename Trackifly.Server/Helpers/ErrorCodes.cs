using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Nancy;

namespace Trackifly.Server.Helpers
{
    public class ErrorCodes : Dictionary<int, string>
    {
        private static readonly Dictionary<int, string> InternalStorage;

        static ErrorCodes()
        {
            InternalStorage = EnumToDictionary(typeof (HttpStatusCode));
        }

        private static Dictionary<int, string> EnumToDictionary(Type enumType)
        {
            return Enum.GetValues(enumType).Cast<int>().ToDictionary(e => e, e => Enum.GetName(enumType, e));
        }

        public new ICollection<int> Keys
        {
            get { return InternalStorage.Keys; }
        }

        public new ICollection<string> Values
        {
            get { return InternalStorage.Values; }
        }

        public new string this[int key]
        {
            get
            {
                string value;
                return InternalStorage.TryGetValue(key, out value) ? value : "Unknown";
            }
        }

        public new int Count
        {
            get { return InternalStorage.Count; }
        }

        public new IEnumerator GetEnumerator()
        {
            return ((IEnumerable) InternalStorage).GetEnumerator();
        }

        public bool Contains(object key)
        {
            return ((IDictionary) InternalStorage).Contains(key);
        }

        public new bool ContainsKey(int key)
        {
            return InternalStorage.ContainsKey(key);
        }

        public new bool ContainsValue(string value)
        {
            return InternalStorage.ContainsValue(value);
        }

        public new bool TryGetValue(int key, out string value)
        {
            return InternalStorage.TryGetValue(key, out value);
        }

        public bool Contains(int item)
        {
            return InternalStorage.ContainsKey(item);
        }

        public bool Contains(string item)
        {
            return InternalStorage.ContainsValue(item);
        }
    }
}