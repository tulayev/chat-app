namespace ChatApp.Domain.Abstractions
{
    // Marker interface: CreatedAt/UpdatedAt are configured as EF Core shadow
    // properties (see AuditableEntityConfigurationExtensions) instead of CLR
    // properties, so this interface has no members. It's still used by
    // ChatAppDbContext.UpdateTimestamps() to filter tracked entities by type.
    public interface IAuditableEntity
    {
    }
}
