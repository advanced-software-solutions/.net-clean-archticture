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
        var entityProvider = context.CompilationProvider
            .SelectMany((compilation, _) =>
            {
                // Resolve EntityRoot from the correct namespace
                var entityRoot = compilation.GetTypeByMetadataName("CleanBase.EntityRoot");
                if (entityRoot == null) return Enumerable.Empty<INamedTypeSymbol>();
                return GetAllTypes(compilation.GlobalNamespace)
                                 .Where(t => IsValidEntity(t, compilation.Assembly, entityRoot));
            });

        var dependencyProvider = context.CompilationProvider
            .Select((compilation, _) => (
                RootService: GetGenericInterface(compilation, "CleanBusiness.RootService`1"),
                IRepository: GetGenericInterface(compilation, "CleanBase.CleanAbstractions.CleanOperation.IRepository`1")
            ));

        var combinedProvider = entityProvider.Combine(dependencyProvider);

        context.RegisterSourceOutput(combinedProvider, (spc, source) =>
        {
            var (entity, dependencies) = source;
            var (rootService, iRepository) = dependencies;

            if (rootService == null || iRepository == null) return;

            GenerateServiceClass(spc, entity, rootService, iRepository);
        });
    }

    private static void GenerateServiceClass(
        SourceProductionContext context,
        INamedTypeSymbol entity,
        INamedTypeSymbol rootService,
        INamedTypeSymbol iRepository)
    {
        var namespaceName = "CleanBusiness.Generated";
        var className = $"{entity.Name}Service";
        var interfaceName = $"I{entity.Name}Service";

        var source = $$"""
            using CleanBase.Entities;
            using {{rootService.ContainingNamespace}};
            using {{iRepository.ContainingNamespace}};

            namespace {{namespaceName}};

            public partial class {{className}} : RootService<{{entity.Name}}>, {{interfaceName}}
            {
                public {{className}}(IRepository<{{entity.Name}}> repository) 
                    : base(repository)
                {
                }
            }
            """;

        context.AddSource($"{className}.g.cs", SourceText.From(source, Encoding.UTF8));
        var sourceInterface = $$"""
            using CleanBase.CleanAbstractions.CleanBusiness;
            using CleanBase.Entities;
            namespace {{namespaceName}};
            public interface {{interfaceName}} : IRootService<{{entity.Name}}> {}
            """;
        context.AddSource($"{interfaceName}.g.cs", SourceText.From(sourceInterface, Encoding.UTF8));
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

    private static INamedTypeSymbol? GetGenericInterface(Compilation compilation, string metadataName)
    {
      var z = compilation.GetType().GetMembers();
      return  compilation.GetTypeByMetadataName(metadataName);
    }
}