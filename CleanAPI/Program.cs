using Akka.Actor;
using Akka.Hosting;
using CleanAPI.Extensions;
using CleanAPI.Middlewares;
using CleanBase;
using CleanBase.Configurations;
using CleanBase.Validator;
using CleanBusiness.Actors;
using CleanOperation;
using CleanOperation.DataAccess;
using FastEndpoints;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Scalar.AspNetCore;
using Serilog;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

namespace CleanAPI;

public class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
        .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3} {CorrelationId}] {Message}{NewLine}{Exception}")
        .Enrich.FromLogContext()
        .CreateLogger();
        Log.Information("Application Starting");
        var builder = WebApplication.CreateBuilder(args);
        CleanAppConfiguration appConfig = new();
        builder.Configuration.GetSection("CleanAppConfiguration").Bind(appConfig);
        builder.Services.AddOptions<CleanAppConfiguration>().BindConfiguration(nameof(CleanAppConfiguration));
        if (appConfig is null)
        {
            throw new ArgumentNullException(
                "Configurations are missing. Please check your appsettings.json configuration.");
        }
        builder.Services.AddAuthentication()
        .AddJwtBearer(jwtOptions =>
        {
            jwtOptions.RequireHttpsMetadata = false;
            jwtOptions.Authority = appConfig.Auth.Authority;
            jwtOptions.Audience = appConfig.Auth.Audience;
            jwtOptions.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidAudiences = appConfig.Auth.ValidAudiences,
                ValidIssuers = appConfig.Auth.ValidIssuers,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appConfig.Auth.Key))
            };

            jwtOptions.MapInboundClaims = false;
        });

        builder.AddInMemoryCache(appConfig);
        builder.Services.AddOpenApi();
        builder.Services.AddSerilog();
        builder.Services.AddFastEndpoints().AddResponseCaching();
        builder.Services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
        });

        builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Fastest;
        });

        builder.Services.Configure<GzipCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.SmallestSize;
        });
        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        // Add services to the container.
        builder.AddDatastoreConnection(appConfig);

        var defaultBatchHandler = new DefaultODataBatchHandler();
        defaultBatchHandler.MessageQuotas.MaxNestingDepth = 3;
        defaultBatchHandler.MessageQuotas.MaxOperationsPerChangeset = 10;
        defaultBatchHandler.MessageQuotas.MaxReceivedMessageSize = 1000;
        builder.Services.AddFluentValidationClientsideAdapters();
        builder.Services.AddValidatorsFromAssemblyContaining<TodoListValidation>(); ;
        builder.Services.AddControllers()
            .AddJsonOptions(x =>
            {
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                x.JsonSerializerOptions.TypeInfoResolverChain.Add(AppJsonSerializerContext.Default);
            })
        .AddOData(opt => opt.Select().Filter().Expand().Count().SetMaxTop(10).EnableQueryFeatures()
        .AddRouteComponents("odata", GetEdmModel(), defaultBatchHandler));

        builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        builder.Services.AddAkka("clean-system", (akkaBuilder, provider) =>
        {
            akkaBuilder.WithActors((system, registry) =>
            {
                // Get all concrete types implementing ICleanActor
                var actorTypes = typeof(ICleanActor).Assembly.GetTypes()
                    .Where(t => typeof(ICleanActor).IsAssignableFrom(t) &&
                               !t.IsInterface &&
                               !t.IsAbstract && t.Name.EndsWith("Actor"));

                foreach (var type in actorTypes)
                {
                    var actorKey = type.FullName!;
                    var props = Props.Create(type);

                    // Create the actor instance first
                    var actorRef = system.ActorOf(props, actorKey);

                    // Register the actor reference directly
                    registry.TryRegister(type, actorRef);
                }
            });
        });


        var app = builder.Build();
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.ConfigureAppUse(appConfig);
        app.UseResponseCompression();
        app.UseResponseCaching()
            .UseFastEndpoints(y => y.Serializer.Options.ReferenceHandler = ReferenceHandler.IgnoreCycles);
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.Services.UpdateDatabase();
            app.MapOpenApi();
            app.MapScalarApiReference();
            app.UseSerilogRequestLogging();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
    private static IEdmModel GetEdmModel()
    {
        var assembly = typeof(EntityRoot).Assembly;
        var types = assembly.ExportedTypes
           // filter types that are unrelated
           .Where(x => x.IsClass && x.IsPublic && x.BaseType == typeof(CleanBase.EntityRoot));
        ODataConventionModelBuilder builder = new ODataConventionModelBuilder();

        foreach (var type in types)
        {
            var entityType = builder.AddEntityType(type);
            PropertyInfo key = new EntityRoot().GetType().GetProperty("Id");
            entityType.HasKey(key);
            builder.AddEntitySet(type.Name, entityType);
        }
        return builder.GetEdmModel();
    }
}