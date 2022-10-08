using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NativeObjectsSourceGenerator
{
    [Generator]
    public class CppObjectGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            if (!(context.SyntaxContextReceiver is CppObjectSyntaxReceiver receiver))
            {
                return;
            }

            foreach (var symbol in receiver.Interfaces)
            {
                EmitStubForInterface(context, symbol);
            }
        }

        public void EmitStubForInterface(GeneratorExecutionContext context, INamedTypeSymbol symbol)
        {
            var sourceBuilder = new StringBuilder(@"
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace {namespace}
{
    unsafe partial struct {typeName}
    {
        {functions}      
    }
}
");

            var interfaceName = symbol.ToString();
            var typeName = symbol.Name;
            var @namespace = symbol.ContainingNamespace.ToString();
            var functions = new StringBuilder();

            foreach (var member in symbol.GetMembers())
            {
                if (member is not IMethodSymbol method)
                {
                    continue;
                }

                if (!method.IsPartialDefinition)
                {
                    continue;
                }

                if (method.DeclaredAccessibility != Accessibility.NotApplicable)
                {
                    functions.Append(method.DeclaredAccessibility.ToString().ToLower());
                    functions.Append(" ");
                }

                functions.Append($"partial {method.ReturnType} {method.Name}(");

                bool firstParameter = true;

                var genericTypes = new StringBuilder();
                var parameterList = new StringBuilder();

                genericTypes.Append("IntPtr");
                parameterList.Append("ptr");

                foreach (var parameter in method.Parameters)
                {
                    if (firstParameter)
                    {
                        functions.Append($"{parameter.OriginalDefinition} {parameter.Name}");
                        firstParameter = false;
                    }
                    else
                    {
                        functions.Append($", {parameter.OriginalDefinition} {parameter.Name}");
                    }

                    genericTypes.Append($", {parameter.OriginalDefinition}");
                    parameterList.Append($", {parameter.Name}");
                }

                if (method.ReturnsVoid)
                {
                    genericTypes.Append(", void");
                }
                else
                {
                    genericTypes.Append($", {method.ReturnType}");
                }

                functions.AppendLine(")");
                functions.AppendLine("            {");

                functions.AppendLine($@"            var address = LldbExtension.MethodLocator.Find(""liblldb"", ""{@namespace}.{typeName}.{method.Name}"");");
                functions.AppendLine($@"                var method = (delegate* unmanaged[Cdecl, MemberFunction]<{genericTypes}>)address;");
                functions.AppendLine($@"                var ptr = (IntPtr)Unsafe.AsPointer(ref this);");

                functions.Append("                ");

                if (!method.ReturnsVoid)
                {
                    functions.Append("return ");
                }

                functions.AppendLine($"method({parameterList});");


                functions.AppendLine("            }");

            }
            
            sourceBuilder.Replace("{namespace}", @namespace);
            sourceBuilder.Replace("{typeName}", typeName);
            sourceBuilder.Replace("{interfaceName}", interfaceName);
            sourceBuilder.Replace("{functions}", functions.ToString());

            context.AddSource($"{symbol.ContainingNamespace?.Name ?? "_"}.{symbol.Name}.g.cs", sourceBuilder.ToString());
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForPostInitialization(EmitAttribute);

            context.RegisterForSyntaxNotifications(() => new CppObjectSyntaxReceiver());
        }

        private void EmitAttribute(GeneratorPostInitializationContext context)
        {
            context.AddSource("CppObjectAttribute.g.cs", @"
using System;

[AttributeUsage(AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
internal class CppObjectAttribute : Attribute { }
");
        }
    }

    public class CppObjectSyntaxReceiver : ISyntaxContextReceiver
    {
        public List<INamedTypeSymbol> Interfaces { get; } = new();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is StructDeclarationSyntax classDeclarationSyntax
                && classDeclarationSyntax.AttributeLists.Count > 0)
            {
                var symbol = (INamedTypeSymbol)context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax);

                if (symbol.GetAttributes().Any(a => a.AttributeClass.ToDisplayString() == "CppObjectAttribute"))
                {
                    Interfaces.Add(symbol);
                }
            }
        }
    }
}