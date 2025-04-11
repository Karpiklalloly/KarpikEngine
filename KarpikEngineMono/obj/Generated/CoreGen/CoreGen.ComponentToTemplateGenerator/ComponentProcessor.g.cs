using KarpikEngine.Modules.SaveLoad;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace KarpikEngine.Modules.EcsCore
{
    public static class ComponentProcessorFirst
    {
        public static ComponentTemplateBase ToComponentTemplate(this IEcsComponent component)
        {
            switch (component)
            {
                case Transform comp:
                    return new ComponentTemplate<Transform>(comp);
                    break;
                case SpriteRenderer comp:
                    return new ComponentTemplate<SpriteRenderer>(comp);
                    break;
                default:
                    System.Console.WriteLine("Component is not a supported struct");
                    break;
            }
        return null;
        }
    }
}
