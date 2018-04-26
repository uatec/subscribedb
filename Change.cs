namespace SubscribeDb
{
    public class Change
    {
        public Change(ChangeType changeType, string id, string value)
        {
            ChangeType = changeType;
            Id = id;
            Value = value;
        }

        public ChangeType ChangeType { get; }
        public string Id { get; }
        public string Value { get; }
    }
}
