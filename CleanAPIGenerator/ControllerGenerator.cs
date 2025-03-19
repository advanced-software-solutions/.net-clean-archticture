using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CleanAPIGenerator;

[Generator]
public class ControllerGenerator : IIncrementalGenerator
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
        using CleanBase.Entities;
        using CleanBase.Dtos;
        using Enyim.Caching;
        using CleanBase.CleanAbstractions.CleanOperation;
        using CleanBase;

        namespace CleanAPI.Controllers;

        public partial class {{source.Name}}Controller : AppBaseController<{{source.Name}}>
        {

        }

        #region FastEdnpoints

                public class {{source.Name}}Create : FastEndpoints.Endpoint<{{source.Name}}, {{source.Name}}>
                {
                    public IRepository<{{source.Name}}> _repository { get; set; }
                    public override void Configure()
                    {
                        Post("/api/{{source.Name}}");
        #if DEBUG
                        AllowAnonymous();
        #endif
                        SerializerContext<AppJsonSerializerContext>();
                    }

                    public override async Task HandleAsync({{source.Name}} req, CancellationToken ct)
                    {
                        var result = await _repository.InsertAsync(req);
                        await SendAsync(result);
                    }
                }
                public class {{source.Name}}ListCreate : FastEndpoints.Endpoint<List<{{source.Name}}>, List<{{source.Name}}>>
                {
                    public IRepository<{{source.Name}}> _repository { get; set; }
                    public override void Configure()
                    {
                        Post("/api/{{source.Name}}/CreateList");
        #if DEBUG
                        AllowAnonymous();
        #endif
                        SerializerContext<AppJsonSerializerContext>();
                    }

                    public override async Task HandleAsync(List<{{source.Name}}> req, CancellationToken ct)
                    {
                        await _repository.InsertAsync(req);
                        await SendAsync(req);
                    }
                }

                public class {{source.Name}}GetById : FastEndpoints.EndpointWithoutRequest<{{source.Name}}>
                {
                    public IRepository<{{source.Name}}> _repository { get; set; }
                    public IMemcachedClient _factory { get; set; }
                    public override void Configure()
                    {
                        Get("/api/{{source.Name}}/{id}");
        #if DEBUG
                        AllowAnonymous();
        #endif
                        ResponseCache(30);
                        SerializerContext<AppJsonSerializerContext>();
                    }

                    public override async Task HandleAsync(CancellationToken ct)
                    {
                        var req = Route<Guid>("id");
                        var exists = await _factory.GetAsync<EntityResult<{{source.Name}}>>("{{source.Name}}:id:" + req);
                        if (exists.HasValue)
                        {
                            await SendAsync((await _factory.GetAsync<{{source.Name}}>("{{source.Name}}:id:" + req)).Value);
                            return;
                        }
                        var result = await _repository.GetAsync(req);
                        await _factory.SetAsync("{{source.Name}}:id:" + req, result, TimeSpan.FromSeconds(30));
                        await SendAsync(result);
                    }
                }

                public class {{source.Name}}Delete : FastEndpoints.EndpointWithoutRequest
                {
                    public IRepository<{{source.Name}}> _repository { get; set; }
                    public override void Configure()
                    {
                        Delete("/api/{{source.Name}}/{id}");
        #if DEBUG
                        AllowAnonymous();
        #endif
                        SerializerContext<AppJsonSerializerContext>();
                    }

                    public override async Task HandleAsync(CancellationToken ct)
                    {
                        var req = Route<Guid>("id");
                        _repository.Delete(req);
                        await SendOkAsync(ct);
                    }
                }

                public class {{source.Name}}Update : FastEndpoints.Endpoint<{{source.Name}},{{source.Name}}>
                {
                    public IRepository<{{source.Name}}> _repository { get; set; }
                    public override void Configure()
                    {
                        Put("/api/{{source.Name}}");
        #if DEBUG
                        AllowAnonymous();
        #endif
                        SerializerContext<AppJsonSerializerContext>();
                    }

                    public override async Task HandleAsync({{source.Name}} req, CancellationToken ct)
                    {

                        var result = _repository.Update(req);
                        await SendAsync(result.Entity,cancellation: ct);
                    }
                }

        #endregion
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

    private static bool IsValidEntity(
        INamedTypeSymbol symbol,
        IAssemblySymbol currentAssembly,
        INamedTypeSymbol entityRoot)
        => symbol.TypeKind == TypeKind.Class &&
           symbol.DeclaredAccessibility == Accessibility.Public &&
           !symbol.IsStatic &&
           !SymbolEqualityComparer.Default.Equals(symbol.ContainingAssembly, currentAssembly) &&
           SymbolEqualityComparer.Default.Equals(symbol.BaseType, entityRoot);
}
