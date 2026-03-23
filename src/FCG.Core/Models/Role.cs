using FCG.Core.Events;
using FCG.Core.Mediatr;

namespace FCG.Core.Models
{

    public class Role : EntityBase
    {
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }

        public virtual ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();

        public static Role Create(string name, string? description)
        {
            var role = new Role
            {
                Name = name,
                Description = description
            };

            role.AddEvent(new RoleCreatedDomainEvent(role.ToSnapshot()));
            return role;
        }

        public void Update(string? name, string? description)
        {
            if (name is not null) Name = name;
            if (description is not null) Description = description;

            AddEvent(new RoleUpdatedDomainEvent(ToSnapshot()));
        }

        public void Delete()
        {
            AddEvent(new RoleDeletedDomainEvent(ToSnapshot()));
        }

        public override Event CreateDomainEvent(DomainEventAction action) => action switch
        {
            DomainEventAction.Created => new RoleCreatedDomainEvent(ToSnapshot()),
            DomainEventAction.Updated => new RoleUpdatedDomainEvent(ToSnapshot()),
            DomainEventAction.Deleted => new RoleDeletedDomainEvent(ToSnapshot()),
            _ => throw new ArgumentOutOfRangeException(nameof(action), action, null)
        };

        private RoleSnapshot ToSnapshot() => new(
            Id,
            Name,
            Description,
            CreatedAtUtc);
    }
}