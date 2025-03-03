using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

[Generator]
public class ServiceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<INamedTypeSymbol> entities =
               context.CompilationProvider.SelectMany((compilation, _) =>
               {
                   // Resolve EntityRoot from the correct namespace
                   var entityRoot = compilation.GetTypeByMetadataName("CleanBase.EntityRoot");
                   if (entityRoot == null) return Enumerable.Empty<INamedTypeSymbol>();

                   return GetAllTypes(compilation.GlobalNamespace)
                       .Where(t => IsValidEntity(t, compilation.Assembly, entityRoot));
               });

        context.RegisterSourceOutput(entities, (spc, source) =>
        {

            var sourceText = $$"""

            using Akka.Actor;
            using CleanBase.CleanAbstractions.CleanOperation;
            using CleanBase.Dtos;
            using CleanBase.Entities;
            using CleanOperation.Abstractions;
            using Microsoft.Extensions.DependencyInjection;

            namespace CleanBusiness.Actors
            {
                public partial class {{source.Name}}Actor : ReceiveActor, ICleanActor
                {
                    public {{source.Name}}Actor(IServiceProvider serviceProvider)
                    {
                        Receive<EntityCommand<{{source.Name}}, Guid>>(async msg =>
                        {
                            using (var scope = serviceProvider.CreateScope())
                            {
                                var repo = scope.ServiceProvider.GetRequiredService<IRepository<{{source.Name}}>>();

                                switch (msg.Action)
                                {
                                    case ActionType.Insert:
                                        await Insert(repo, msg);
                                        break;
                                    case ActionType.InsertList:
                                        await InsertList(repo, msg);
                                        break;
                                    case ActionType.Update:
                                        Update(repo, msg);
                                        break;
                                    case ActionType.Delete:
                                        Delete(repo, msg);
                                        break;
                                    case ActionType.GetById:
                                        await GetById(repo, msg);
                                        break;
                                    case ActionType.GetPaginated:
                                        break;
                                }
                            }
                        });
                    }
                    private async Task Insert(IRepository<{{source.Name}}> repo,
                        EntityCommand<{{source.Name}}, Guid> msg)
                    {
                        EntityResult<{{source.Name}}> entityResult = new();
                        entityResult.Data = await repo.InsertAsync(msg.Entity);
                        entityResult.IsSuccess = true;
                        Context.Sender.Tell(entityResult);
                    }
                    private async Task InsertList(IRepository<{{source.Name}}> repo,
                        EntityCommand<{{source.Name}}, Guid> msg)
                    {
                        EntityResult<List<{{source.Name}}>> entityResult = new();
                        await repo.InsertAsync(msg.Entities);
                        entityResult.IsSuccess = true;
                        Context.Sender.Tell(entityResult);
                    }
                    private void Update(IRepository<{{source.Name}}> repo,
                       EntityCommand<{{source.Name}}, Guid> msg)
                    {
                        EntityResult<{{source.Name}}> entityResult = new();
                        repo.Update(msg.Entity);
                        entityResult.IsSuccess = true;
                        Context.Sender.Tell(entityResult);
                    }
                    private void Delete(IRepository<{{source.Name}}> repo,
                       EntityCommand<{{source.Name}}, Guid> msg)
                    {
                        EntityResult<{{source.Name}}> entityResult = new();
                        repo.Delete(msg.Id);
                        entityResult.IsSuccess = true;
                        entityResult.Details = new() { { "Id", msg.Id } };
                        Context.Sender.Tell(entityResult);
                    }
                    private async Task GetById(IRepository<{{source.Name}}> repo,
                       EntityCommand<{{source.Name}}, Guid> msg)
                    {
                        EntityResult<{{source.Name}}> entityResult = new();
                        entityResult.Data = await repo.GetAsync(msg.Id);
                        entityResult.IsSuccess = true;
                        Context.Sender.Tell(entityResult);
                    }
                }

            }
            
            """;

            spc.AddSource($"{source.Name}Controller.g.cs", SourceText.From(sourceText, Encoding.UTF8));

        });
    }


    private static IEnumerable<INamedTypeSymbol> GetAllTypes(INamespaceSymbol namespaceSymbol)
    {
        foreach (var type in namespaceSymbol.GetTypeMembers())
        {
            yield return type;
            foreach (var nestedType in type.GetTypeMembers())
            {
                yield return nestedType;
            }
        }

        foreach (var nestedNs in namespaceSymbol.GetNamespaceMembers())
        {
            foreach (var type in GetAllTypes(nestedNs))
            {
                yield return type;
            }
        }
    }

    private static bool IsValidEntity(INamedTypeSymbol symbol,
        IAssemblySymbol currentAssembly,
        INamedTypeSymbol entityRoot)
    {
       Debug.WriteLine(symbol.Name);
       Debug.WriteLine(symbol.BaseType);
       return symbol.TypeKind == TypeKind.Class &&
           symbol.DeclaredAccessibility == Accessibility.Public &&
           !symbol.IsStatic &&
           !SymbolEqualityComparer.Default.Equals(symbol.ContainingAssembly, currentAssembly) &&
           SymbolEqualityComparer.Default.Equals(symbol.BaseType, entityRoot); ;
    }

}