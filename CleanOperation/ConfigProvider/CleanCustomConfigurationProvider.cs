using CleanBase.Entities;
using CleanOperation.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CleanOperation.ConfigProvider
{
    public class CleanCustomConfigurationProvider : ConfigurationProvider, IDisposable
    {
        private readonly IConfigurationSource _source;
        private readonly IDisposable _changeTokenRegistration;
        Action<DbContextOptionsBuilder> DbContextOptions { get; }
        public CleanCustomConfigurationProvider(Action<DbContextOptionsBuilder> optionsAction, IConfigurationSource configurationSource)
        {
            DbContextOptions = optionsAction;
            _source = configurationSource;
            ConfigChangeObserver.Instance.Changed += EntityChangeObserverChanged;
        }

        private void EntityChangeObserverChanged(object? sender, ConfigChangeEventArgs e)
        {
            Load();
        }
        public override void Load()
        {
            var builder = new DbContextOptionsBuilder<AppDataContext>();
            DbContextOptions(builder);
            using (var dbContext = new AppDataContext(builder.Options))
            {
                SeedConfiguration seedConfiguration = new(dbContext);
                seedConfiguration.Seed();
                Dictionary<string, string> configs = new Dictionary<string, string>();
                var Configurations = dbContext.Set<CleanConfiguration>().Include(p => p.ConfigurationItems).ToList();
                foreach (var configuration in Configurations)
                {
                    foreach (var ConfigurationItem in configuration.ConfigurationItems)
                    {
                        configs.Add($"{configuration.Name}:{ConfigurationItem.Key}", ConfigurationItem.Value);
                    }
                }

                Data = configs;
            }
        }
        public void Dispose()
        {
            _changeTokenRegistration?.Dispose();
        }
    }
}
