using Karpik.DragonECS;

namespace Game.Modules;

public class HealthModule : IEcsModule
{
    public void Import(EcsPipeline.Builder b)
    {
        b
            .Add(new DealDamageEventSystem())
            .Add(new DealDamageSystem())
            .Add(new DealDamageOnContactSystem())
            .AddCaller<DealDamageEvent>()
            .AddCaller<DealDamageRequest>();
    }
}