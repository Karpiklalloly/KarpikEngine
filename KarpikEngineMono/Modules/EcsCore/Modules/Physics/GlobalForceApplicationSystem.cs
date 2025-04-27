using KarpikEngineMono.Modules.EcsRunners;

namespace KarpikEngineMono.Modules.EcsCore;

public class GlobalForceApplicationSystem : IEcsFixedRun
{
    private class Aspect : EcsAspect
    {
        public EcsPool<RigidBody> rigidBody = Inc;
        public EcsPool<Force> force = Inc;
    }

    private EcsDefaultWorld _world = Worlds.Instance.World;
    
    public void FixedRun()
    {
        foreach (var e in _world.Where(out Aspect a))
        {
            ref var rigidBody = ref a.rigidBody.Get(e);
            ref var force = ref a.force.Get(e);
            
            if (rigidBody.Type != RigidBody.BodyType.Dynamic) continue;

            if (rigidBody.InverseMass > 0)
            {
                
            }
        }
    }
}