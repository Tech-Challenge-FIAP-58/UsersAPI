namespace FCG.Core.Events;

public sealed record UserSnapshot(
    int Id,
    string Name,
    string Email,
    string Cpf,
    string Address,
    bool IsAdmin,
    DateTime CreatedAtUtc);

public sealed record RoleSnapshot(
    int Id,
    string Name,
    string? Description,
    DateTime CreatedAtUtc);
