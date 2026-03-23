namespace FCG.Core.Mediatr
{
    public abstract class Message
    {
        public string MessageType { get; protected set; }
        public Guid AggregateId { get; internal set; }

        protected Message()
        {
            MessageType = GetType().Name;
        }
    }
}
