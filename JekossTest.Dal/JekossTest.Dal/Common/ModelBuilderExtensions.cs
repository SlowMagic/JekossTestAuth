using Microsoft.EntityFrameworkCore;

namespace JekossTest.Dal.Common
{
    public static class ModelBuilderExtensions
    {
        public static void AddConfiguration<TEntity>(this ModelBuilder modelBuilder, DbEntityConfiguration<TEntity> entityConfiguration) where TEntity : class
        {
            modelBuilder.Entity<TEntity>(entityConfiguration.Configure);
        }
    }
}