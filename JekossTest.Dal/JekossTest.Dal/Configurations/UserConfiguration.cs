using JekossTest.Dal.Common;
using JekossTest.Dal.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JekossTest.Dal.Configurations
{
    public class UserConfiguration : DbEntityConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> entity)
        {
            entity.ToTable("User");
            entity.HasIndex(x => x.Id);
            entity.HasOne(x => x.Role).WithMany(x => x.Users).HasForeignKey(x => x.RoleId);
            entity.HasOne(x => x.AccountRefreshToken).WithOne(x => x.User).HasForeignKey<AccountRefreshToken>(x=>x.UserId);          
        }
    }
}