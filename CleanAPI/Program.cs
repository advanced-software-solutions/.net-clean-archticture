using CleanBase.CleanAbstractions.CleanBusiness;
using CleanBase.CleanAbstractions.CleanOperation;
using CleanBase.Entities;
using CleanBase.Validator;
using CleanBusiness;
using CleanOperation.ConfigProvider;
using CleanOperation.DataAccess;
using FluentValidation.AspNetCore;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Serilog;
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
            builder.Services.AddSerilog();
            // Add services to the container.
            builder.Services.AddDbContext<AppDataContext>(y =>
            {
                y.UseSqlServer(builder.Configuration["ConnectionString:DefaultConnection"]);
                //y.UseInMemoryDatabase("Main");
                y.EnableDetailedErrors();
                y.EnableSensitiveDataLogging();
                y.ConfigureWarnings(y => y.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            });
            builder.Host.ConfigureAppConfiguration((hostingContext, configBuilder) =>
            {
                var config = configBuilder.Build();
                var configSource = new CustomCleanConfigurationSource(opts =>
                    opts.UseSqlServer(builder.Configuration["ConnectionString:DefaultConnection"]));
                configBuilder.Add(configSource);
            });
            var defaultBatchHandler = new DefaultODataBatchHandler();
            defaultBatchHandler.MessageQuotas.MaxNestingDepth = 3;
            defaultBatchHandler.MessageQuotas.MaxOperationsPerChangeset = 10;
            defaultBatchHandler.MessageQuotas.MaxReceivedMessageSize = 1000;
            builder.Services.AddControllers()
                .AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)
                .AddFluentValidation(r =>
                {
                    r.RegisterValidatorsFromAssemblyContaining<TodoListValidation>(lifetime: ServiceLifetime.Scoped);
                    r.AutomaticValidationEnabled = false;
                    r.ImplicitlyValidateChildProperties = false;
                })
            .AddOData(opt => opt.Select().Filter().Expand().Count().SetMaxTop(10).EnableQueryFeatures()
            .AddRouteComponents("odata", GetEdmModel(), defaultBatchHandler));
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddFluentValidationRulesToSwagger();

            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<ITodoListService, TodoListService>();
            var app = builder.Build();
            app.UseSerilogRequestLogging();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            
            app.MapControllers();

            app.Run();
        }
        private static IEdmModel GetEdmModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<TodoItem>("TodoItem");
            builder.EntitySet<TodoList>("TodoList");
            return builder.GetEdmModel();
        }
    }
}