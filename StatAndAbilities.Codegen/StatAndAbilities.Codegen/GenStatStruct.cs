using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using MoveReplace;

namespace Karpik.StatAndAbilities.Codegen
{
    [Generator]
    public class GenStatStruct : ISourceGenerator
    {
        public const string Stat = "Stat";
        public const string RangeStat = "RangeStat";
    
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new StructWithStatReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxReceiver is not StructWithStatReceiver receiver) return;

            Span<StructDeclarationSyntax> structs = receiver.Structs.ToArray();
            foreach (var s in structs)
            {
                if (s.HasAttribute(Stat))
                {
                    var model = context.Compilation.GetSemanticModel(s.SyntaxTree);
                    var structSymbol = model.GetDeclaredSymbol(s) as INamedTypeSymbol;
                    var name = structSymbol.Name;
                    var namespaceName = structSymbol.ContainingNamespace.ToString();
                    var accessibility = structSymbol.DeclaredAccessibility.ToString().ToLower();
                    Write(context, StatGenerator.Generate(name, namespaceName, accessibility));
                }
                else if (s.HasAttribute(RangeStat))
                {
                    var model = context.Compilation.GetSemanticModel(s.SyntaxTree);
                    var structSymbol = model.GetDeclaredSymbol(s) as INamedTypeSymbol;
                    var name = structSymbol.Name;
                    var namespaceName = structSymbol.ContainingNamespace.ToString();
                    var accessibility = structSymbol.DeclaredAccessibility.ToString().ToLower();
                    Write(context, RangeStatGenerator.Generate(name, namespaceName, accessibility));
                }
            }
        }

        private void Write(GeneratorExecutionContext context, List<(string, string)> list)
        {
            foreach (var tuple in list)
            {
                var name = tuple.Item1;
                var text = tuple.Item2;
                context.AddSource(name, SourceText.From(text, Encoding.UTF8));
            }
        }
    }
}