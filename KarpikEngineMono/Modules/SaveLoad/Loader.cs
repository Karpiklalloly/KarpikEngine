using KarpikEngineMono.Modules.EcsCore;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KarpikEngineMono.Modules.SaveLoad;

public static class Loader
{
    internal static ContentManager Manager;
    private static string RootDirectory => Path.Combine(@"C:\Users\artem\source\repos\KarpikEngineMono\KarpikEngineMono\", Manager.RootDirectory);
    
    public static ComponentsTemplate LoadTemplate(string path)
    {
        path = ApproveFileName(path, "json");
        path = Path.Combine(RootDirectory, path);
        var json = File.ReadAllText(path);
        var options = new JsonSerializerSettings { Converters = { new ComponentArrayConverter() } };
        return JsonConvert.DeserializeObject<ComponentsTemplate>(json, options);
    }

    public static void Serialize<T>(T obj, string path)
    {
        var settings = new JsonSerializerSettings
        {
            Converters = { new ComponentArrayConverter() }, // Один конвертер для всего массива
            Formatting = Formatting.Indented
        };
        path = ApproveFileName(path, "json");
        path = Path.Combine(RootDirectory, path);
        if (!File.Exists(path)) File.Create(path);
        var json = JsonConvert.SerializeObject(obj, settings);
        File.WriteAllText(path, json);
    }
    
    public static Texture2D LoadTexture(string path) => Manager.Load<Texture2D>(path);
    public static T Load<T>(string path) => Manager.Load<T>(path);
    
    private static string ApproveFileName(string path, string extension)
    {
        extension = $".{extension}";
        if (path[^extension.Length..] != extension)
        {
            return path + extension;
        }
        return path;
    }
}