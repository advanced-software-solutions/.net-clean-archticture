using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CleanAPIGenerator
{
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

                    return  GetAllTypes(compilation.GlobalNamespace)
                        .Where(t => IsValidEntity(t, compilation.Assembly, entityRoot));
                });

            context.RegisterSourceOutput(entities, (spc, source) =>
            {

                var sourceText = $$"""

            using Akka.Actor;
            using CleanBase.Entities;

            namespace CleanAPI.Controllers;

            public partial class {{source.Name}}Controller(IActorRef actorRef) : AppBaseController<{{source.Name}}>(actorRef)
            {

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
}
