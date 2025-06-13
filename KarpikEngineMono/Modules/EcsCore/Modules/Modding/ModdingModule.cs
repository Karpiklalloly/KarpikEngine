namespace KarpikEngineMono.Modules.EcsCore.Modules.Modding;

public class ModdingModule : IEcsModule
{
    public void Import(EcsPipeline.Builder b)
    {
        b.Add(new ModUpdateSystem());
    }
}