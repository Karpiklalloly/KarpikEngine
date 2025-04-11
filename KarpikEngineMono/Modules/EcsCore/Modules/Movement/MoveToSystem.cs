using Karpik.DragonECS;

namespace KarpikEngineMono.Modules.EcsCore
{
    public class MoveToSystem : IEcsRunOnRequest<MoveTo>
    {
        public void RunOnEvent(ref MoveTo evt)
        {
            ref var pos = ref evt.Target.Get<Transform>();

            pos.Position = evt.Position;
        }
    }
}
