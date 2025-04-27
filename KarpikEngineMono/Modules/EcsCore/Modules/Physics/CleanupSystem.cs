using KarpikEngineMono.Modules.EcsRunners;

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
        foreach (var e in _world.Where(out Aspect a))
        {
            ref var force = ref a.force.Get(e);
            
        }
    }
}