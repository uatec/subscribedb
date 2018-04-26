using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SubscribeDb
{
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
            ChangeType changeType = ChangeType.Create;

            if ( data.ContainsKey(id) )
            {
                changeType = ChangeType.Update;
            }

            data[id] = value;
            changes.Add(new Change(changeType, id, value));
        }

        public static string Delete(string id)
        {
            var value = data[id];
            data.Remove(id);
            changes.Add(new Change(ChangeType.Delete, id, value));            
            return value;
        }

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
