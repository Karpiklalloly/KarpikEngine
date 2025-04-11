using KarpikEngineMono.Modules.SaveLoad;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using KarpikEngineMono.Modules.EcsCore;
namespace KarpikEngineMono.Modules.EcsCore
{
    public static class ComponentProcessorSecond
    {
        public static ComponentTemplateBase ToComponentTemplate(this IEcsTagComponent component)
        {
            switch (component)
            {
                case Player comp:
                    return new TagComponentTemplate<Player>(comp);
                default:
                    return null;
            }
        return null;
        }
    }
}
