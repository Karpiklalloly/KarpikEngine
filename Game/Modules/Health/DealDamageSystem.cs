using Karpik.DragonECS;
using Karpik.StatAndAbilities;
using KarpikEngineMono.Modules.EcsCore;

namespace Game.Modules;

public class DealDamageSystem : RunOnRequestSystem<DealDamageRequest, DealDamageSystem.Aspect>
{
    public class Aspect : EcsAspect
    {
        public EcsPool<Health> health = Inc;
    }
    
    protected override void RunOnEvent(ref DealDamageRequest evt, ref Aspect aspect)
    {
        ref var health = ref evt.Target.Get<Health>();
        health.ApplyBuffInstantly(new Buff((float)-evt.Damage, BuffType.Add), BuffRange.Value);
    }
}