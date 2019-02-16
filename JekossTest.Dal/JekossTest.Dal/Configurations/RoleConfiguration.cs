using JekossTest.Dal.Common;
using JekossTest.Dal.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JekossTest.Dal.Configurations
{
    public class RoleConfiguration : DbEntityConfiguration<Role>
    {
        public override void Configure(EntityTypeBuilder<Role> entity)
        {
            entity.ToTable("Role");
            entity.HasIndex(x => x.Id);
            entity.HasMany(x => x.Users).WithOne(x => x.Role).HasForeignKey(x => x.RoleId);
        }
    }
}