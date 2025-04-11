using Karpik.DragonECS;

namespace KarpikEngineMono.Modules.EcsCore
{
    internal class MovementModule : IEcsModule
    {
        public void Import(EcsPipeline.Builder b)
        {
            b
                .Add(new MoveSystem())
                .Add(new MoveToSystem())
                .AddCaller<MoveDirection>()
                .AddCaller<MoveTo>();
        }
    }
}
