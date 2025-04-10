using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CoreGen;

[Generator]
public class ComponentToTemplateGenerator : IIncrementalGenerator
{
   public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var componentTypes = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: (node, _) => node is StructDeclarationSyntax sds && sds.AttributeLists.Any(al => al.Attributes.Any(a => a.Name.ToString() == "Serializable")),
                transform: (ctx, _) => (StructDeclarationSyntax)ctx.Node)
            .Where(sds => sds.BaseList?.Types.Any(t => t.Type.ToString() == "IEcsComponent") == true)
            .Select((sds, _) => sds.Identifier.Text)
            .Collect();

        context.RegisterSourceOutput(componentTypes, (spc, types) =>
        {
            var sb = new StringBuilder();
            sb.AppendLine("using KarpikEngine.Modules.SaveLoad;");
            sb.AppendLine("using Newtonsoft.Json;");
            sb.AppendLine("using Newtonsoft.Json.Linq;");
            sb.AppendLine("namespace KarpikEngine.Modules.EcsCore");
            sb.AppendLine("{");
            sb.AppendLine("    public static class ComponentProcessorFirst");
            sb.AppendLine("    {");
            sb.AppendLine("        public static ComponentTemplateBase ToComponentTemplate(this IEcsComponent component)");
            sb.AppendLine("        {");
            sb.AppendLine("            switch (component)");
            sb.AppendLine("            {");
            foreach (var type in types)
            {
                sb.AppendLine($"                case {type} comp:");
                sb.AppendLine($"                    return new ComponentTemplate<{type}>(comp);");
                sb.AppendLine("                    break;");
            }
            sb.AppendLine("                default:");
            sb.AppendLine("                    System.Console.WriteLine(\"Component is not a supported struct\");");
            sb.AppendLine("                    break;");
            sb.AppendLine("            }");
            sb.AppendLine("        return null;");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            spc.AddSource("ComponentProcessor.g.cs", sb.ToString());
        });
        
        componentTypes = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: (node, _) => node is StructDeclarationSyntax sds && sds.AttributeLists.Any(al => al.Attributes.Any(a => a.Name.ToString() == "Serializable")),
                transform: (ctx, _) => (StructDeclarationSyntax)ctx.Node)
            .Where(sds => sds.BaseList?.Types.Any(t => t.Type.ToString() == "IEcsTagComponent") == true)
            .Select((sds, _) => sds.Identifier.Text)
            .Collect();

        context.RegisterSourceOutput(componentTypes, (spc, types) =>
        {
            var sb = new StringBuilder();
            sb.AppendLine("using KarpikEngine.Modules.SaveLoad;");
            sb.AppendLine("using Newtonsoft.Json;");
            sb.AppendLine("using Newtonsoft.Json.Linq;");
            sb.AppendLine("using KarpikEngine.Modules.EcsCore;");
            sb.AppendLine("namespace KarpikEngine.Modules.EcsCore");
            sb.AppendLine("{");
            sb.AppendLine("    public static class ComponentProcessorSecond");
            sb.AppendLine("    {");
            sb.AppendLine("        public static ComponentTemplateBase ToComponentTemplate(this IEcsTagComponent component)");
            sb.AppendLine("        {");
            sb.AppendLine("            switch (component)");
            sb.AppendLine("            {");
            foreach (var type in types)
            {
                sb.AppendLine($"                case {type} comp:");
                sb.AppendLine($"                    return new TagComponentTemplate<{type}>(comp);");
                sb.AppendLine("                    break;");
            }
            sb.AppendLine("                default:");
            sb.AppendLine("                    System.Console.WriteLine(\"Component is not a supported struct\");");
            sb.AppendLine("                    break;");
            sb.AppendLine("            }");
            sb.AppendLine("        return null;");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            spc.AddSource("ComponentTagProcessor.g.cs", sb.ToString());
        });
    }
}