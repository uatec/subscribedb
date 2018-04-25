using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SubscribeDb
{
    public class Change
    {
        public Change(string id, string value)
        {
            Id = id;
            Value = value;
        }

        public string Id { get; }
        public string Value { get; }
    }

    // TODO: Data Store IOC
    public static class Database
    {
        static BlockingCollection<Change> changes = new BlockingCollection<Change>();
        static Dictionary<string, string> data = new Dictionary<string, string>();

        public static string Get(string id)
        {
            return data[id];
        }

        public static void Put(string id, string value)
        {
            data[id] = value;
            changes.Add(new Change(id, value));
        }

        // TODO: Delete functionality

        public static IEnumerable<Change> Watch(string id)
        {
            while ( true )
            {
                var next = changes.Take();
                if ( next.Id == id ) 
                {
                    yield return next;
                }
            }
        }
    }
}
