using Karpik.DragonECS;

namespace Game.Modules;

public class HealthModule : IEcsModule
{
    public void Import(EcsPipeline.Builder b)
    {
        b
            .Add(new DealDamageOnContactSystem())
            .Add(new DealDamageEventSystem())
            .Add(new DealDamageSystem())
            .AddCaller<DealDamageEvent>()
            .AddCaller<DealDamageRequest>()
            ;
    }
}