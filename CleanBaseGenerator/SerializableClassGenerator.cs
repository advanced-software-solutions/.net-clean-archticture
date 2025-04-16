using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace CleanBaseGenerator;

[Generator]
public class SerializableClassGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var entityRootClasses = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => s is ClassDeclarationSyntax,
                transform: static (ctx, _) => GetClassSymbol(ctx))
            .Where(static s => s is not null && IsInheritingFromEntityRoot(s!));

        // Generate source code for each found class

        context.RegisterSourceOutput(entityRootClasses,
            static (spc, classSymbol) =>
            {

                GenerateClass(spc, classSymbol!);
            });
    }

    private static INamedTypeSymbol? GetClassSymbol(GeneratorSyntaxContext context)
    {
        var classDeclaration = (ClassDeclarationSyntax)context.Node;
        return context.SemanticModel.GetDeclaredSymbol(classDeclaration) as INamedTypeSymbol;
    }

    private static bool IsInheritingFromEntityRoot(INamedTypeSymbol symbol)
    {
        var baseType = symbol.BaseType;
        while (baseType != null)
        {
            if (baseType.Name == "EntityRoot")
            {
                return true;
            }
            baseType = baseType.BaseType;
        }
        return false;
    }

    private static void GenerateClass(SourceProductionContext context, INamedTypeSymbol classSymbol)
    {
        string namespaceName = "Generated";
        string className = classSymbol.Name;

        string source = $$"""

                          using System.Text.Json.Serialization;
                          using CleanBase.Entities;

                          namespace CleanBase.Generated;

                          /*[JsonSerializable(typeof(List<{{className}}>))]
                          [JsonSerializable(typeof({{className}}))]
                          public partial class EntityRootContextList : JsonSerializerContext { }*/           

                          """;

        context.AddSource($"{className}.g.cs", SourceText.From(source, Encoding.UTF8));
    }
}