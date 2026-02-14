using Bogus;
using CleanBase.Entities;
using NBomber.CSharp;
using System.Net.Http.Json;
namespace CleanConsole.PerformanceTests;

internal class Program
{
    static async Task Main(string[] args)
    {
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri("https://localhost:7219/api/");
        var tokenRequest = await httpClient.PostAsJsonAsync("UserAccount/Login", new { email = "admin", password = "123" });
        var token = await tokenRequest.Content.ReadAsStringAsync();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        var scenarioSqlServer1k = Scenario.Create("sqlserver_scenario_insert1000", async context =>
        {
            Faker<TodoList> faker = new Faker<TodoList>();
            faker.Rules((f, item) =>
            {
                item.DueDate = f.Date.Between(DateTime.Now.AddYears(-20), DateTime.Now);
                item.Title = f.Random.Words(3);
                item.TodoItems = new List<TodoItem>()
                {
                    new TodoItem()
                    {
                        Title = f.Random.Words(3)
                    },
                    new TodoItem()
                    {
                        Title = f.Random.Words(3)
                    },
                    new TodoItem()
                    {
                        Title = f.Random.Words(3)
                    },
                    new TodoItem()
                    {
                        Title = f.Random.Words(3)
                    },
                    new TodoItem()
                    {
                        Title = f.Random.Words(3)
                    }
                };
            });
            var response = await httpClient.PostAsJsonAsync("TodoList/CreateList", faker.Generate(1000));
            var z = response.Content.ReadAsStringAsync();
            return Response.Ok();
        })
            .WithoutWarmUp()
            .WithLoadSimulations(Simulation.IterationsForInject(1,TimeSpan.FromSeconds(5),10));
        var scenarioSqlServer = Scenario.Create("sqlserver_scenario_insert10,000", async context =>
        {
            Faker<TodoList> faker = new Faker<TodoList>();
            faker.Rules((f, item) =>
            {
                item.DueDate = f.Date.Between(DateTime.Now.AddYears(-20), DateTime.Now);
                item.Title = f.Random.Words(3);
                item.TodoItems = new List<TodoItem>()
                {
                    new TodoItem()
                    {
                        Title = f.Random.Words(3)
                    },
                    new TodoItem()
                    {
                        Title = f.Random.Words(3)
                    },
                    new TodoItem()
                    {
                        Title = f.Random.Words(3)
                    },
                    new TodoItem()
                    {
                        Title = f.Random.Words(3)
                    },
                    new TodoItem()
                    {
                        Title = f.Random.Words(3)
                    }
                };
            });
            var response = await httpClient.PostAsJsonAsync("TodoList/CreateList", faker.Generate(10000));
            var z = response.Content.ReadAsStringAsync();
            return Response.Ok();
        })
            .WithoutWarmUp()
            .WithLoadSimulations(Simulation.IterationsForInject(1,TimeSpan.FromSeconds(5),1));
        
        var scenarioSqlServerSingle = Scenario.Create("sqlserver_scenario_insert1", async context =>
        {
            Faker<TodoList> faker = new Faker<TodoList>();
            faker.Rules((f, item) =>
            {
                item.DueDate = f.Date.Between(DateTime.Now.AddYears(-20), DateTime.Now);
                item.Title = f.Random.Words(3);
                item.TodoItems = new List<TodoItem>()
                {
                    new TodoItem()
                    {
                        Title = f.Random.Words(3)
                    },
                    new TodoItem()
                    {
                        Title = f.Random.Words(3)
                    },
                    new TodoItem()
                    {
                        Title = f.Random.Words(3)
                    },
                    new TodoItem()
                    {
                        Title = f.Random.Words(3)
                    },
                    new TodoItem()
                    {
                        Title = f.Random.Words(3)
                    }
                };
            });
            var response = await httpClient.PostAsJsonAsync("TodoList", faker.Generate(1).First());
            var z = response.Content.ReadAsStringAsync();
            return Response.Ok();
        })
            .WithoutWarmUp()
            .WithLoadSimulations(Simulation.IterationsForInject(1,TimeSpan.FromSeconds(1),10));

        var scenarioPostgres = Scenario.Create("postgres_scenario_insert1000", async context =>
        {
            Faker<TodoList> faker = new Faker<TodoList>();
            faker.Rules((f, item) =>
            {
                item.DueDate = f.Date.Between(DateTime.Now.AddYears(-20), DateTime.Now);
                item.Title = f.Random.Words(3);
                item.TodoItems = new List<TodoItem>()
                {
                    new TodoItem()
                    {
                        Title = f.Random.Words(3)
                    },
                    new TodoItem()
                    {
                        Title = f.Random.Words(3)
                    },
                    new TodoItem()
                    {
                        Title = f.Random.Words(3)
                    },
                    new TodoItem()
                    {
                        Title = f.Random.Words(3)
                    },
                    new TodoItem()
                    {
                        Title = f.Random.Words(3)
                    }
                };
            });
            var response = await httpClient.PostAsJsonAsync("TodoList/CreateList", faker.Generate(1000));
            var z = response.Content.ReadAsStringAsync();
            return Response.Ok();
        })
            .WithoutWarmUp()
            .WithLoadSimulations(Simulation.IterationsForInject(1,TimeSpan.FromSeconds(1),10));
        
        var scenarioPostgresSingle = Scenario.Create("postgres_scenario_insert1", async context =>
        {
            Faker<TodoList> faker = new Faker<TodoList>();
            faker.Rules((f, item) =>
            {
                item.DueDate = f.Date.Between(DateTime.Now.AddYears(-20), DateTime.Now);
                item.Title = f.Random.Words(3);
                item.TodoItems = new List<TodoItem>()
                {
                    new TodoItem()
                    {
                        Title = f.Random.Words(3)
                    },
                    new TodoItem()
                    {
                        Title = f.Random.Words(3)
                    },
                    new TodoItem()
                    {
                        Title = f.Random.Words(3)
                    },
                    new TodoItem()
                    {
                        Title = f.Random.Words(3)
                    },
                    new TodoItem()
                    {
                        Title = f.Random.Words(3)
                    }
                };
            });
            var response = await httpClient.PostAsJsonAsync("TodoList", faker.Generate(1).First());
            var z = response.Content.ReadAsStringAsync();
            return Response.Ok();
        })
            .WithoutWarmUp()
            .WithLoadSimulations(Simulation.IterationsForInject(1,TimeSpan.FromSeconds(1),10));

        NBomberRunner
            .RegisterScenarios([scenarioSqlServer])
            .Run();
    }
}
