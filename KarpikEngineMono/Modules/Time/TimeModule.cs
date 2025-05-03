namespace KarpikEngineMono.Modules;

public class TimeModule : IEcsModule
{
    public void Import(EcsPipeline.Builder b)
    {
        b
            .Add(new TweenUpdateSystem(), EcsConsts.POST_END_LAYER)
            .Add(new TweenUpdatePausableSystem(), EcsConsts.POST_END_LAYER);
    }
}