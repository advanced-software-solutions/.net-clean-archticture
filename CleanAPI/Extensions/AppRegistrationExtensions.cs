using CleanBase;
using CleanBase.Configurations;
using CleanBase.Dtos;
using CleanBase.Entities;
using CleanBase.Extensions;
using CleanOperation.DataAccess;
using CleanOperation.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Serilog;

namespace CleanAPI.Extensions;

public static class AppRegistrationExtensions
{
    public static void AddInMemoryCache(this WebApplicationBuilder builder,
        CleanAppConfiguration appConfiguration)
    {
        Log.Information("Application InMemory Enabled => {0}", appConfiguration.InMemoryCaching.Enabled);
        builder.Services.AddScoped<IMemoryCacheOperation, MemoryCacheOperation>();
        if (appConfiguration.InMemoryCaching.Enabled)
        {
            switch (appConfiguration.InMemoryCaching.Provider.ParseEnum<CachingProvider>())
            {
                case CachingProvider.Memcache:
                    builder.Services.AddEnyimMemcached(y => y.Servers
                    .Add(new Enyim.Caching.Configuration.Server
                    {
                        Address = appConfiguration.InMemoryCaching.Configs.Host,
                        Port = int.Parse(appConfiguration.InMemoryCaching.Configs.Port)
                    }));
                    break;
                case CachingProvider.InMemory:
                    builder.Services.AddMemoryCache();
                    break;
                default:
                    Log.Error("The provided caching provider is not supported, caching will be disabled");
                    break;
            }
            EntityResult<TodoItem> test = new EntityResult<TodoItem> { Data = new TodoItem() };
        }
    }

    public static void AddDatastoreConnection(this WebApplicationBuilder builder,
        CleanAppConfiguration appConfiguration)
    {
        builder.Services.AddDbContext<AppDataContext>(y =>
        {
            Enum.TryParse(appConfiguration.Datastore.Provider, true, out DatastoreType datastoreType);
            switch (datastoreType)
            {
                case DatastoreType.InMemory:
                    y.UseInMemoryDatabase("Main");
                    break;
                case DatastoreType.SqlServer:
                    y.UseSqlServer(appConfiguration.Datastore.ConnectionString,
                        o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
                    break;
                case DatastoreType.PostgreSql:
                    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
                    y.UseNpgsql(appConfiguration.Datastore.ConnectionString);
                    break;
                case DatastoreType.SQLite:
                    var folder = Environment.SpecialFolder.LocalApplicationData;
                    var path = Environment.GetFolderPath(folder);
                    string dbpath = System.IO.Path.Join(path, "clean.db");
                    y.UseSqlite($"Data Source={dbpath}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Provider not supported");
            }
#if DEBUG
            y.EnableDetailedErrors();
            y.EnableSensitiveDataLogging();
#endif
            y.ConfigureWarnings(y => y.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            y.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
#if DEBUG
            y.UseSeeding((dbContext, _) =>
            {
                if (dbContext.Set<UserRole>().Count() == 0)
                {
                    dbContext.Set<UserRole>().Add(new UserRole { Id = Constants.AdminRoleId, Name = "admin" });
                }
                if (dbContext.Set<UserAccount>().Count() == 0)
                {
                    dbContext.Set<UserAccount>().Add(
                        new UserAccount { Email = "admin", Password = "123", UserRoleId = Constants.AdminRoleId });
                }
                dbContext.SaveChanges();
            });
#endif
        });
    }

    public static IServiceProvider UpdateDatabase(this IServiceProvider provider)
    {
        using (var scope = provider.GetRequiredService<IServiceScopeFactory>().CreateScope())
        using (var context = scope.ServiceProvider.GetService<AppDataContext>())
            context.Database.EnsureCreated();

        return provider;
    }
    public static void ConfigureAppUse(this WebApplication app,
        CleanAppConfiguration appConfiguration)
    {
        if (appConfiguration.InMemoryCaching.Enabled)
        {
            switch (appConfiguration.InMemoryCaching.Provider.ParseEnum<CachingProvider>())
            {
                case CachingProvider.Memcache:
                    app.UseEnyimMemcached();
                    break;
                case CachingProvider.InMemory:
                    break;
            }
        }
    }
}