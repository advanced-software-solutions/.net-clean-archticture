using CleanBase;
using CleanBase.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanOperation.DataAccess
{
    public class AppDataContext : DbContext
    {
        public AppDataContext(DbContextOptions<AppDataContext> options) : base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Configuration>().ToTable("Configurations", "Main");
            modelBuilder.Entity<ConfigurationItem>().ToTable("ConfigurationsItems", "Main");
            EntityPropertyMapper(modelBuilder);
        }
        private void EntityPropertyMapper(ModelBuilder modelBuilder)
        {

            IEnumerable<string> entityProperties = typeof(EntityRoot).GetProperties().Select(propInfo => propInfo.Name);

            IEnumerable<IMutableEntityType> mappedEntities = modelBuilder.Model.GetEntityTypes().Where(y => y.ClrType.BaseType.Name.Contains(nameof(EntityRoot))).ToList();

            foreach (IMutableEntityType entity in mappedEntities)
            {
                EntityTypeBuilder entityTypeBuilder = modelBuilder.Entity(entity.ClrType);
                entityTypeBuilder.Property(nameof(EntityRoot.Id)).ValueGeneratedOnAdd();
                entityTypeBuilder.Property(nameof(EntityRoot.Rowversion)).IsConcurrencyToken();
            }
        }
    }
}
