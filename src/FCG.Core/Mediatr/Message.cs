namespace FCG.Core.Mediatr
{
    public abstract class Message
    {
        public string MessageType { get; protected set; }
        public int AggregateId { get; internal set; }

        protected Message()
        {
            MessageType = GetType().Name;
        }
    }
}
