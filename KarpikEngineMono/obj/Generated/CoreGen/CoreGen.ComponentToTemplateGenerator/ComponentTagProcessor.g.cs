using KarpikEngine.Modules.SaveLoad;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using KarpikEngine.Modules.EcsCore;
namespace KarpikEngine.Modules.EcsCore
{
    public static class ComponentProcessorSecond
    {
        public static ComponentTemplateBase ToComponentTemplate(this IEcsTagComponent component)
        {
            switch (component)
            {
                case Player comp:
                    return new TagComponentTemplate<Player>(comp);
                    break;
                default:
                    System.Console.WriteLine("Component is not a supported struct");
                    break;
            }
        return null;
        }
    }
}
