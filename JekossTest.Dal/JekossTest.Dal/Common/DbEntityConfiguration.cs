using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JekossTest.Dal.Common
{
    public abstract class DbEntityConfiguration<TEntity> where TEntity : class 
    {
        public abstract void Configure(EntityTypeBuilder<TEntity> entity);
    }
}