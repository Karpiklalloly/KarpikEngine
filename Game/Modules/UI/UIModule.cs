namespace Game.Modules;

public class UIModule : IEcsModule
{
    public void Import(EcsPipeline.Builder b)
    {
        b.Add(new DrawPlayerHealthSystem(), EcsConsts.END_LAYER);
    }
}