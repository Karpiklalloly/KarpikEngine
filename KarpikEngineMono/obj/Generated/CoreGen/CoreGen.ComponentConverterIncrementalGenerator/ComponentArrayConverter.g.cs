using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace KarpikEngine.Modules.EcsCore
{
    public class ComponentArrayConverter : JsonConverter<IEcsComponent[]>
    {
        public override void WriteJson(JsonWriter writer, IEcsComponent[] value, JsonSerializer serializer)
        {
            writer.WriteStartArray();
            foreach (var component in value)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("Type");
                switch (component)
                {
                    case Transform comp:
                        writer.WriteValue("Transform");
                        serializer.Serialize(writer, comp);
                        break;
                    case SpriteRenderer comp:
                        writer.WriteValue("SpriteRenderer");
                        serializer.Serialize(writer, comp);
                        break;
                    default:
                        throw new Exception($"Unknown component type: {component.GetType().Name}");
                }
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
        }

        public override IEcsComponent[] ReadJson(JsonReader reader, Type objectType, IEcsComponent[] existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jArray = JArray.Load(reader);
            var components = new IEcsComponent[jArray.Count];
            int index = 0;
            foreach (JObject obj in jArray)
            {
                string type = obj["Type"].Value<string>();
                switch (type)
                {
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

        public static IEcsComponent[] Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<IEcsComponent[]>(json, new ComponentArrayConverter());
        }

        public static IEcsComponent[] DeserializeFromFile(string filePath)
        {
            string json = System.IO.File.ReadAllText(filePath);
            return Deserialize(json);
        }
    }
}
