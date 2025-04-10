using DCFApixels.DragonECS;

namespace KarpikEngine.Modules.EcsUtils;

public interface IEcsEvent
{
    public entlong Source { get; set; }
    public entlong Target { get; set; }
}