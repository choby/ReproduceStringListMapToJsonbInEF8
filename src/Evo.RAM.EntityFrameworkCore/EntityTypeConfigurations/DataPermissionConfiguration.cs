using Evo.RAM.DataPermissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Evo.RAM.EntityTypeConfigurations
{
    internal class DataPermissionConfiguration : IEntityTypeConfiguration<DataPermission>
    {
        public void Configure(EntityTypeBuilder<DataPermission> builder)
        {
            builder.ToTable(RAMConsts.DbTablePrefix + "DataPermissions", RAMConsts.DbSchema);
            builder.HasKey(e => e.Id);
            builder.Configure();
            builder.Property(e => e.Name).HasMaxLength(64);
            builder.Property(e => e.Code).HasMaxLength(64);
            builder.Property(e => e.Brands)
                .HasComment("绑定的品牌 ，有序")
                .HasColumnType("jsonb");
            builder.Property(e => e.ConcurrencyStamp).HasMaxLength(40);
            builder.Property(e => e.CreationTime).HasColumnType("timestamp without time zone");
            builder.Property(e => e.CreatorName).HasMaxLength(24);
            builder.Property(e => e.DeleterName).HasMaxLength(24);
            builder.Property(e => e.DeletionTime).HasColumnType("timestamp without time zone");
            builder.Property(e => e.ExtraProperties).HasColumnType("jsonb");
            builder.Property(e => e.IncludeAllUsers).HasComment("加载所有用户");
            builder.Property(e => e.IsEnabled).HasComment("停用启用");
            builder.Property(e => e.LastModificationTime).HasColumnType("timestamp without time zone");
            builder.Property(e => e.LastModifierName).HasMaxLength(24);
            builder.Property(e => e.Users)
                .HasComment("绑定的用户，有序")
                .HasColumnType("jsonb");
            builder.Property(e => e.Tags).HasColumnType("jsonb");
            
            builder.HasMany(e => e.Children).WithOne().HasForeignKey(p => p.ParentId);
        }
    }
}
