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
            modelBuilder.Entity<CleanConfiguration>().ToTable("CleanConfigurations", "Core");
            modelBuilder.Entity<CleanConfigurationItem>().ToTable("CleanConfigurationItems", "Core");
            modelBuilder.Entity<TodoItem>().ToTable("TodoItems", "Core");
            modelBuilder.Entity<TodoList>().ToTable("TodoLists", "Core");
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
                entityTypeBuilder.Property(nameof(EntityRoot.Rowversion)).ValueGeneratedOnAddOrUpdate();
            }
        }
    }
}
