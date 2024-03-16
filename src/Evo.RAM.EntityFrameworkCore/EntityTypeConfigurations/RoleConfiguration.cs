using Evo.RAM.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Evo.RAM.EntityTypeConfigurations
{
    internal class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable(RAMConsts.DbTablePrefix + "Roles", RAMConsts.DbSchema);
            builder.HasKey(e => e.Id);
            builder.Configure();
            builder.Property(e => e.Code).HasMaxLength(12);
            builder.Property(e => e.ConcurrencyStamp).HasMaxLength(40);
            builder.Property(e => e.CreationTime).HasColumnType("timestamp without time zone");
            builder.Property(e => e.CreatorName).HasMaxLength(24);
            builder.Property(e => e.DeleterName).HasMaxLength(24);
            builder.Property(e => e.DeletionTime).HasColumnType("timestamp without time zone");
            builder.Property(e => e.Description).HasMaxLength(100);
            builder.Property(e => e.ExtraProperties).HasColumnType("jsonb");
            builder.Property(e => e.IsEnabled).HasDefaultValueSql("true");
            builder.Property(e => e.LastModificationTime).HasColumnType("timestamp without time zone");
            builder.Property(e => e.LastModifierName).HasMaxLength(24);
            builder.Property(e => e.Name).HasMaxLength(24);
            builder.Property(e => e.Tags).HasColumnType("jsonb");
        }
    }
}
