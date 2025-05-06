using System;
using DCFApixels.DragonECS;
using DCFApixels.DragonECS.RunnersCore;
using KarpikEngineMono.Modules.EcsCore;

namespace KarpikEngineMono.Modules.EcsRunners;

public interface IEcsPausableRun : IEcsProcess
{
    public void PausableRun();
}

public class EcsPausableRunner : EcsRunner<IEcsPausableRun>, IEcsPausableRun
{
    private EcsWorld _world;
    
    public EcsPausableRunner()
    {
        _world = Worlds.Instance.MetaWorld;
    }
    
    public void PausableRun()
    {
        if (Time.IsPaused) return;
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
        if (Time.IsPaused) return;
            
        foreach (var process in Process)
        {
            process.PausableLateRun();
        }
    }
}