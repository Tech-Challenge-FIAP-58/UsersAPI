using FCG.Core.Events;
using FCG.Core.Mediatr;

namespace FCG.Core.Models
{
    public class User : EntityBase
    {
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }
        public string Cpf { get; private set; }
        public string Address { get; private set; }
        public bool IsAdmin { get; private set; }

        private readonly List<UserRole> _items = [];

        public IReadOnlyCollection<UserRole> UserRoles => _items;

        public override Event CreateDomainEvent(DomainEventAction action) => action switch
        {
            DomainEventAction.Created => new UserCreatedDomainEvent(ToSnapshot()),
            DomainEventAction.Updated => new UserUpdatedDomainEvent(ToSnapshot()),
            DomainEventAction.Deleted => new UserDeletedDomainEvent(ToSnapshot()),
            _ => throw new ArgumentOutOfRangeException(nameof(action), action, null)
        };

        public void Delete()
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
            AddEvent(new UserDeletedDomainEvent(ToSnapshot()));
        }

        public void Update(string? name, string? email, string? address, string? password)
        {
            if (name is not null) Name = name;
            if (email is not null) Email = email;
            if (address is not null) Address = address;
            if (password is not null) Password = password;

            AddEvent(new UserUpdatedDomainEvent(ToSnapshot()));
        }

        private UserSnapshot ToSnapshot() => new(
            Id,
            Name,
            Email,
            Cpf,
            Address,
            IsAdmin,
            CreatedAt,
            UpdatedAt,
            DeletedAt,
            IsDeleted);
    }
}
