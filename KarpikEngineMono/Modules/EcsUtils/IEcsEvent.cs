using DCFApixels.DragonECS;

namespace KarpikEngineMono.Modules.EcsUtils;

public interface IEcsEvent
{
    public entlong Source { get; set; }
    public entlong Target { get; set; }
}