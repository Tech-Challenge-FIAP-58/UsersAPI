using FCG.Core.Mediatr;

namespace FCG.Core.Events;

public class RoleCreatedDomainEvent : Event
{
    public RoleSnapshot Role { get; }

    public RoleCreatedDomainEvent(RoleSnapshot role)
    {
        Role = role;
    }
}

public class RoleUpdatedDomainEvent : Event
{
    public RoleSnapshot Role { get; }

    public RoleUpdatedDomainEvent(RoleSnapshot role)
    {
        Role = role;
    }
}

public class RoleDeletedDomainEvent : Event
{
    public RoleSnapshot Role { get; }

    public RoleDeletedDomainEvent(RoleSnapshot role)
    {
        Role = role;
    }
}
