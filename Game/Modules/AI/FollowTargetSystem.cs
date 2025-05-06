using KarpikEngineMono;
using KarpikEngineMono.Modules;
using KarpikEngineMono.Modules.EcsCore;
using KarpikEngineMono.Modules.EcsRunners;

namespace Game.Modules;

public class FollowTargetSystem : IEcsFixedRun
{
    private class Aspect : EcsAspect
    {
        public EcsPool<Transform> transform = Inc;
        public EcsPool<FollowTarget> followTarget = Inc;
        public EcsPool<Speed> speed = Inc;
    }
    
    private EcsDefaultWorld _world = Worlds.Instance.World;

    public void FixedRun()
    {
        foreach (var e in _world.Where(out Aspect a))
        {
            ref var transform = ref a.transform.Get(e);
            ref var followTarget = ref a.followTarget.Get(e);
            ref var speed = ref a.speed.Get(e);
            
            ref var targetTransform = ref a.transform.Get(followTarget.Target);

            var entity = _world.GetEntityLong(e);
            var direction = (targetTransform.Position - transform.Position);
            direction.Normalize();
            entity.MoveBySpeed(direction);
        }
    }
}