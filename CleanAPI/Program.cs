using Akka.Actor;
using Akka.DependencyInjection;
using CleanBase;
using CleanBase.CleanAbstractions.CleanOperation;
using CleanBase.Validator;
using CleanBusiness.Actors;
using CleanOperation.ConfigProvider;
using CleanOperation.DataAccess;
using FastEndpoints;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Scalar.AspNetCore;
using Serilog;
using System.Reflection;
using System.Text.Json.Serialization;

namespace CleanAPI;

public class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
.WriteTo.Console()
.CreateLogger();

        var builder = WebApplication.CreateBuilder(args);
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        builder.Services.AddSerilog();
        //builder.Services.AddFastEndpoints();
        // Add services to the container.
        builder.Services.AddDbContext<AppDataContext>(y =>
        {
            var dbConnection = builder.Configuration["ConnectionStrings:DefaultConnection"];
            y.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]);
            //y.UseInMemoryDatabase("Main");
            y.EnableDetailedErrors();
            y.EnableSensitiveDataLogging();
            y.ConfigureWarnings(y => y.Ignore(InMemoryEventId.TransactionIgnoredWarning));
        });
#pragma warning disable ASP0013 // Suggest switching from using Configure methods to WebApplicationBuilder.Configuration
        builder.Host.ConfigureAppConfiguration((hostingContext, configBuilder) =>
        {
            var config = configBuilder.Build();
            var configSource = new CustomCleanConfigurationSource(opts =>
                opts.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]));
            configBuilder.Add(configSource);
        });
#pragma warning restore ASP0013 // Suggest switching from using Configure methods to WebApplicationBuilder.Configuration
        var defaultBatchHandler = new DefaultODataBatchHandler();
        defaultBatchHandler.MessageQuotas.MaxNestingDepth = 3;
        defaultBatchHandler.MessageQuotas.MaxOperationsPerChangeset = 10;
        defaultBatchHandler.MessageQuotas.MaxReceivedMessageSize = 1000;
        builder.Services.AddFluentValidationClientsideAdapters();
        builder.Services.AddValidatorsFromAssemblyContaining<TodoListValidation>(); ;
        builder.Services.AddControllers()
            .AddJsonOptions(x =>
            x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)
        .AddOData(opt => opt.Select().Filter().Expand().Count().SetMaxTop(10).EnableQueryFeatures()
        .AddRouteComponents("odata", GetEdmModel(), defaultBatchHandler));

        builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));


        builder.Services.AddSingleton(provider =>
        {
            var bootstrap = BootstrapSetup.Create();
            var di = DependencyResolverSetup.Create(provider);
            var actorSystemSetup = bootstrap.And(di);
            var system = ActorSystem.Create("CleanSystem", actorSystemSetup);
            return system;
        });

        // Register MarketDataProcessorActor with dependency injection
        builder.Services.AddSingleton(provider =>
        {
            var actorSystem = provider.GetRequiredService<ActorSystem>();
            var props = DependencyResolver.For(actorSystem).Props<TodoListActor>();
            return actorSystem.ActorOf(props, nameof(TodoListActor));
        });

        var app = builder.Build();

        //app.UseFastEndpoints();
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
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
        var assembly = typeof(EntityRoot).Assembly; // I actually use Assembly.LoadFile with well-known names 
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