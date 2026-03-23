using System;
using System.Linq;
using FCG.Core.Events;
using FCG.Core.Models;
using Xunit;

namespace FCG.Test;

public class RoleTests
{
    [Fact]
    public void Create_ShouldSetValues_AndAddCreatedEvent()
    {
        var role = Role.Create("Admin", "System administrator");

        Assert.Equal("Admin", role.Name);
        Assert.Equal("System administrator", role.Description);
        Assert.NotNull(role.Notifications);
        Assert.Contains(role.Notifications!, e => e is RoleCreatedDomainEvent);
    }

    [Fact]
    public void Update_ShouldApplyOnlyProvidedValues_AndAddUpdatedEvent()
    {
        var role = Role.Create("Admin", "System administrator");
        role.ClearEvents();

        role.Update("Manager", null);

        Assert.Equal("Manager", role.Name);
        Assert.Equal("System administrator", role.Description);
        Assert.NotNull(role.Notifications);
        Assert.Contains(role.Notifications!, e => e is RoleUpdatedDomainEvent);
    }

    [Fact]
    public void Delete_ShouldMarkAsDeleted_AndAddDeletedEvent()
    {
        var role = Role.Create("Admin", "System administrator");
        role.ClearEvents();

        role.Delete();

        Assert.True(role.IsDeleted);
        Assert.NotNull(role.DeletedAt);
        Assert.NotNull(role.Notifications);
        Assert.Contains(role.Notifications!, e => e is RoleDeletedDomainEvent);
    }
}
