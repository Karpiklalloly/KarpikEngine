namespace KarpikEngineMono.Modules.EcsCore;

public class InputModule : IEcsModule
{
    public void Import(EcsPipeline.Builder b)
    {
        b.Add(new InputUpdateSystem(), EcsConsts.PRE_BEGIN_LAYER);
    }
}