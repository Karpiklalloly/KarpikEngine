using DCFApixels.DragonECS.RunnersCore;

namespace KarpikEngineMono.Modules.EcsRunners;

public interface IEcsFixedRun : IEcsProcess
{
    public void FixedRun();
}

public class EcsFixedRunRunner : EcsRunner<IEcsFixedRun>, IEcsFixedRun
{
    private RunHelper _helper;
    protected override void OnSetup()
    {
        _helper = new RunHelper(this);
    }
    public void FixedRun()
    {
        _helper.Run(p => p.FixedRun());
    }
}
