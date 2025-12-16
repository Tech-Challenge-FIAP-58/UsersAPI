using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using FIAP.FGC.USER.Core.Models;


namespace FIAP.FGC.USER.Infra.Configuration
{
    public class RoleMapping : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles");
            builder.HasIndex(r => r.Name).IsUnique();
        }
    }
}
