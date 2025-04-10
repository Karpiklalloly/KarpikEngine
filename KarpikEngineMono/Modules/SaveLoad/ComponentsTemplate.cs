using System.Runtime.Serialization;
using KarpikEngine.Modules.EcsCore;

namespace KarpikEngine.Modules.SaveLoad;

[Serializable]
public class ComponentsTemplate
{
    [JsonIgnore]
    public ComponentTemplateBase[] Components;
    [JsonProperty]
    private IEcsComponentMember[] _components;

    public ComponentsTemplate()
    {
        Components = [];
    }

    public ComponentsTemplate(params ComponentTemplateBase[] components)
    {
        Components = components;
    }

    public ComponentsTemplate(params IEcsComponentMember[] components)
    {
        Components = Convert(components);
    }

    public void ApplyTo(int entityID, EcsWorld world)
    {
        foreach (var template in Components)
        {
            template.ApplyTo(entityID, world);
        }
    }
    
    [OnSerializing]
    private void OnSerialize()
    {
        _components = Components.Select(x => (IEcsComponentMember)x.GetRaw()).ToArray();
    }
    
    [OnDeserialized]
    private void OnDeserialize()
    {
        Components = Convert(_components);
    }

    private ComponentTemplateBase[] Convert(params IEcsComponentMember[] components)
    {
        var c = components.Select(ConvertFrom).ToArray();
        return c;
    }

    private ComponentTemplateBase ConvertFrom(IEcsComponentMember x)
    {
        return x switch
        {
            IEcsComponent component => component.ToComponentTemplate(),
            IEcsTagComponent tagComponent => tagComponent.ToComponentTemplate(),
            _ => null
        };
    }
}