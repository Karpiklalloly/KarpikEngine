using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CoreGen;

[Generator]
public class ComponentConverterIncrementalGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Собираем типы с [Serializable] и IEcsComponent
        var componentTypes = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: (node, _) => node is TypeDeclarationSyntax tds && tds.AttributeLists.Any(al => al.Attributes.Any(a => a.Name.ToString() == "Serializable")),
                transform: (ctx, _) => (TypeDeclarationSyntax)ctx.Node)
            .Where(tds => tds.BaseList?.Types.Any(t => t.Type.ToString() == "IEcsComponent") == true)
            .Select((tds, _) => tds.Identifier.Text)
            .Collect();

        // Генерируем код на основе собранных типов
        context.RegisterSourceOutput(componentTypes, (spc, types) =>
        {
            var sb = new StringBuilder();
            sb.AppendLine("using Newtonsoft.Json;");
            sb.AppendLine("using Newtonsoft.Json.Linq;");
            sb.AppendLine("using System;");
            sb.AppendLine();
            sb.AppendLine("namespace KarpikEngine.Modules.EcsCore");
            sb.AppendLine("{");
            sb.AppendLine("    public class ComponentArrayConverter : JsonConverter<IEcsComponent[]>");
            sb.AppendLine("    {");
            sb.AppendLine("        public override void WriteJson(JsonWriter writer, IEcsComponent[] value, JsonSerializer serializer)");
            sb.AppendLine("        {");
            sb.AppendLine("            writer.WriteStartArray();");
            sb.AppendLine("            foreach (var component in value)");
            sb.AppendLine("            {");
            sb.AppendLine("                writer.WriteStartObject();");
            sb.AppendLine("                writer.WritePropertyName(\"Type\");");
            sb.AppendLine("                switch (component)");
            sb.AppendLine("                {");
            foreach (var type in types)
            {
                sb.AppendLine($"                    case {type} comp:");
                sb.AppendLine($"                        writer.WriteValue(\"{type}\");");
                sb.AppendLine("                        serializer.Serialize(writer, comp);"); // Сериализуем свойства напрямую
                sb.AppendLine("                        break;");
            }
            sb.AppendLine("                    default:");
            sb.AppendLine("                        throw new Exception($\"Unknown component type: {component.GetType().Name}\");");
            sb.AppendLine("                }");
            sb.AppendLine("                writer.WriteEndObject();");
            sb.AppendLine("            }");
            sb.AppendLine("            writer.WriteEndArray();");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public override IEcsComponent[] ReadJson(JsonReader reader, Type objectType, IEcsComponent[] existingValue, bool hasExistingValue, JsonSerializer serializer)");
            sb.AppendLine("        {");
            sb.AppendLine("            var jArray = JArray.Load(reader);");
            sb.AppendLine("            var components = new IEcsComponent[jArray.Count];");
            sb.AppendLine("            int index = 0;");
            sb.AppendLine("            foreach (JObject obj in jArray)");
            sb.AppendLine("            {");
            sb.AppendLine("                string type = obj[\"Type\"].Value<string>();");
            sb.AppendLine("                switch (type)");
            sb.AppendLine("                {");
            foreach (var type in types)
            {
                sb.AppendLine($"                    case \"{type}\":");
                sb.AppendLine($"                        components[index] = obj.ToObject<{type}>(serializer);"); // Десериализуем весь объект
                sb.AppendLine("                        break;");
            }
            sb.AppendLine("                    default:");
            sb.AppendLine("                        throw new Exception($\"Unknown component type: {type}\");");
            sb.AppendLine("                }");
            sb.AppendLine("                index++;");
            sb.AppendLine("            }");
            sb.AppendLine("            return components;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public static IEcsComponent[] Deserialize(string json)");
            sb.AppendLine("        {");
            sb.AppendLine("            return JsonConvert.DeserializeObject<IEcsComponent[]>(json, new ComponentArrayConverter());");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public static IEcsComponent[] DeserializeFromFile(string filePath)");
            sb.AppendLine("        {");
            sb.AppendLine("            string json = System.IO.File.ReadAllText(filePath);");
            sb.AppendLine("            return Deserialize(json);");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            spc.AddSource("ComponentArrayConverter.g.cs", sb.ToString());
        });
    }
}