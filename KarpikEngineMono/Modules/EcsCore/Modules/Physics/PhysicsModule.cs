using Karpik.DragonECS;

namespace KarpikEngineMono.Modules.EcsCore;

public class PhysicsModule : IEcsModule
{
    public void Import(EcsPipeline.Builder b)
    {
        b
            .Add(new GlobalForceApplicationSystem())
            .Add(new MovementSystem())
            .Add(new CollisionDetectionSystem())
            .Add(new CollisionResolutionSystem())
            .Add(new CleanupSystem(), EcsConsts.POST_END_LAYER)
            .AddFixedCaller<CollisionsEvent>(EcsConsts.END_LAYER);
    }
}