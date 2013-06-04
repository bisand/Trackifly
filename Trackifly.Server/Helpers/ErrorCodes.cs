using System.Collections.Generic;

namespace Trackifly.Server.Helpers
{
    public class ErrorCodes : Dictionary<int, string>
    {
        private static readonly Dictionary<int, string> InternalStorage;
 
        static ErrorCodes()
        {
            InternalStorage = new Dictionary<int, string>
                {
                    {0, "OK"},
                    {10, "Too early to create a new session! Try again later."}
                };
        }

        public new void Add(int errorCode, string errorText)
        {
            InternalStorage.Add(errorCode, errorText);
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
    }
}