using KarpikEngineMono.Modules.SaveLoad;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace KarpikEngineMono.Modules.EcsCore
{
    public static class ComponentProcessorFirst
    {
        public static ComponentTemplateBase ToComponentTemplate(this IEcsComponent component)
        {
            switch (component)
            {
                case Transform comp:
                    return new ComponentTemplate<Transform>(comp);
                case SpriteRenderer comp:
                    return new ComponentTemplate<SpriteRenderer>(comp);
                default:
                    return null;
            }
        return null;
        }
    }
}
