using Karpik.DragonECS;

namespace KarpikEngineMono.Modules.EcsCore
{
    public class MoveSystem : RunOnEventSystem<MoveDirection, MoveSystem.MoveSystemAspect>
    {
        public class MoveSystemAspect : EcsAspect
        {
            public EcsPool<Transform> position = Inc;
        }

        public override void RunOnEvent(ref MoveDirection evt, ref MoveSystemAspect aspect)
        {
            var e = evt.Target;
            e.Move(ref evt);
        }
    }
}
