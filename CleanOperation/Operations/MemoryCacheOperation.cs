using CleanBase;
using CleanBase.Configurations;
using CleanBase.Extensions;
using Enyim.Caching;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;

namespace CleanOperation.Operations
{
    public class MemoryCacheOperation : IMemoryCacheOperation
    {
        private readonly CleanAppConfiguration appConfiguration;
        private readonly IServiceProvider serviceProvider;
        private readonly CachingProvider cachingProvider;

        public MemoryCacheOperation(IOptions<CleanAppConfiguration> configuration,
            IServiceProvider serviceProvider)
        {
            appConfiguration = configuration.Value;
            this.serviceProvider = serviceProvider;
            cachingProvider = appConfiguration.InMemoryCaching.Provider.ParseEnum<CachingProvider>();
            Log.Information("InMemory Cache Provider: {0}", cachingProvider);
        }
        public T? Get<T>(string key)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                switch (cachingProvider)
                {
                    case CachingProvider.Memcache:
                        var memcacheService = scope.ServiceProvider.GetService<IMemcachedClient>();
                        return memcacheService.Get<T?>(key);
                    case CachingProvider.InMemory:
                        var inMemory = scope.ServiceProvider.GetService<IMemoryCache>();
                        return inMemory.Get<T?>(key);
                }
                return default(T?);
            }
        }

        public void Set(string key, object data, int seconds = 30)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                switch (appConfiguration.InMemoryCaching.Provider.ParseEnum<CachingProvider>())
                {
                    case CachingProvider.Memcache:
                        var memcacheService = scope.ServiceProvider.GetService<IMemcachedClient>();
                        memcacheService.Add(key, data, seconds);
                        break;
                    case CachingProvider.InMemory:
                        var inMemory = scope.ServiceProvider.GetService<IMemoryCache>();
                        inMemory.Set(key, data);
                        break;
                }
            }
        }

    }
}
