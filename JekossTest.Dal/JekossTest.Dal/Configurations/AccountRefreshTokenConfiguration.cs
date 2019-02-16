using JekossTest.Dal.Common;
using JekossTest.Dal.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JekossTest.Dal.Configurations
{
    public class AccountRefreshTokenConfiguration : DbEntityConfiguration<AccountRefreshToken>
    {
        public override void Configure(EntityTypeBuilder<AccountRefreshToken> entity)
        {
            entity.ToTable("AccountRefreshToken");
            entity.HasOne(x => x.User).WithOne(x => x.AccountRefreshToken).HasForeignKey<User>(x=>x.AccountRefreshTokenId);            
        }
    }
}