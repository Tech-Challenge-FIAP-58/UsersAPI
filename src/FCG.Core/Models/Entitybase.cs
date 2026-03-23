using FCG.Core.Mediatr;

namespace FCG.Core.Models
{
    public abstract class EntityBase
    {
        public Guid Id { get; set; }
        public bool IsDeleted { get; protected set; } = false;
        public DateTime CreatedAt { get; protected set; }
        public DateTime? UpdatedAt { get; protected set; }
        public DateTime? DeletedAt { get; protected set; }

        protected EntityBase()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
        }

        private List<Event> _notifications;

        public IReadOnlyCollection<Event> Notifications => _notifications?.AsReadOnly();

        public void AddEvent(Event domainEvent)
        {
            _notifications ??= new List<Event>();
            domainEvent.AggregateId = this.Id;

            var existingIndex = _notifications.FindIndex(e => e.GetType() == domainEvent.GetType());

            if (existingIndex >= 0)
            {
                _notifications[existingIndex] = domainEvent;
                return;
            }

            _notifications.Add(domainEvent);
        }

        public void RemoveEvent(Event domainEvent)
        {
            _notifications?.Remove(domainEvent);
        }

        public void ClearEvents()
        {
            _notifications?.Clear();
        }

        public abstract Event CreateDomainEvent(DomainEventAction action);

        #region Comparisons
        public override bool Equals(object obj)
        {
            var compareTo = obj as EntityBase;

            if (ReferenceEquals(this, compareTo)) return true;
            if (ReferenceEquals(null, compareTo)) return false;

            return Id.Equals(compareTo.Id);
        }

        public override string ToString()
        {
            return $"{GetType().Name} [Id={Id}]";
        }

        public static bool operator ==(EntityBase a, EntityBase b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(EntityBase a, EntityBase b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return GetType().GetHashCode() * 907 + Id.GetHashCode();
        }
        #endregion
    }

}