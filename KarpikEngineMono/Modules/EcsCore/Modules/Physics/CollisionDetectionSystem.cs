using KarpikEngineMono.Modules.EcsRunners;

namespace KarpikEngineMono.Modules.EcsCore;

public class CollisionDetectionSystem : IEcsFixedRun
{
    private IDetectionMode _mode;

    public CollisionDetectionSystem(IDetectionMode mode)
    {
        _mode = mode;
    }
    
    public void FixedRun()
    {
        _mode.Collect();
        _mode.Detect();
    }


    
    
    

}