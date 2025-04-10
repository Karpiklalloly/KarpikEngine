using System;
using DCFApixels.DragonECS;
using DCFApixels.DragonECS.RunnersCore;
using KarpikEngine.Modules.EcsCore;

namespace KarpikEngine.Modules.EcsRunners;

public interface IPausableProcess : IEcsProcess
{
    public void PausableRun();
}

public class PausableRunner : EcsRunner<IPausableProcess>, IPausableProcess
{
    private EcsWorld _world;
    
    public PausableRunner()
    {
        _world = Worlds.Instance.MetaWorld;
    }
    
    public void PausableRun()
    {
        if (_world.Get<TimeComponent>().Paused) return;
        foreach (var process in Process)
        {
            process.PausableRun();
        }
    }
}

public interface IEcsPausableLateRun : IEcsProcess
{
    public void PausableLateRun();
}

public sealed class PausableLateRunner : EcsRunner<IEcsPausableLateRun>, IEcsPausableLateRun
{
    private EcsWorld _world;
    
    public PausableLateRunner()
    {
        _world = Worlds.Instance.MetaWorld;
    }
        
    public void PausableLateRun()
    {
        if (_world.Get<TimeComponent>().Paused) return;
            
        foreach (var process in Process)
        {
            process.PausableLateRun();
        }
    }
}