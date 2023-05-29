using CleanBase.CleanAbstractions.CleanBusiness;
using CleanBase.CleanAbstractions.CleanOperation;
using CleanBusiness;
using CleanOperation.ConfigProvider;
using CleanOperation.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CleanAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<AppDataContext>(y =>
            {
                //y.UseSqlServer("");
                y.UseInMemoryDatabase("Main");
                y.ConfigureWarnings(y => y.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            });
            builder.Host.ConfigureAppConfiguration((hostingContext, configBuilder) =>
            {
                var config = configBuilder.Build();
                var configSource = new CustomCleanConfigurationSource(opts =>
                    opts.UseInMemoryDatabase("Main"));
                configBuilder.Add(configSource);
            });
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
          
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<ITodoListService, TodoListService>();
            var app = builder.Build();

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