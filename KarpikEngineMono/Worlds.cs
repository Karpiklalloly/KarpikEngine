using DCFApixels.DragonECS;

namespace KarpikEngine;

public class Worlds
{
    private static Worlds _instance = new Worlds();
    public static Worlds Instance => _instance;
    
    public EcsDefaultWorld World { get; } = new EcsDefaultWorld();
    public EcsEventWorld EventWorld { get; } = new EcsEventWorld();
    public MetaWorld MetaWorld { get; } = new MetaWorld();
    public EcsPipeline Pipeline { get; internal set; }
}