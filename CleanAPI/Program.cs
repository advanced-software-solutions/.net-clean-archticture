using CleanBase.CleanAbstractions.CleanBusiness;
using CleanBase.CleanAbstractions.CleanOperation;
using CleanBusiness;
using CleanOperation.ConfigProvider;
using CleanOperation.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Serilog;

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
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
          
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
    }
}