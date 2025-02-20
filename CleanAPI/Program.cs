using Akka.Actor;
using CleanBase;
using CleanBase.CleanAbstractions.CleanBusiness;
using CleanBase.CleanAbstractions.CleanOperation;
using CleanBase.Entities;
using CleanBase.Validator;
using CleanBusiness;
using CleanOperation.ConfigProvider;
using CleanOperation.DataAccess;
using FluentValidation;
using FluentValidation.AspNetCore;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
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

namespace CleanAPI
{
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
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddFluentValidationRulesToSwagger();

            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            var assembly = typeof(EntityRoot).Assembly; // I actually use Assembly.LoadFile with well-known names 
            var types = assembly.ExportedTypes
               // filter types that are unrelated
               .Where(x => x.IsClass && x.IsPublic && x.BaseType == typeof(CleanBase.EntityRoot));

            foreach (var type in types)
            {
                // assume that we want to inject any class that implements an interface
                // whose name is the type's name prefixed with I
                var serviceName = type.Name + "Service";
                var businessAssembly = typeof(RootService<>).Assembly;
                var service = businessAssembly.ExportedTypes.FirstOrDefault(y => y.Name == serviceName);
                if (service != null)
                {
                    builder.Services.AddScoped(businessAssembly.ExportedTypes.FirstOrDefault(y => y.Name == $"I{type.Name}Service") , service);
                }
            }
            //builder.Services.AddScoped<ITodoListService, TodoListService>();
            var actorSystem = ActorSystem.Create("CleanSystem");
            // Register Akka.NET services
            builder.Services.AddSingleton(actorSystem);
            var app = builder.Build();
            app.UseSerilogRequestLogging();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
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
}