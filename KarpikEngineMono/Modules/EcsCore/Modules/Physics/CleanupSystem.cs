using KarpikEngineMono.Modules.EcsRunners;
using Microsoft.Xna.Framework;

namespace KarpikEngineMono.Modules.EcsCore;

public class CleanupSystem : IEcsFixedRun
{
    private class Aspect : EcsAspect
    {
        public EcsPool<Force> force = Inc;
    }
    
    private EcsDefaultWorld _world = Worlds.Instance.World;
    
    public void FixedRun()
    {
        _world.GetPool<Collisions>().ClearAll();

        foreach (var e in _world.Where(out Aspect a))
        {
            ref var force = ref a.force.Get(e);
            
        }
    }
}