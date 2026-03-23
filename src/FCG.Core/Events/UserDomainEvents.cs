using FCG.Core.Mediatr;

namespace FCG.Core.Events;

public class UserCreatedDomainEvent : Event
{
    public UserSnapshot User { get; }

    public UserCreatedDomainEvent(UserSnapshot user)
    {
        User = user;
    }
}

public class UserUpdatedDomainEvent : Event
{
    public UserSnapshot User { get; }

    public UserUpdatedDomainEvent(UserSnapshot user)
    {
        User = user;
    }
}

public class UserDeletedDomainEvent : Event
{
    public UserSnapshot User { get; }

    public UserDeletedDomainEvent(UserSnapshot user)
    {
        User = user;
    }
}
