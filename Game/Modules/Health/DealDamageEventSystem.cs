using Karpik.DragonECS;
using KarpikEngineMono;

namespace Game.Modules;

public class DealDamageEventSystem : IEcsRunOnEvent<DealDamageEvent>
{
    private EcsDefaultWorld _world = Worlds.Instance.World;
    
    public void RunOnEvent(ref DealDamageEvent evt)
    {
        ref var request = ref _world.GetPool<DealDamageRequest>().TryAddOrGet(evt.Target.ID);
        request.Target = evt.Target;
        request.Damage += evt.Damage;
    }
}