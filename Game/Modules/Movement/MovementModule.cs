namespace Game.Modules;

public class MovementModule : IEcsModule
{
    public void Import(EcsPipeline.Builder b)
    {
        b.Add(new HandleInputMovementSystem(), EcsConsts.BEGIN_LAYER);
    }
}