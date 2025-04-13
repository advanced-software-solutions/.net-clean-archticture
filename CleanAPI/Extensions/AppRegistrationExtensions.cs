using CleanBase;
using CleanBase.Configurations;
using CleanOperation.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CleanAPI.Extensions;

public static class AppRegistrationExtensions
{
    public static void AddInMemoryCache(this WebApplicationBuilder builder,
        CleanAppConfiguration appConfiguration)
    {
        if (appConfiguration.InMemoryCaching.Enabled)
        {
            builder.Services.AddEnyimMemcached();
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
                    break;
                case DatastoreType.SQLite:
                    var folder = Environment.SpecialFolder.LocalApplicationData;
                    var path = Environment.GetFolderPath(folder);
                    string dbpath = System.IO.Path.Join(path, "blogging.db");
                    y.UseSqlite($"Data Source={dbpath}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Provider not supported");
            }
            
            
            y.EnableDetailedErrors();
            y.EnableSensitiveDataLogging();
            y.ConfigureWarnings(y => y.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            y.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });
    }
    
    public static IServiceProvider UpdateDatabase(this IServiceProvider provider)
    {
        using (var scope = provider.GetRequiredService<IServiceScopeFactory>().CreateScope())
        using (var context = scope.ServiceProvider.GetService<AppDataContext>())
            context.Database.EnsureCreated();

        return provider;
    }
}