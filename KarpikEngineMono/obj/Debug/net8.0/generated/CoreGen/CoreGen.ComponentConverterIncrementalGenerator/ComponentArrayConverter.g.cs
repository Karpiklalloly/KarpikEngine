using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace KarpikEngineMono.Modules.EcsCore
{
    public class ComponentArrayConverter : JsonConverter<IEcsComponentMember[]>
    {
        public override void WriteJson(JsonWriter writer, IEcsComponentMember[] value, JsonSerializer serializer)
        {
            writer.WriteStartArray();
            foreach (var component in value)
            {
                JObject obj = JObject.FromObject(component, serializer);
                string typeName;
                switch (component)
                {
                    case Player comp:
                        typeName = "Player";
                        break;
                    case Transform comp:
                        typeName = "Transform";
                        break;
                    case SpriteRenderer comp:
                        typeName = "SpriteRenderer";
                        break;
                    default:
                        throw new Exception($"Unknown component type: {component.GetType().Name}");
                }
                obj.AddFirst(new JProperty("Type", typeName));
                obj.WriteTo(writer);
            }
            writer.WriteEndArray();
        }

        public override IEcsComponentMember[] ReadJson(JsonReader reader, Type objectType, IEcsComponentMember[] existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
System.Diagnostics.Debugger.Launch();
            var jArray = JArray.Load(reader);
            var components = new IEcsComponentMember[jArray.Count];
            int index = 0;
            foreach (JObject obj in jArray)
            {
                string type = obj["Type"].Value<string>();
                switch (type)
                {
                    case "Player":
                        components[index] = obj.ToObject<Player>(serializer);
                        break;
                    case "Transform":
                        components[index] = obj.ToObject<Transform>(serializer);
                        break;
                    case "SpriteRenderer":
                        components[index] = obj.ToObject<SpriteRenderer>(serializer);
                        break;
                    default:
                        throw new Exception($"Unknown component type: {type}");
                }
                index++;
            }
            return components;
        }

        public static IEcsComponentMember[] Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<IEcsComponentMember[]>(json, new ComponentArrayConverter());
        }

        public static IEcsComponentMember[] DeserializeFromFile(string filePath)
        {
            string json = System.IO.File.ReadAllText(filePath);
            return Deserialize(json);
        }
    }
}
